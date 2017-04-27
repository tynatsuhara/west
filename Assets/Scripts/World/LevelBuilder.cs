using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelBuilder : MonoBehaviour {

	public static LevelBuilder instance;

	public GameObject floorPrefab;
	public GameObject trailPrefab;
	public GameObject wallPrefab;
	public GameObject wallCornerPrefab;
	public GameObject doorPrefab;
	public GameObject horsePrefab;
	public GameObject teleporterPrefab;
	public List<GameObject> recycle;  // everything that is spawned in the world

	private PicaVoxel.Volume[,] floorTiles;
	private Location loadedLocation;

	public static int TILE_SIZE = 2;  // in-game units

	public void Awake() {
		instance = this;
	}

	public void LoadLocation(System.Guid guid) {
		bool firstLoad = GameManager.instance.loadReposition == Vector3.zero;

		if (!firstLoad) {
			foreach (GameObject go in recycle) {
				if (go != null) {
					Destroy(go);
				}
			}
			foreach (PlayerControls pc in GameManager.players) {
				pc.transform.root.position = GameManager.instance.loadReposition;
			}
		}

		Location l = Map.Location(guid);
		loadedLocation = l;
		floorTiles = new PicaVoxel.Volume[l.width, l.height];
		SpawnHorses(l, firstLoad);
		SpawnTeleporters(l);
		SaveGame.currentGame.quests.UpdateQuests();  // mark quests at teleporters		
		GameUI.instance.topCenterText.Say(l.name + ", population " + l.characters.Count, color: "grey");		

		GameObject floorHolder = new GameObject();
		floorHolder.name = "Ground";
		for (int i = 0; i < l.width; i++) {
			for (var j = 0; j < l.height; j++) {
				GameObject tile = Instantiate(l.TileAt(i, j), new Vector3(i * TILE_SIZE, -.2f, j * TILE_SIZE), 
											  Quaternion.identity) as GameObject;
				tile.transform.parent = floorHolder.transform;
				floorTiles[i, j] = tile.GetComponent<PicaVoxel.Volume>();
			}
		}
		recycle.Add(floorHolder);
		PositionWalls(l);
	}

	public PicaVoxel.Volume FloorTileAt(Vector3 pos) {
		int x = (int)(pos.x / TILE_SIZE);
		int z = (int)(pos.z /  TILE_SIZE);
		if (x >= loadedLocation.width || z >= loadedLocation.height || x < 0 || z < 0)
			return null;
		return floorTiles[x, z];
	}

	public Floor FloorAt(Vector3 pos) {
		PicaVoxel.Volume f = FloorTileAt(pos);
		return f == null ? null : f.GetComponent<Floor>();
	}


	private void SpawnHorses(Location l, bool spawnRiddenByPlayers) {
		Debug.Log("should spawn player's horses? " + spawnRiddenByPlayers);
		List<System.Guid> spawnExceptions = spawnRiddenByPlayers
				? new List<System.Guid>()
				: SaveGame.currentGame.savedPlayers.Select(x => x.mountGuid).Where(x => x != System.Guid.Empty).ToList();
		foreach (System.Guid id in l.horses) {
			if (!spawnExceptions.Contains(id)) {
				Horse.HorseSaveData hsd = SaveGame.currentGame.horses[id];
				Horse h = Instantiate(horsePrefab).GetComponent<Horse>();
				h.LoadSaveData(hsd);
				recycle.Add(h.gameObject);
			}
		}
	}

	private void SpawnTeleporters(Location l) {
		GameObject porterParent = new GameObject();
		porterParent.name = "Teleporters";
		foreach (Teleporter.TeleporterData td in l.teleporters) {
			GameObject porter = Instantiate(teleporterPrefab);
			porter.name = "-> " + SaveGame.currentGame.map.locations[td.toId].name;
			porter.transform.parent = porterParent.transform;
			porter.GetComponent<Teleporter>().LoadSaveData(td);
			recycle.Add(porter);			
		}
	}

	private void PositionWalls(Location l) {
		Transform walls = GameObject.Find("Walls").transform;
		// bottom
		walls.GetChild(0).localScale = new Vector3(l.width * TILE_SIZE + 2, 10, 1);
		walls.GetChild(0).position = new Vector3(l.width * TILE_SIZE/2f, 1, -.5f);
		// top
		walls.GetChild(1).localScale = new Vector3(l.width * TILE_SIZE + 2, 10, 1);
		walls.GetChild(1).position = new Vector3(l.width * TILE_SIZE/2f, 1, l.height * TILE_SIZE + .5f);
		// right
		walls.GetChild(2).localScale = new Vector3(1, 10, l.height * TILE_SIZE + 2);
		walls.GetChild(2).position = new Vector3(l.width * TILE_SIZE + .5f, 1, l.height * TILE_SIZE/2f);
		// left
		walls.GetChild(3).localScale = new Vector3(1, 10, l.height * TILE_SIZE + 2);
		walls.GetChild(3).position = new Vector3(-.5f, 1, l.height * TILE_SIZE/2f);
	}
}
