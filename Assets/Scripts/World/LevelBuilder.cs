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

	private List<Teleporter> teleporters;
	private PicaVoxel.Volume[,] floorTiles;
	private Location loadedLocation;

	public static int TILE_SIZE = 2;  // in-game units

	public void Awake() {
		instance = this;
	}

	public void LoadLocation(System.Guid guid, bool firstLoadSinceStartup) {
		markedDestinations = new Dictionary<string, GameObject>();

		if (!firstLoadSinceStartup) {
			Clean();
		} else {
			permanent = Object.FindObjectsOfType<Transform>().Where(x => x.parent == null).ToList();
		}

		SaveGame.currentGame.events.CheckQueue(false);
		Location l = Map.Location(guid);

		if (l is InteriorLocation) {
			Debug.Log(l);
		}

		loadedLocation = l;
		mat.SetColor("_Tint", biomeColors[l.biomeColor]);
		floorTiles = new PicaVoxel.Volume[l.width, l.height];
		teleporters = new List<Teleporter>();
		SpawnBuildings();
		SpawnNPCs();
		SpawnHorses(firstLoadSinceStartup);
		SpawnAtmospherics();
		SpawnTeleporters();
		string greeting = l.name + ", population " + l.characters.Count;
		int bounty = SaveGame.currentGame.savedPlayers.Select(x => SaveGame.currentGame.crime.Bounty(x.guid, guid)).Aggregate((x, y) => x + y);
		greeting = bounty > 0 ? greeting + "\nbounty $" + bounty : greeting;
		greeting = loadedLocation.discovered ? greeting : "New location discovered\n" + greeting;
		loadedLocation.discovered = true;

		GameUI.instance.topCenterText.Say(greeting, duration:4);

		GameObject floorHolder = new GameObject();
		floorHolder.name = "Ground";
		for (int i = 0; i < l.width; i++) {
			for (int j = 0; j < l.height; j++) {
				GameObject tilePrefab = l.PrefabAt(i, j);
				if (tilePrefab == null)
					continue;
				GameObject tile = Instantiate(tilePrefab, new Vector3(i * TILE_SIZE, -.2f, j * TILE_SIZE), 
											  Quaternion.identity) as GameObject;
				tile.transform.parent = floorHolder.transform;
				floorTiles[i, j] = tile.GetComponent<PicaVoxel.Volume>();
			}
		}
		PositionWalls();
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
			SpawnNPC(id);
		}
	}

	public void SpawnNPC(System.Guid id) {
		NPC.NPCSaveData data = SaveGame.currentGame.savedCharacters[id];
		if (data.departed) {
			return;
		}
		NPC npc = Instantiate(npcPrefab).GetComponent<NPC>();
		npc.LoadSaveData(data);
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
		if (!(loadedLocation is TownLocation))
			return;

		// cacti
		foreach (int tile in loadedLocation.cacti.Keys) {
			GameObject c = Instantiate(cactusPrefab, loadedLocation.TileVectorPosition(tile), Quaternion.identity);
			c.GetComponent<Cactus>().LoadSaveData(loadedLocation.cacti[tile]);
		}

		// tumbly weedos
		int tumbleweeds = Random.Range(0, 2);
		for (int i = 0; i < tumbleweeds; i++) {
			Instantiate(tumbleweedPrefab, loadedLocation.RandomUnoccupiedTile(), Quaternion.identity);
		}

		// headstones
		foreach (int tile in loadedLocation.headstones.Keys) {
			GameObject h = Instantiate(headstonePrefab, loadedLocation.TileVectorPosition(tile), Quaternion.identity);
			h.GetComponent<Headstone>().LoadSaveData(loadedLocation.headstones[tile]);
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
	private Dictionary<string, GameObject> markedDestinations;
	public void MarkQuestDestinations(List<Task.TaskDestination> destinations) {
		// Quest teleporter destinations		
		List<System.Guid> questTeleportDestinations = destinations
				.Where(x => loadedLocation.guid != x.location)
				.Select(x => SaveGame.currentGame.map.BestPathFrom(loadedLocation.guid, x.location)[0])
				.ToList();
		foreach (Teleporter t in teleporters) {
			t.MarkQuest(questTeleportDestinations.Contains(t.toId));
		}

		List<System.Guid> markedCharacters = destinations
				.Where(x => x.location == loadedLocation.guid && x.character != System.Guid.Empty)
				.Select(x => x.character)
				.ToList();
		foreach (NPC npc in GameManager.spawnedNPCs) {
			if (markedCharacters.Contains(npc.guid)) {
				npc.MarkForQuest();
			} else {
				npc.UnMarkForQuest();
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
				.Where(x => !destStrings.Contains(x.transform.position.ToString()))
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
			GameObject spawnedBuilding = Instantiate(buildingPrefabs[b.prefabIndex],
						loadedLocation.TileVectorPosition(b.bottomLeftTile, false) + new Vector3(b.width/2f, 0, b.height/2f) * TILE_SIZE, 
						Quaternion.Euler(0, b.angle, 0));
			Teleporter porter = spawnedBuilding.GetComponentInChildren<Teleporter>();
			b.doors[0].position = new SerializableVector3(porter.transform.position);
			porter.LoadSaveData(b.doors[0], loadedLocation.guid);
			teleporters.Add(porter);
		}
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
