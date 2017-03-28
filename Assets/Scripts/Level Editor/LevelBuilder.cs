using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelBuilder : MonoBehaviour {

	public static LevelBuilder instance;

	public GameObject floorPrefab;
	public GameObject wallPrefab;
	public GameObject wallCornerPrefab;
	public GameObject doorPrefab;
	public GameObject horsePrefab;

	private PicaVoxel.Volume[,] floorTiles;

	public const int TILE_GRID_LENGTH = 10;
	public const int TILE_SIZE = 2;  // in-game units

	public void Awake() {
		instance = this;
		floorTiles = new PicaVoxel.Volume[TILE_GRID_LENGTH, TILE_GRID_LENGTH];
	}

	public void LoadLocation(System.Guid guid) {
		Location l = SaveGame.currentGame.map.locations[guid];
		SpawnHorses(l);
		SpawnTeleporters(l);

		// TEMP
		GameObject floorHolder = new GameObject();
		floorHolder.name = "Floor";
		for (int i = 0; i < TILE_GRID_LENGTH; i++) {
			for (var j = 0; j < TILE_GRID_LENGTH; j++) {
				GameObject tile = Instantiate(floorPrefab, new Vector3(i * TILE_SIZE, -.2f, j * TILE_SIZE), 
											  Quaternion.identity) as GameObject;
				tile.transform.parent = floorHolder.transform;
				floorTiles[i, j] = tile.GetComponent<PicaVoxel.Volume>();
			}
		}
	}

	public PicaVoxel.Volume FloorTileAt(Vector3 pos) {
		int x = (int)(pos.x / TILE_SIZE);
		int z = (int)(pos.z /  TILE_SIZE);
		if (x >= TILE_GRID_LENGTH || z >= TILE_GRID_LENGTH || x < 0 || z < 0)
			return null;
		return floorTiles[x, z];
	}

	public Floor FloorAt(Vector3 pos) {
		PicaVoxel.Volume f = FloorTileAt(pos);
		return f == null ? null : f.GetComponent<Floor>();
	}


	private void SpawnHorses(Location l) {
		foreach (System.Guid id in l.horses) {
			Horse.HorseSaveData hsd = SaveGame.currentGame.horses[id];
			Horse h = Instantiate(horsePrefab).GetComponent<Horse>();
			h.LoadSaveData(hsd);
		}
	}

	private void SpawnTeleporters(Location l) {
		string s = "Welcome to " + l.name + "! Connections to: ";

		GameObject porterParent = new GameObject();
		porterParent.name = "Teleporters";
		foreach (Teleporter.TeleporterData td in l.teleporters) {
			GameObject porter = new GameObject();
			porter.name = "-> " + SaveGame.currentGame.map.locations[td.toId].name;
			s += SaveGame.currentGame.map.locations[td.toId].name + ", ";
			SphereCollider sc = porter.AddComponent<SphereCollider>();
			sc.isTrigger = true;
			sc.radius = 1.5f;
			porter.transform.parent = porterParent.transform;
			porter.AddComponent<Teleporter>().LoadSaveData(td);
		}
		Debug.Log(s.Substring(0, s.Length - 2));
	}
}
