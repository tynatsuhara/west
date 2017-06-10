using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/*
	This name is kind of misleading. This class represents all
	the saving/loading functions for character options, as well
	as storing prefabs for spawning weapons in.
*/

public class CharacterOptionsManager : MonoBehaviour {

	public static CharacterOptionsManager instance;

	public string nextScene;
	public PlayerControls[] players;
	public Camera[] cams;
	private List<int> playingPlayers;
	private bool[] readyPlayers;
	public float rotationSpeed;
	public Accessory[] accessories;
	public GameObject[] weapons;
	public GameObject[] sidearms;
	public Color32[] skinColors;
	public Color32[] hairColors;
	public Accessory[] hairstyles;

	void Awake() {
		instance = this;
		playingPlayers = new List<int>(new int[] { 1 });
		readyPlayers = new bool[4];
	}

	// LOBBY FUNCTIONS

	void Start() {
		float dist = 500f;
		cams = new Camera[] { Camera.main, null, null, null };
		for (int i = 1; i < players.Length; i++) {
			players[i] = (Instantiate(players[0].gameObject, 
								      players[0].transform.position + Vector3.right * dist * i, 
									  players[0].transform.rotation) as GameObject).GetComponent<PlayerControls>();
			players[i].id = i + 1;
			cams[i] = (Instantiate(cams[0].gameObject, cams[0].transform.position + Vector3.right * dist * i, 
								   cams[0].transform.rotation) as GameObject).GetComponent<Camera>();
			cams[i].gameObject.SetActive(false);
			cams[i].GetComponentInChildren<WeaponSelection>().playerId = players[i].id;
		}
		foreach (PlayerControls pc in players) {
			CustomizeFromSave(pc);
		}
	}
	
	void Update() {
		foreach (int playerId in playingPlayers) {
			if (playerId == 1 && Input.GetMouseButton(0)) {
				float dir = Input.GetAxis("Mouse X");
				players[0].transform.RotateAround(players[0].transform.position, Vector3.up, dir * -rotationSpeed);
			} else {
				float dir = Input.GetAxis("Horizontal" + playerId) / 2f;
				players[playerId - 1].transform.RotateAround(players[playerId - 1].transform.position, Vector3.up, dir * -rotationSpeed);
			}
		}
		// for (int id = 2; id <= 4; id++) {
		// 	if (!playingPlayers.Contains(id) && Input.GetKeyDown("joystick " + id + " button 1")) {
		// 		LobbyJoin(id);
		// 	} else if (playingPlayers.Contains(id) && Input.GetKeyDown("joystick " + id + " button 2")) {
		// 		LobbyLeave(id);
		// 	}
		// }
		if (!readyPlayers[0] && Input.GetKeyDown("joystick 1 button 2")) {
			// TODO: close lobby
		}
	}

	private void LobbyJoin(int id) {
		playingPlayers.Add(id);
		UpdateCameras();
	}

	private void LobbyLeave(int id) {
		playingPlayers.Remove(id);
		UpdateCameras();
	}

	// Returns: new ready state
	public bool ToggleReady(int id) {
		readyPlayers[id - 1] = !readyPlayers[id - 1];
		bool allReady = true;
		foreach (int pid in playingPlayers)
			if (!readyPlayers[pid-1])
				allReady = false;
		if (allReady)
			StartGame();
		return readyPlayers[id - 1];
	}

	private void StartGame() {
		GameManager.playersToSpawn = playingPlayers.ToArray();
		SceneManager.LoadScene(nextScene);
	}

	private void UpdateCameras() {
		foreach (Camera c in cams)
			c.gameObject.SetActive(false);
		if (playingPlayers.Count == 1) {
			cams[playingPlayers[0]-1].rect = new Rect(0, 0, 1, 1);
		} else if (playingPlayers.Count == 2) {
			cams[playingPlayers[0]-1].rect = new Rect(0, .5f, 1, .5f);
			cams[playingPlayers[1]-1].rect = new Rect(0, 0, 1, .5f);
		} else if (playingPlayers.Count == 3) {
			cams[playingPlayers[0]-1].rect = new Rect(0, .5f, 1, .5f);
			cams[playingPlayers[1]-1].rect = new Rect(0, 0, .5f, .5f);
			cams[playingPlayers[2]-1].rect = new Rect(.5f, 0, .5f, .5f);
		} else if (playingPlayers.Count == 4) {
			cams[playingPlayers[0]-1].rect = new Rect(0, .5f, .5f, .5f);
			cams[playingPlayers[1]-1].rect = new Rect(.5f, .5f, .5f, .5f);
			cams[playingPlayers[2]-1].rect = new Rect(0, 0, .5f, .5f);
			cams[playingPlayers[3]-1].rect = new Rect(.5f, 0f, .5f, .5f);
		}
		foreach (int id in playingPlayers)
			cams[id - 1].gameObject.SetActive(true);
	}



	// SAVING/LOADING OPTIONS FUNCTIONS

	public void CustomizeFromSave(PlayerControls p) {
		Accessory[] accs = new Accessory[] { hairstyles[LoadHairstyle(p.id)], accessories[LoadAccessory(p.id)] };
		p.GetComponent<CharacterCustomization>().ColorCharacter(LoadOutfitName(p.id), LoadSkinColor(p.id), LoadHairColor(p.id), accessories: accs);
	}

	public int CurrentSidearmId(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].sidearmId;
	}

	public int CurrentWeaponId(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].weaponId;
	}
	
	public string LoadOutfitName(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].outfit;
	}

	public void SetOutfit(int id, string name) {
		SaveGame.currentGame.savedPlayers[id - 1].outfit = name;
	}

	public void SetSkinColor(int id, int color) {
		SaveGame.currentGame.savedPlayers[id - 1].skinColor = (skinColors.Length + color) % skinColors.Length;
	}

	public int LoadSkinColor(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].skinColor;
	}

	public void SetHairColor(int id, int color) {
		SaveGame.currentGame.savedPlayers[id - 1].hairColor = (hairColors.Length + color) % hairColors.Length;
	}

	public int LoadHairColor(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].hairColor;
	}

	public void SetHairstyle(int id, int style) {
		SaveGame.currentGame.savedPlayers[id - 1].hairStyle = (hairstyles.Length + style) % hairstyles.Length;
	}

	public int LoadHairstyle(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].hairStyle;
	}

	public void SetAccessory(int id, int acc) {
		SaveGame.currentGame.savedPlayers[id - 1].accessory = (accessories.Length + acc) % accessories.Length;
	}

	public int LoadAccessory(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].accessory;
	}
}
