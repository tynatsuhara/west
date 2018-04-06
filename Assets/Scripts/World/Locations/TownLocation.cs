using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using World;

// TODO: Move stuff into TownBuilder, make this just store data

[System.Serializable]
public class TownLocation : Location {

	public override string greeting {
		get { 
			int bounty = SaveGame.currentGame.savedPlayers
					.Select(x => SaveGame.currentGame.crime.CharacterLocationBounty(x.guid, guid))
					.Aggregate((x, y) => x + y);
			string gr = name + ", population " + characters.Count;
			return bounty > 0 ? gr + "\nbounty $" + bounty : gr;
		}
	}

	public TownLocation(
		Map parent,
		float x, 
		float y
	) : base(parent, true) {
		var icons = new string[]{"{", "}", "[", "]", "> <", "*", "@", ">", "<"};
		icon = icons[Random.Range(0, icons.Length)];
		this.worldLocation = new SerializableVector3(new Vector3(x, y, 0));
		name = NameGen.townName.Generate("<name>");
		int townConnectionAmount = Random.Range(1, 6);
		for (int i = 0; i < townConnectionAmount; i++) {
			connections.Add(System.Guid.Empty);
		}
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
		connections[connections.IndexOf(System.Guid.Empty)] = l.guid;
	}

	// Build the street layout for a town
	public void Generate() {
		// default minimum heights
		int width = 30;
		int height = width;

		connections = connections.Where(x => x != System.Guid.Empty).ToList();
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

		// initialize the tile element grid
		// guarantee that the first element in each stack is the ground tile
		tiles = new Grid<List<TileElement>>(width, height, () => {
			List<TileElement> lst = new List<TileElement>();
			lst.Add(new GroundTile());
			return lst;
		});

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

		PlaceBuildingsAndRoads(exits);
		// TODO: make trails "occupied" after placing buildings to prevent cacti etc on the trails

		// add foliage
		int cactiAmount = Random.Range(2, 8);
		for (int i = 0; i < cactiAmount; i++) {
			Vector2 xy = RandomUnoccupiedXY();
			tiles.Get((int)xy.x, (int)xy.y).Add(new Cactus());
		}

		// temp NPC spawning
		int pplAmount = Random.Range(1, 5);
		List<System.Guid> spawnedChars = new List<System.Guid>();
		for (int i = 0; i < pplAmount; i++) {
			Location work = Map.Location(buildings[0].guid);
			work.name = "WORK";
			Location home = Map.Location(buildings[1].guid);
			home.name = "HOME";
			NPCData npc = new NPCFactory().MakeNormie(work, home);
			SaveGame.currentGame.savedCharacters[npc.guid] = npc;
			npc.position = new SerializableVector3(RandomUnoccupiedTile());
			npc.questsToGive.Add(new KillQuest(npc.guid));
			npc.location = guid;
			spawnedChars.Add(npc.guid);
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

	// ================= PATHFINDING STUFF ================= //

	// Djikstra's
	private List<int> BestPathFrom(int start, int end) {
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
				GroundTile ground = GroundTileAt(v);
				if (ground.occupied) {
					continue;
				} else if (valX == 0 || valX == width - 1 || valY == 0 || valY == height - 1 || AdjacentToBuilding(v)) {  // don't travel right on edges
					alt += 2;
				} else if (ground.type == GroundTile.GroundType.TRAIL) {
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

	// A*
	private List<int> BestPathFrom2(int start, int end) {
		HashSet<int> closedSet = new HashSet<int>();
		HashSet<int> openSet = new HashSet<int>();

		openSet.Add(start);
		Dictionary<int, int> cameFrom = new Dictionary<int, int>();
		Dictionary<int, float> gScore = Enumerable.Range(0, width * height)
				.ToDictionary(x => x, x => float.MaxValue);
		gScore[start] = 0;
		Dictionary<int, float> fScore = Enumerable.Range(0, width * height)
				.ToDictionary(x => x, x => float.MaxValue);
		fScore[start] = HeuristicCostEstimate(start, end);
		
		while (openSet.Count > 0) {
			int current = openSet.OrderBy(x => fScore[x]).First();
			if (current == end) {
				List<int> result = new List<int>();
				result.Add(current);
				while (cameFrom.Keys.Contains(current)) {
					current = cameFrom[current];
					result.Insert(0, current);
				}
				return result;
			}
			openSet.Remove(current);
			closedSet.Add(current);
			foreach (int neighbor in TileNeighbors(current)) {
				if (closedSet.Contains(neighbor))
					continue;
				if (!openSet.Contains(neighbor))
					openSet.Add(neighbor);

				GroundTile ground = GroundTileAt(neighbor);
				int valX = X(neighbor);
				int valY = Y(neighbor);
				float gScoreTentative = gScore[current];

				if (ground.occupied) {
					openSet.Remove(neighbor);
					closedSet.Add(neighbor);
					continue;
				} else if (valX == 0 || valX == width - 1 || valY == 0 || valY == height - 1 || AdjacentToBuilding(current)) {  // don't travel right on edges
					gScoreTentative += 2f;
				} else if (ground.type == GroundTile.GroundType.TRAIL) {
					gScoreTentative += .1f;
				} else {
					gScoreTentative += Random.Range(.5f, 1f);
				}

				if (gScoreTentative >= gScore[neighbor])
					continue;

				cameFrom[neighbor] = current;
				gScore[neighbor] = gScoreTentative;
				fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, end);
			}
		}

		return null;
	}

	private float HeuristicCostEstimate(int start, int end) {
		int startX = X(start);
		int startY = Y(start);
		int endX = X(end);
		int endY = Y(end);
		float diffX = startX - endX;
		float diffY = startY - endY;
		return Mathf.Sqrt(diffX * diffX + diffY * diffY)/2;
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
			if (GroundTileAt(tile).occupied)
				return true;
		}
		return false;
	}

	// ================= BUILDING STUFF ================= //
	
	private void PlaceBuildingsAndRoads(List<int> exits) {		
		// Place roads from all teleporters to first building
		int buildingsToAttempt = Random.Range(5, 10);
		InteriorLocation interior = GetInterior();
		for (int i = 0; i < buildingsToAttempt; i++) {
			Building building = new Building(interior);
			int destination = TryPlaceBuilding(building);
			if (destination == -1)
				continue;

			parent.locations[interior.guid] = interior;
			connections.Add(interior.guid);
			interior.PlaceAt(guid);

			// TODO: different buildings can have multiple entrances 
			// that can attach or not attach to roads 
			GroundTile gt = GroundTileAt(destination);
			gt.occupied = false;
			foreach (int exit in exits) {
				foreach (int path in BestPathFrom2(exit, destination)) {
					GroundTileAt(path).type = GroundTile.GroundType.TRAIL;
				}
				// TODO: make roads form loops
			}
			gt.occupied = true;

			teleporters.AddRange(building.doors);
			interior = GetInterior();
		}
	}

	private GroundTile GroundTileAt(int val) {
		return tiles.Get(X(val), Y(val)).First() as GroundTile;
	}

	// todo: all this should be moved to BuildingFactory or something
	private InteriorLocation GetInterior() {
		Room room1 = new Room('a', '/',
			"      #//T/#",
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
		
		return new InteriorBuilder(room1)
				.Attach("##", room2)
				.AddTeleporter('T', guid, "front door")
				.Build(parent, guid, "SOME BUILDING");
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
					GroundTileAt(Val(xi, yi)).occupied = true;
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
		int edgePadding = 3;  // amount of spaces from edge to building
		int buildingPadding = 2;
		int paddedW = b.width + buildingPadding;
		int paddedH = b.height + buildingPadding;
		int attempts = 20;

		for (int i = 0; i < attempts; i++) {
			int x = Random.Range(edgePadding, width - b.width + 1 - edgePadding);
			int y = Random.Range(edgePadding, height - b.height + 1 - edgePadding);

			// check corners first
			bool obstructed = TileOccupied(x + paddedW - 1, y) || 
							  TileOccupied(x, y + paddedH - 1) ||
			                  TileOccupied(x + paddedW - 1, y + paddedH - 1);

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