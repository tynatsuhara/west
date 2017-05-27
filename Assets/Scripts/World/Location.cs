using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Location {
	public Map parent;
	public System.Guid guid = System.Guid.NewGuid();
	public string name = NameGen.townName.Generate("<name>");
	public string icon;  // bull head in pixel western font
	public SerializableVector3 worldLocation;
	public System.Guid[] connections = new System.Guid[Random.Range(1, 6)];
	public List<System.Guid> horses = new List<System.Guid>();
	public List<System.Guid> characters = new List<System.Guid>();
	public Dictionary<int, Cactus.CactusSaveData> cacti = new Dictionary<int, Cactus.CactusSaveData>();  // maps tile to cactus
	public List<Teleporter.TeleporterData> teleporters = new List<Teleporter.TeleporterData>();
	public int biomeColor = Random.Range(0, LevelBuilder.instance.biomeColors.Length);
	public int width = 20;  // w and h might be changed later by Generate()!
	public int height = 10;
	private BitArray trails;
	private BitArray buildings;

	public Location(Map parent, float x, float y) {
		var icons = new string[]{"{", "}", "[", "]", "> <", "*", "@", ">", "<"};
		icon = icons[Random.Range(0, icons.Length)];
		this.parent = parent;
		this.worldLocation = new SerializableVector3(new Vector3(x, y, 0));
	}

	// Save things local to this location (ie updates to environment)
	public void Save() {
		
	}

	public bool DoneConnecting() {
		return !connections.Contains(System.Guid.Empty);
	}

	public bool CanConnectTo(Location l) {
		return connections.Contains(System.Guid.Empty) && l.connections.Contains(System.Guid.Empty);
	}

	public void Connect(Location l) {
		int index = System.Array.IndexOf(connections, System.Guid.Empty);
		connections[index] = l.guid;
	}

	public float DistanceFrom(Location l) {
		return (l.worldLocation.val - worldLocation.val).magnitude;
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
		buildings = new BitArray(width * height);

		// Place roads from all teleporters to first building
		int firstDestination = Val(width/2, height/2);
		buildings.Set(firstDestination, true);
		foreach (int exit in exits) {
			foreach (int path in BestPathFrom(exit, firstDestination)) {
				trails.Set(path, true);
			}
			// TODO: make roads form loops
		}

		// add foliage
		int cactiAmount = Random.Range(2, 8);
		for (int i = 0; i < cactiAmount; i++) {
			cacti[RandomUnoccupiedTile()] = new Cactus.CactusSaveData();
		}

		// temp horse spawning
		int horseAmount = Random.Range(1, 5);
		for (int i = 0; i < horseAmount; i++) {
			int tile = RandomUnoccupiedTile();
			Horse.HorseSaveData hsd = new Horse.HorseSaveData(LevelBuilder.instance.horsePrefab);
			hsd.location = new SerializableVector3(TileVectorPosition(tile));
			SaveGame.currentGame.horses.Add(hsd.guid, hsd);
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

	public GameObject TileAt(int x, int y) {
		int val = Val(x, y);
		if (buildings.Get(val)) {
			return LevelBuilder.instance.buildingPrefabPlaceholder;
		} else if (trails.Get(val)) {
			return LevelBuilder.instance.trailPrefab;
		} else {
			return LevelBuilder.instance.floorPrefab;
		}
	}
	// returns the center vector pos of a tile with y = 0
	public Vector3 TileVectorPosition(int val) {
		float xPos = X(val) * LevelBuilder.TILE_SIZE + LevelBuilder.TILE_SIZE/2f;
		float zPos = Y(val) * LevelBuilder.TILE_SIZE + LevelBuilder.TILE_SIZE/2f;
		return new Vector3(xPos, 0, zPos);
	}
	public int RandomUnoccupiedTile() {
		List<int> all = Enumerable.Range(0, width * height).Where(x => !TileOccupied(x)).ToList();
		return all[Random.Range(0, all.Count)];
	}

	private bool TileOccupied(int val) {
		return buildings.Get(val) || trails.Get(val) || cacti.ContainsKey(val);
	}

	private int Val(int x, int y) {
		return x + y * width;
	}
	private int X(int val) {
		return val % width;
	}
	private int Y(int val) {
		return val / width;
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
				if (valX == 0 || valX == width - 1 || valY == 0 || valY == height - 1) {  // don't travel right on edges
					alt += 2;
				} else if (trails.Get(v)) {
					alt += .1f;
				} else {
					alt += 1;
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

	// ================= BUILDING STUFF =============g==== //

	// returns Val(x, y) where (x, y) is the bottom left corner
	// if a spot cannot easily be found, returns -1
	private int randomBuildingPos(int w, int h) {
		for (int i = 0; i < 10; i++) {
			int x = Random.Range(0, width - w + 1);
			int y = Random.Range(0, height - h + 1);
			bool obstructed = false;
			for (int xi = x; xi < x + w && !obstructed; xi++) {
				for (int yi = y; yi < y + h && !obstructed; yi++) {
					obstructed = TileOccupied(Val(xi, yi));
				}
			}
		}
		return -1;
	}
}