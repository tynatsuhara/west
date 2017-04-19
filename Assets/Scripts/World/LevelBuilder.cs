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

	private PicaVoxel.Volume[,] floorTiles;
	private Location loadedLocation;

	public static int TILE_SIZE = 2;  // in-game units

	public void Awake() {
		instance = this;
	}

	public void LoadLocation(System.Guid guid) {
		Location l = SaveGame.currentGame.map.locations[guid];
		loadedLocation = l;
		floorTiles = new PicaVoxel.Volume[l.width, l.height];		
		SpawnHorses(l);
		SpawnTeleporters(l);

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
			GameObject porter = Instantiate(teleporterPrefab);
			porter.name = "-> " + SaveGame.currentGame.map.locations[td.toId].name;
			s += SaveGame.currentGame.map.locations[td.toId].name + ", ";
			porter.transform.parent = porterParent.transform;
			porter.GetComponent<Teleporter>().LoadSaveData(td);
			porter.GetComponentInChildren<TextObject>().Say("memes", permanent: true);
		}
		Debug.Log(s.Substring(0, s.Length - 2));
	}
}
