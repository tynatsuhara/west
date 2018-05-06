using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelBuilder : MonoBehaviour {

	public static LevelBuilder instance;
	public Camera townRenderCam;
	public GameObject floorPrefab;
	public GameObject trailPrefab;
	public GameObject wallPrefab;
	public GameObject wallCornerPrefab;
	public GameObject doorPrefab;
	public GameObject cactusPrefab;
	public GameObject headstonePrefab;
	public GameObject tumbleweedPrefab;
	public GameObject horsePrefab;
	public GameObject teleporterPrefab;
	public GameObject destinationMarkerPrefab;
	public Material mat;
	public Color32[] biomeColors;
	public GameObject[] buildingPrefabs;
	public GameObject npcPrefab;

	public List<Transform> permanent;  // everything that shouldn't be deleted for loading a new location

	public List<Teleporter> teleporters;
	public PicaVoxel.Volume[,] floorTiles;
	private Location loadedLocation;

	public static int TILE_SIZE = 2;  // in-game units

	public void Awake() {
		instance = this;
	}

	public void LoadLocation(System.Guid guid, bool firstLoadSinceStartup) {
		StartCoroutine(LoadLocationCoroutine(guid, firstLoadSinceStartup));
	}

	private IEnumerator LoadLocationCoroutine(System.Guid guid, bool firstLoadSinceStartup) {
		markedDestinations = new Dictionary<string, GameObject>();

		if (!firstLoadSinceStartup) {
			Clean();
		} else {
			permanent = Object.FindObjectsOfType<Transform>().Where(x => x.parent == null).ToList();
		}

		SaveGame.currentGame.events.CheckQueue(false);
		loadedLocation = Map.Location(guid);

		mat.SetColor("_Tint", biomeColors[loadedLocation.biomeColor]);
		floorTiles = new PicaVoxel.Volume[loadedLocation.width, loadedLocation.height];
		teleporters = new List<Teleporter>();

		SpawnBuildings();
		SpawnTileElements();
		SpawnAtmospherics();
		SpawnWallsHackily();
		SpawnNPCs();
		SpawnHorses(firstLoadSinceStartup);
		GroupFloor();
		SpawnTeleporters();
		ShowGreeting();
		PositionWalls();

		if (!loadedLocation.discovered && loadedLocation.onMap) {
			townRenderCam.enabled = true;
			yield return new WaitForEndOfFrame();
			RenderTexture rt = townRenderCam.targetTexture;
			Debug.Log("w = " + rt.width + ", h = " + rt.height);
			Texture2D tex = new Texture2D(rt.width, rt.height);
			tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, true);
			tex.Apply();
			loadedLocation.mapRender = tex.GetRawTextureData();
			townRenderCam.enabled = false;
		}

		loadedLocation.discovered = true;
		VisualMap.instance.Refresh();		
	}

	public void Clean(bool removePlayers=false) {
		PicaVoxel.VoxelParticleSystem.Instance.GetComponent<ParticleSystem>().Clear();
		List<Transform> delete = Object.FindObjectsOfType<Transform>().Where(x => x.parent == null && !permanent.Contains(x)).ToList();
		foreach (Transform t in delete) {
			Destroy(t.gameObject);
		}
		foreach (Player pc in GameManager.players) {
			if (removePlayers) {
				Destroy(pc.gameObject);
			} else {
				pc.transform.root.position = GameManager.instance.loadReposition;
			}
		}
	}

	// floor tiles are stored in cache for fast lookup
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

	private void SpawnNPCs() {
		foreach (System.Guid id in loadedLocation.characters) {
			NPC npc = SpawnNPC(id);
			npc.FastForwardPosition();
		}
	}

	public NPC SpawnNPC(System.Guid id) {
		NPCData data = SaveGame.currentGame.savedCharacters[id];
		if (data.departed || GameObject.FindObjectsOfType<NPC>().Where(x => x.guid == data.guid).Count() > 0) {
			return null;
		}
		NPC npc = Instantiate(npcPrefab).GetComponent<NPC>();
		npc.LoadSaveData(data);
		return npc;
	}

	private void SpawnHorses(bool spawnRiddenByPlayers) {
		// we only want to spawn the horses ridden by players when they don't exist
		// because horses transition seamlessly through teleporters
		List<System.Guid> spawnExceptions = spawnRiddenByPlayers
				? new List<System.Guid>()
				: SaveGame.currentGame.savedPlayers.Select(x => x.mountGuid).Where(x => x != System.Guid.Empty).ToList();
		foreach (System.Guid id in loadedLocation.horses) {
			if (!spawnExceptions.Contains(id)) {
				Horse.HorseSaveData hsd = SaveGame.currentGame.horses[id];
				Horse h = Instantiate(horsePrefab).GetComponent<Horse>();
				h.LoadSaveData(hsd);
			}
		}
	}

	private void SpawnAtmospherics() {
		// tumbly weedos
		int tumbleweeds = Random.Range(0, 2);
		for (int i = 0; i < tumbleweeds; i++) {
			Instantiate(tumbleweedPrefab, loadedLocation.RandomUnoccupiedTile(), Quaternion.identity);
		}
	}

	private void SpawnTileElements() {
		loadedLocation.tiles.ForEach((lst, x, y) => lst.ForEach(el => el.Spawn(this, loadedLocation, x, y)));
	}

	// Just to keep our heirarchy cleaner
	private void GroupFloor() {
		Transform floorHolder = new GameObject().transform;
		floorHolder.name = "Ground";
		for (int x = 0; x < loadedLocation.width; x++) {
			for (int y = 0; y < loadedLocation.height; y++) {
				if (floorTiles[x, y] != null) {
					floorTiles[x, y].transform.SetParent(floorHolder);
				}
			}
		}
	}

	// TODO: How do we change this to make walls spawn following the TileElement design pattern?
	private void SpawnWallsHackily() {
		Location l = loadedLocation;
		
		GameObject interiorObjectHolder = new GameObject();
		interiorObjectHolder.name = "Building Walls";

		for (int y = -1; y <= l.height; y++) {
			for (int x = -1; x <= l.width; x++) {
				World.FloorTile sq = l.TileElementsAt(x, y).Where(el => el is World.FloorTile).FirstOrDefault() as World.FloorTile;
				World.FloorTile sqLeft = l.TileElementsAt(x-1, y).Where(el => el is World.FloorTile).FirstOrDefault() as World.FloorTile;
				World.FloorTile sqTop = l.TileElementsAt(x, y+1).Where(el => el is World.FloorTile).FirstOrDefault() as World.FloorTile;
				if ((sq != null && sq.wallLeft) || (sqLeft != null && sqLeft.wallRight)) {
					GameObject wall = Instantiate(wallPrefab, new Vector3(TILE_SIZE * x, .8f, TILE_SIZE * y + 1), Quaternion.identity);
					wall.transform.SetParent(interiorObjectHolder.transform);
				}
				if ((sq != null && sq.wallTop) || (sqTop != null && sqTop.wallBottom)) {
					GameObject wall = Instantiate(wallPrefab, new Vector3(TILE_SIZE * x + 1, .8f, TILE_SIZE * y + 2), Quaternion.Euler(0, 90, 0));
					wall.transform.SetParent(interiorObjectHolder.transform);
				}
			}
		}
	}

	private void SpawnTeleporters() {
		GameObject porterParent = new GameObject();
		porterParent.name = "Teleporters";
		foreach (Teleporter.TeleporterData td in loadedLocation.teleporters /* HACK */ .Where(x => Map.Location(x.toId).onMap)) {
			GameObject porter = Instantiate(teleporterPrefab);
			porter.name = "-> " + SaveGame.currentGame.map.locations[td.toId].name;
			porter.transform.parent = porterParent.transform;
			porter.transform.position = td.position.val;
			porter.GetComponent<Teleporter>().LoadSaveData(td, loadedLocation.guid);
			teleporters.Add(porter.GetComponent<Teleporter>());
		}
	}

	// Called by quest manager
	public Dictionary<string, GameObject> markedDestinations;
	public void MarkQuestDestinations(List<Task.TaskDestination> destinations) {
		// Quest teleporter destinations
		List<System.Guid> questTeleportDestinations = destinations
				.Where(x => loadedLocation.guid != x.location)
				.Select(x => SaveGame.currentGame.map.BestPathFrom(loadedLocation.guid, x.location)[0])
				.ToList();
		foreach (Teleporter t in teleporters) {
			t.MarkQuest(questTeleportDestinations.Contains(t.toId));
		}

		Dictionary<System.Guid, Task.TaskDestination> markedCharacters = destinations
				.Where(x => x.location == loadedLocation.guid && x.character != System.Guid.Empty)
				.ToDictionary(x => x.character, x => x);
		foreach (NPC npc in GameManager.spawnedNPCs) {
			if (markedCharacters.ContainsKey(npc.guid)) {
				npc.characterIndicator.MarkForQuest(markedCharacters[npc.guid]);
			} else {
				npc.characterIndicator.UnmarkForQuest();
			}
		}

		// Quests in this location, spawn markers at spots
		List<Vector3> destinationMarkers = destinations
				.Where(x => loadedLocation.guid == x.location && x.character == System.Guid.Empty)
				.Select(x => x.position)
				.ToList();
		List<string> destStrings = destinationMarkers.Select(x => x.ToString()).ToList();
		List<Vector3> newDestinationMarkers = destinationMarkers
				.Where(x => !markedDestinations.ContainsKey(x.ToString()))
				.ToList();
		List<Vector3> expiredDestinationMarkers = markedDestinations.Values
				.Where(x => x != null && !destStrings.Contains(x.transform.position.ToString()))
				.Select(x => x.transform.position)
				.ToList();
		
		GameObject spotParent = GameObject.Find("QuestPositionParent");
		if (spotParent == null) {
			spotParent = new GameObject();
			spotParent.name = "QuestPositionParent";
		}
		foreach (Vector3 expired in expiredDestinationMarkers) {
			Destroy(markedDestinations[expired.ToString()]);
			markedDestinations.Remove(expired.ToString());
		}
		foreach (Vector3 v in newDestinationMarkers) {
			GameObject spot = Instantiate(destinationMarkerPrefab, v, Quaternion.identity);
			spot.transform.SetParent(spotParent.transform);
			markedDestinations.Add(v.ToString(), spot);
			spot.name = v.ToString();
		}
	}

	private void SpawnBuildings() {
		foreach (Building b in loadedLocation.buildings) {
			int t = b.bottomLeftTile;
			int x = loadedLocation.X(b.bottomLeftTile);
			int y = loadedLocation.Y(b.bottomLeftTile);
			GameObject spawnedBuilding = Instantiate(buildingPrefabs[b.prefabIndex],
						loadedLocation.TileVectorPosition(x, y, false) + new Vector3(b.width/2f, 0, b.height/2f) * TILE_SIZE, 
						Quaternion.Euler(0, b.angle, 0));
			Teleporter porter = spawnedBuilding.GetComponentInChildren<Teleporter>();
			b.doors[0].position = new SerializableVector3(porter.transform.position);
			porter.LoadSaveData(b.doors[0], loadedLocation.guid);
			teleporters.Add(porter);
		}
	}

	private void ShowGreeting() {
		string greeting = loadedLocation.greeting;
		greeting = loadedLocation.discovered ? greeting : "New location discovered\n" + greeting;
		GameUI.instance.topCenterText.Say(greeting, duration:4);
	}

	private void PositionWalls() {
		Transform walls = GameObject.Find("Walls").transform;
		float height = 30;
		// bottom
		walls.GetChild(0).localScale = new Vector3(loadedLocation.width * TILE_SIZE + 2, height, 1);
		walls.GetChild(0).position = new Vector3(loadedLocation.width * TILE_SIZE/2f, 1, -.5f);
		// top
		walls.GetChild(1).localScale = new Vector3(loadedLocation.width * TILE_SIZE + 2, height, 1);
		walls.GetChild(1).position = new Vector3(loadedLocation.width * TILE_SIZE/2f, 1, loadedLocation.height * TILE_SIZE + .5f);
		// right
		walls.GetChild(2).localScale = new Vector3(1, height, loadedLocation.height * TILE_SIZE + 2);
		walls.GetChild(2).position = new Vector3(loadedLocation.width * TILE_SIZE + .5f, 1, loadedLocation.height * TILE_SIZE/2f);
		// left
		walls.GetChild(3).localScale = new Vector3(1, height, loadedLocation.height * TILE_SIZE + 2);
		walls.GetChild(3).position = new Vector3(-.5f, 1, loadedLocation.height * TILE_SIZE/2f);
	}
}
