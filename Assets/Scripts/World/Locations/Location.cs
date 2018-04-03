using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using World;

[System.Serializable]
public abstract class Location {

	// YOU WILL NEED TO OVERRIDE SOME FIELDS IN HERE IF EXTENDING LOCATION

	public bool onMap = true;
	public bool discovered;  // discovered only matters for onMap and the future purpose of fast travel
	public string name = "DEFAULT_NAME";
	public abstract string greeting { get; }
	public string icon;  // bull head in pixel western font
	public Map parent;
	public System.Guid town;
	public System.Guid guid = System.Guid.NewGuid();
	public List<System.Guid> connections = new List<System.Guid>();

	public List<System.Guid> characters {
		get {
			return SaveGame.currentGame.savedCharacters.Values
				.Where(x => x.location == guid)
				.Select(x => x.guid)
				.ToList();
		}
	}
	public List<System.Guid> horses = new List<System.Guid>();
	public List<Teleporter.TeleporterData> teleporters = new List<Teleporter.TeleporterData>();
	public List<Building> buildings = new List<Building>();
	public Grid<List<TileElement>> tiles;
	public SerializableVector3 worldLocation;

	public int biomeColor = LevelBuilder.instance == null ? 0 : Random.Range(0, LevelBuilder.instance.biomeColors.Length);
	public int width {
		get { return tiles.width; }	
	}
	public int height {
		get { return tiles.height; }
	}

	public Location(
		Map parent, 
		bool onMap
	) {
		this.parent = parent;
		this.onMap = onMap;
	}

	public float DistanceFrom(Location l) {
		return (l.worldLocation.val - worldLocation.val).magnitude;
	}

	public bool TileOccupied(int x, int y) {
		List<TileElement> stack = tiles.Get(x, y);
		return stack == null || stack.Count == 0 || stack.Any(el => el.occupied);
	}

	public Vector3 TileVectorPosition(int x, int y, bool center = true) {
		return new Vector3(x * LevelBuilder.TILE_SIZE, 0, y * LevelBuilder.TILE_SIZE)
				+ (center ? LevelBuilder.TILE_SIZE : 0) * new Vector3(.5f, 0, .5f);
	}

	// Returns the X/Y coords in the grid of an unoccupied tile
	public Vector2 RandomUnoccupiedXY() {
		List<Vector2> possibilities = new List<Vector2>();
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				possibilities.Add(new Vector2(x, y));
			}
		}
		return possibilities[Random.Range(0, possibilities.Count)];
	}

	// Returns the world position of the tile
	public Vector3 RandomUnoccupiedTile() {
		Vector2 xy = RandomUnoccupiedXY();
		return TileVectorPosition((int)xy.x, (int)xy.y);
	}

	public List<World.TileElement> TileElementsAt(int x, int y) {
        List<TileElement> t = tiles.Get(x, y);
        return t == null ? new List<World.TileElement>() : t;
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