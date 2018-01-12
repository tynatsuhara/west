using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class TownLocation : Location {

	// Things contained locally
	private BitArray trails;
	private BitArray buildingSpaces;

	public TownLocation(Map parent, float x, float y) : base(parent, true) {
		var icons = new string[]{"{", "}", "[", "]", "> <", "*", "@", ">", "<"};
		icon = icons[Random.Range(0, icons.Length)];
		this.worldLocation = new SerializableVector3(new Vector3(x, y, 0));
		name = NameGen.townName.Generate("<name>");
		connections = new System.Guid[Random.Range(1, 6)];
	}

	public bool DoneConnecting() {
		return !connections.Contains(System.Guid.Empty);
	}

	public bool CanConnectTo(TownLocation l) {
		return connections.Contains(System.Guid.Empty) 
				&& l.connections.Contains(System.Guid.Empty)
				&& !connections.Contains(l.guid)
				&& !l.connections.Contains(l.guid);
	}

	public void Connect(TownLocation l) {
		int index = System.Array.IndexOf(connections, System.Guid.Empty);
		connections[index] = l.guid;
	}

	// Build the street layout for a town
	public void Generate() {
		connections = connections.Where(x => x != System.Guid.Empty).ToArray();
		List<Location> links = connections.Select(x => parent.locations[x]).ToList();
		
		List<Location> nLinks = links.Where(l => l.worldLocation.y > worldLocation.y && 
			Mathf.Abs((l.worldLocation.val - worldLocation.val).x) < (Mathf.Abs((l.worldLocation.val - worldLocation.val).y)))
			.OrderBy(x => x.worldLocation.val.x).ToList();
		List<Location> sLinks = links.Where(l => l.worldLocation.y < worldLocation.y && 
			Mathf.Abs((l.worldLocation.val - worldLocation.val).x) < (Mathf.Abs((l.worldLocation.val - worldLocation.val).y)))
			.OrderBy(x => x.worldLocation.val.x).ToList();
		List<Location> eLinks = links.Where(l => l.worldLocation.x > worldLocation.x && 
			Mathf.Abs((l.worldLocation.val - worldLocation.val).y) < (Mathf.Abs((l.worldLocation.val - worldLocation.val).x)))
			.OrderBy(x => x.worldLocation.val.y).ToList();
		List<Location> wLinks = links.Where(l => l.worldLocation.x < worldLocation.x && 
			Mathf.Abs((l.worldLocation.val - worldLocation.val).y) < (Mathf.Abs((l.worldLocation.val - worldLocation.val).x)))
			.OrderBy(x => x.worldLocation.val.y).ToList();

		int minRoadsNS = Mathf.Max(nLinks.Count, sLinks.Count);
		int minRoadsEW = Mathf.Max(eLinks.Count, wLinks.Count);
		int minRoadDist = 3;
		int maxRoadDist = 8;

		// place N/S roads and grow the width if necessary
		var nsRoadCoords = new List<int>();
		int p = Random.Range(minRoadDist, maxRoadDist);
		for (int i = 0; i < minRoadsNS; i++) {
			nsRoadCoords.Add(p);
			p += Random.Range(minRoadDist, maxRoadDist);
		}
		width = Mathf.Max(p, width);

		// place E/W roads and grow the height if necessary
		var ewRoadCoords = new List<int>();
		p = Random.Range(minRoadDist, maxRoadDist);
		for (int i = 0; i < minRoadsEW; i++) {
			ewRoadCoords.Add(p);
			p += Random.Range(minRoadDist, maxRoadDist);
		}
		height = Mathf.Max(p, height);

		// Place teleporters
		List<int> exits = new List<int>();

		List<int> nPlaces = Subset(nsRoadCoords, nLinks.Count);
		for (int i = 0; i < nPlaces.Count; i++) {
			exits.Add(Val(nPlaces[i], height - 1));
			teleporters.Add(new Teleporter.TeleporterData(nLinks[i].guid, new Vector3(nPlaces[i] + LevelBuilder.TILE_SIZE/4f, 1, height) * LevelBuilder.TILE_SIZE));
		}

		List<int> sPlaces = Subset(nsRoadCoords, sLinks.Count);
		for (int i = 0; i < sPlaces.Count; i++) {
			exits.Add(Val(sPlaces[i], 0));
			teleporters.Add(new Teleporter.TeleporterData(sLinks[i].guid, new Vector3(sPlaces[i] + LevelBuilder.TILE_SIZE/4f, 1, 0) * LevelBuilder.TILE_SIZE));
		}

		List<int> ePlaces = Subset(ewRoadCoords, eLinks.Count);
		for (int i = 0; i < ePlaces.Count; i++) {
			exits.Add(Val(width - 1, ePlaces[i]));
			teleporters.Add(new Teleporter.TeleporterData(eLinks[i].guid, new Vector3(width, 1, ePlaces[i] + LevelBuilder.TILE_SIZE/4f) * LevelBuilder.TILE_SIZE));
		}

		List<int> wPlaces = Subset(ewRoadCoords, wLinks.Count);
		for (int i = 0; i < wPlaces.Count; i++) {
			exits.Add(Val(0, wPlaces[i]));
			teleporters.Add(new Teleporter.TeleporterData(wLinks[i].guid, new Vector3(0, 1, wPlaces[i] + LevelBuilder.TILE_SIZE/4f) * LevelBuilder.TILE_SIZE));
		}

		trails = new BitArray(width * height);
		buildingSpaces = new BitArray(width * height);

		PlaceBuildingsAndRoads(exits);

		// add foliage
		int cactiAmount = Random.Range(2, 8);
		for (int i = 0; i < cactiAmount; i++) {
			int tile = RandomUnoccupiedTileInt();
			cacti[tile] = new Cactus.CactusSaveData(tile);
		}

		// temp NPC spawning
		int pplAmount = Random.Range(1, 5);
		List<System.Guid> spawnedChars = new List<System.Guid>();
		for (int i = 0; i < pplAmount; i++) {
			NPC.NPCData npc = new NPC.NPCData(NPC.NPCType.NORMIE, guid, Random.Range(0, 2) == 0);
			npc.position = new SerializableVector3(RandomUnoccupiedTile());
			SaveGame.currentGame.savedCharacters[npc.guid] = npc;
			spawnedChars.Add(npc.guid);

			// test schedule: follow for 5 minutes, chill for 5, repeat
			Schedule schedule = new Schedule()
				.AddBlock(5 * WorldTime.MINUTE, new NPCFollowTask(10f))
				.AddBlock(5 * WorldTime.MINUTE);

			npc.taskSources.Add(schedule);
		}

		// temp horse spawning
		int horseAmount = pplAmount + Random.Range(1, 3);
		for (int i = 0; i < horseAmount; i++) {
			Horse.HorseSaveData hsd = new Horse.HorseSaveData(LevelBuilder.instance.horsePrefab, i < pplAmount ? spawnedChars[i] : System.Guid.Empty);
			hsd.location = new SerializableVector3(RandomUnoccupiedTile());
			SaveGame.currentGame.horses[hsd.guid] = hsd;
			horses.Add(hsd.guid);
		}
	}

	private List<int> Subset(List<int> lst, int size) {
		List<int> newList = lst.ToList();
		while (newList.Count > size) {
			newList.RemoveAt(Random.Range(0, newList.Count));
		}
		return newList;
	}

	public override GameObject PrefabAt(int x, int y) {
		int val = Val(x, y);
		if (trails.Get(val)) {
			return LevelBuilder.instance.trailPrefab;
		} else {
			return LevelBuilder.instance.floorPrefab;
		}
	}

	public override bool TileOccupied(int x, int y) {
		if (x < 0 || x >= width || y < 0 || y >= height)
			return true;
		int val = Val(x, y);
		return buildingSpaces.Get(val) || trails.Get(val) || cacti.ContainsKey(val);
	}

	public List<int> BestPathFrom(int start, int end) {
		Dictionary<int, float> dist = new Dictionary<int, float>();
		for (int i = 0; i < width * height; i++)
			dist.Add(i, float.MaxValue);
		dist[start] = 0;

		Dictionary<int, int> prev = new Dictionary<int, int>();
		for (int i = 0; i < width * height; i++)
			prev.Add(i, -1);
		
		List<int> q = Enumerable.Range(0, height * width).ToList();

		while (q.Count > 0) {
			int u = q.OrderBy(x => dist[x]).First();
			if (u == end) {
				List<int> path = new List<int>();
				path.Add(u);
				while (prev[path[0]] != -1) {  // backtrack
					path.Insert(0, prev[path[0]]);
				}
				return path;
			}
			q.Remove(u);

			foreach (int v in TileNeighbors(u)) {
				float alt = dist[u];
				int valX = X(v);
				int valY = Y(v);
				if (buildingSpaces.Get(v)) {
					continue;
				} else if (valX == 0 || valX == width - 1 || valY == 0 || valY == height - 1 || AdjacentToBuilding(v)) {  // don't travel right on edges
					alt += 2;
				} else if (trails.Get(v)) {
					alt += .1f;
				} else {
					alt += Random.Range(.5f, 1f);
				}
				if (alt < dist[v]) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}
		return null;
	}
	private List<int> TileNeighbors(int tile) {
		List<int> lst = new List<int>();
		if (tile >= width)               // not on the bottom row, so add tile below
			lst.Add(tile - width);
		if (tile < width * (height - 1)) // not on top row, so add tile above
			lst.Add(tile + width);
		if (tile % width != 0)           // not on left column, so add tile to the left
			lst.Add(tile - 1);
		if (tile % width != width - 1)           // not on right column, so add tile to the right
			lst.Add(tile + 1);
		return lst;
	}
	private bool AdjacentToBuilding(int tile) {
		List<int> ns = TileNeighbors(tile);
		foreach (int n in ns) {
			if (buildingSpaces.Get(n))
				return true;
		}
		return false;
	}

	// ================= BUILDING STUFF ================= //

	private void PlaceBuildingsAndRoads(List<int> exits) {
		// Place roads from all teleporters to first building
		int buildingsToAttempt = Random.Range(5, 10);
		Building nextBuilding = new Building(GetInterior());
		for (int i = 0; i < buildingsToAttempt; i++) {
			int destination = TryPlaceBuilding(nextBuilding);
			if (destination == -1)
				continue;
			foreach (int exit in exits) {
				foreach (int path in BestPathFrom(exit, destination)) {
					trails.Set(path, true);
				}
				// TODO: make roads form loops
			}
			teleporters.AddRange(nextBuilding.doors);
			nextBuilding = new Building(GetInterior());
		}
	}

	private InteriorLocation GetInterior() {
		Room room1 = new Room('a', '/',
			"      #////#",
			"      #////#",
			"//####/////#",
			"///////#####"
		);

		Room room2 = new Room('b', '/',
			"///////////#",
			"///////////#",
			"///////////#",
			"############"
		);

		return new InteriorBuilder(room1).Attach("##", room2).Build(parent, guid);
	}

	private int TryPlaceBuilding(Building b) {
		b.Rotate(Random.Range(0, 4));

		for (int i = 0; i < 3; i++) {
			int place = RandomBuildingPos(b);
			if (place == -1)
				continue;  // rotate and try again
			
			// mark spaces as occupied by building
			int x = X(place);
			int y = Y(place);
			for (int xi = x; xi < x + b.width; xi++) {
				for (int yi = y; yi < y + b.height; yi++) {
					buildingSpaces.Set(Val(xi, yi), true);
				}
			}

			b.bottomLeftTile = place;
			buildings.Add(b);
			return Val(x + b.doorOffsetX, y + b.doorOffsetY);
		}
		return -1;
	}

	// returns Val(x, y) where (x, y) is the bottom left corner
	// if a spot cannot easily be found, returns -1
	private int RandomBuildingPos(Building b) {
		int padding = 3;  // amount of spaces from edge to building
		for (int i = 0; i < 20; i++) {
			int paddedW = b.width + 2;
			int paddedH = b.height + 2;
			int x = Random.Range(padding, width - b.width + 1 - padding);
			int y = Random.Range(padding, height - b.height + 1 - padding);
			bool obstructed = false;
			for (int xi = x; xi < x + paddedW && !obstructed; xi++) {
				for (int yi = y; yi < y + paddedH && !obstructed; yi++) {
					obstructed = TileOccupied(xi, yi);
				}
			}
			if (!obstructed && !TileOccupied(x + b.doorOffsetX, y + b.doorOffsetY)) {
				return Val(x+1, y+1);
			}
		}
		return -1;
	}
}