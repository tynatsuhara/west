using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Location {
	public Map parent;
	public System.Guid guid = System.Guid.NewGuid();
	public string name = NameGen.townName.Generate("<name>");
	public SerializableVector3 worldLocation;
	public System.Guid[] connections = new System.Guid[Random.Range(1, 6)];
	public List<System.Guid> horses = new List<System.Guid>();
	public List<Teleporter.TeleporterData> teleporters = new List<Teleporter.TeleporterData>();
	public int width = 20;
	public int height = 10;
	private List<int> trails = new List<int>();

	public Location(Map parent, float x, float y) {
		this.parent = parent;
		this.worldLocation = new SerializableVector3(new Vector3(x, y, 0));

		// TEMP spawning
		int horseAmount = Random.Range(1, 5);
		for (int i = 0; i < horseAmount; i++) {
			Horse.HorseSaveData hsd = new Horse.HorseSaveData(LevelBuilder.instance.horsePrefab);
			SaveGame.currentGame.horses.Add(hsd.guid, hsd);
			horses.Add(hsd.guid);
		}
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

		teleporters.Add(new Teleporter.TeleporterData(l.guid, new Vector3(index * 4, 1, -5)));
	}

	public float DistanceFrom(Location l) {
		return (l.worldLocation.val - worldLocation.val).magnitude;
	}

	// Build the street layout for a town
	public void GenerateLayout() {
		connections = connections.Where(x => x != System.Guid.Empty).ToArray();
		List<Location> links = connections.Select(x => parent.locations[x]).ToList();
		
		List<Location> nLinks = links.Where(l => l.worldLocation.y > worldLocation.y && 
			Mathf.Abs((l.worldLocation.val - worldLocation.val).x) < (Mathf.Abs((l.worldLocation.val - worldLocation.val).y))).ToList();
		List<Location> sLinks = links.Where(l => l.worldLocation.y < worldLocation.y && 
			Mathf.Abs((l.worldLocation.val - worldLocation.val).x) < (Mathf.Abs((l.worldLocation.val - worldLocation.val).y))).ToList();
		List<Location> eLinks = links.Where(l => l.worldLocation.x > worldLocation.x && 
			Mathf.Abs((l.worldLocation.val - worldLocation.val).y) < (Mathf.Abs((l.worldLocation.val - worldLocation.val).x))).ToList();
		List<Location> wLinks = links.Where(l => l.worldLocation.x < worldLocation.x && 
			Mathf.Abs((l.worldLocation.val - worldLocation.val).y) < (Mathf.Abs((l.worldLocation.val - worldLocation.val).x))).ToList();

		int minRoadsNS = Mathf.Max(nLinks.Count, sLinks.Count);
		int minRoadsEW = Mathf.Max(eLinks.Count, wLinks.Count);
	}

	public GameObject TileAt(int x, int y) {
		int val = Val(x, y);
		if (trails.Contains(val)) {
			return LevelBuilder.instance.trailPrefab;
		} else {
			return LevelBuilder.instance.floorPrefab;
		}
	}

	private int Val(int x, int y) {
		return x * width + y;
	}
}