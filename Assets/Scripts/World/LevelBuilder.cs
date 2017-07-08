﻿using UnityEngine;
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

	private Teleporter[] teleporters;
	private PicaVoxel.Volume[,] floorTiles;
	private Location loadedLocation;

	public static int TILE_SIZE = 2;  // in-game units

	public void Awake() {
		instance = this;
	}

	public void LoadLocation(System.Guid guid) {
		bool firstLoad = GameManager.instance.loadReposition == Vector3.zero;

		if (!firstLoad) {
			SaveGame.Save(false);
			Clean();
		} else {
			permanent = Object.FindObjectsOfType<Transform>().Where(x => x.parent == null).ToList();
		}

		Location l = Map.Location(guid);
		loadedLocation = l;
		mat.SetColor("_Tint", biomeColors[l.biomeColor]);
		floorTiles = new PicaVoxel.Volume[l.width, l.height];
		SpawnBuildings();
		SpawnNPCs();
		SpawnHorses(firstLoad);
		SpawnAtmospherics();
		SpawnTeleporters();
		SaveGame.currentGame.quests.UpdateQuests();  // mark quests at teleporters
		string greeting = l.name + ", population " + l.characters.Count;
		int bounty = SaveGame.currentGame.savedPlayers.Select(x => SaveGame.currentGame.crime.Bounty(x.guid, guid)).Aggregate((x, y) => x + y);
		GameUI.instance.topCenterText.Say(bounty > 0 ? greeting + "\nbounty $" + bounty : greeting);

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
			NPC.NPCSaveData data = SaveGame.currentGame.savedCharacters[id];
			NPC npc = Instantiate(npcPrefab).GetComponent<NPC>();
			npc.LoadSaveData(data);
		}
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
		// cacti
		foreach (int tile in loadedLocation.cacti.Keys) {
			GameObject c = Instantiate(cactusPrefab, loadedLocation.TileVectorPosition(tile), Quaternion.identity);
			c.GetComponent<Cactus>().LoadSaveData(loadedLocation.cacti[tile]);
		}

		// tumbly weedos
		int tumbleweeds = Random.Range(0, 2);
		for (int i = 0; i < tumbleweeds; i++) {
			int tile = loadedLocation.RandomUnoccupiedTile();
			Instantiate(tumbleweedPrefab, loadedLocation.TileVectorPosition(tile), Quaternion.identity);
		}

		// headstones
		foreach (int tile in loadedLocation.headstones.Keys) {
			GameObject h = Instantiate(headstonePrefab, loadedLocation.TileVectorPosition(tile), Quaternion.identity);
			h.GetComponent<Headstone>().LoadSaveData(loadedLocation.headstones[tile]);
		}
	}

	private void SpawnTeleporters() {
		var teleporterList = new List<Teleporter>();
		GameObject porterParent = new GameObject();
		porterParent.name = "Teleporters";
		foreach (Teleporter.TeleporterData td in loadedLocation.teleporters) {
			GameObject porter = Instantiate(teleporterPrefab);
			porter.name = "-> " + SaveGame.currentGame.map.locations[td.toId].name;
			porter.transform.parent = porterParent.transform;
			porter.GetComponent<Teleporter>().LoadSaveData(td, loadedLocation.guid);
			teleporterList.Add(porter.GetComponent<Teleporter>());
		}
		teleporters = teleporterList.ToArray();
		MarkQuestDestinations();
	}

	public void MarkQuestDestinations() {
		// Quest teleporter destinations		
		List<Task.TaskDestination> destinations = new List<Task.TaskDestination>();
		foreach (Quest q in SaveGame.currentGame.quests.markedQuests)
			destinations.AddRange(q.GetLocations());
		List<System.Guid> questTeleportDestinations = destinations
				.Where(x => loadedLocation.guid != x.location)
				.Select(x => SaveGame.currentGame.map.BestPathFrom(loadedLocation.guid, x.location)[0])
				.ToList();
		foreach (Teleporter t in teleporters) {
			t.MarkQuest(questTeleportDestinations.Contains(t.toId));
		}

		// Quests in this location, spawn markers at spots
		List<Vector3> destinationMarkers = destinations
				.Where(x => loadedLocation.guid == x.location)
				.Select(x => x.position)
				.ToList();
		GameObject spotParent = GameObject.Find("QuestPositionParent");
		if (spotParent != null) {
			Destroy(spotParent);
		}
		if (destinationMarkers.Count > 0) {
			spotParent = new GameObject();
			spotParent.name = "QuestPositionParent";
			foreach (Vector3 v in destinationMarkers) {
				GameObject spot = Instantiate(destinationMarkerPrefab, v, Quaternion.identity);
				spot.transform.SetParent(spotParent.transform);
			}
		}
	}

	private void SpawnBuildings() {
		foreach (Building b in loadedLocation.buildings) {
			int t = b.bottomLeftTile;
			Instantiate(buildingPrefabs[b.prefabIndex],
						loadedLocation.TileVectorPosition(b.bottomLeftTile, false) + new Vector3(b.width/2f, 0, b.height/2f) * TILE_SIZE, 
						Quaternion.Euler(0, b.angle, 0));
		}
	}

	private void PositionWalls() {
		Transform walls = GameObject.Find("Walls").transform;
		// bottom
		walls.GetChild(0).localScale = new Vector3(loadedLocation.width * TILE_SIZE + 2, 10, 1);
		walls.GetChild(0).position = new Vector3(loadedLocation.width * TILE_SIZE/2f, 1, -.5f);
		// top
		walls.GetChild(1).localScale = new Vector3(loadedLocation.width * TILE_SIZE + 2, 10, 1);
		walls.GetChild(1).position = new Vector3(loadedLocation.width * TILE_SIZE/2f, 1, loadedLocation.height * TILE_SIZE + .5f);
		// right
		walls.GetChild(2).localScale = new Vector3(1, 10, loadedLocation.height * TILE_SIZE + 2);
		walls.GetChild(2).position = new Vector3(loadedLocation.width * TILE_SIZE + .5f, 1, loadedLocation.height * TILE_SIZE/2f);
		// left
		walls.GetChild(3).localScale = new Vector3(1, 10, loadedLocation.height * TILE_SIZE + 2);
		walls.GetChild(3).position = new Vector3(-.5f, 1, loadedLocation.height * TILE_SIZE/2f);
	}
}
