using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public abstract class Location {
	public string name = "DEFAULT_NAME";
	public string icon;  // bull head in pixel western font
	public Map parent;
	public System.Guid town;
	public System.Guid guid = System.Guid.NewGuid();

	public List<System.Guid> characters = new List<System.Guid>();
	public List<System.Guid> horses = new List<System.Guid>();
	public List<Teleporter.TeleporterData> teleporters = new List<Teleporter.TeleporterData>();
	public Dictionary<int, Cactus.CactusSaveData> cacti = new Dictionary<int, Cactus.CactusSaveData>();  // maps tile to cactus
	public Dictionary<int, Headstone.HeadstoneSaveData> headstones = new Dictionary<int, Headstone.HeadstoneSaveData>();  // tile to headstone frame
	public List<Building> buildings = new List<Building>();
	public SerializableVector3 worldLocation;

	public int biomeColor = Random.Range(0, LevelBuilder.instance.biomeColors.Length);
	public int width = 20;  // minimums, w and h might be changed later by Generate()!
	public int height = 20;

	public Location(Map parent) {
		this.parent = parent;
	}

	// Save things local to this location (ie updates to environment)
	public void Save() {
		if (Map.CurrentLocation() == this) {
			foreach (Cactus c in Object.FindObjectsOfType<Cactus>()) {
				cacti[c.SaveData().tile] = c.SaveData();
			}
			foreach (Headstone h in Object.FindObjectsOfType<Headstone>()) {
				headstones[h.SaveData().tile] = h.SaveData();
			}
		}
	}

	public float DistanceFrom(Location l) {
		return (l.worldLocation.val - worldLocation.val).magnitude;
	}

	public abstract GameObject PrefabAt(int x, int y);
	public abstract bool TileOccupied(int x, int y);

	protected List<int> Subset(List<int> lst, int size) {
		List<int> newList = lst.ToList();
		while (newList.Count > size) {
			newList.RemoveAt(Random.Range(0, newList.Count));
		}
		return newList;
	}

	public Vector3 TileVectorPosition(int val, bool center = true) {
		float xPos = X(val) * LevelBuilder.TILE_SIZE;
		float zPos = Y(val) * LevelBuilder.TILE_SIZE;
		return new Vector3(xPos, 0, zPos) + (center ? new Vector3(LevelBuilder.TILE_SIZE/2f, 0, LevelBuilder.TILE_SIZE/2f) : Vector3.zero);
	}
	public int RandomUnoccupiedTile() {
		List<int> all = Enumerable.Range(0, width * height).Where(val => !TileOccupied(X(val), Y(val))).ToList();
		return all[Random.Range(0, all.Count)];
	}

	public int Val(int x, int y) {
		return x + y * width;
	}
	public int X(int val) {
		return val % width;
	}
	public int Y(int val) {
		return val / width;
	}
}