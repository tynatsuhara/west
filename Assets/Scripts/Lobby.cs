using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class Lobby : MonoBehaviour {

	public static Lobby instance;

	public CharacterOptionsManager com;

	public Player[] players;	
	public Camera[] cams;
	private List<int> playingPlayers;
	private bool[] readyPlayers;
	public float rotationSpeed;

	void Awake() {
		GameManager.newGame = true;
		SaveGame.NewGame();
		instance = this;
		playingPlayers = new List<int>(new int[] { 1 });
		readyPlayers = new bool[4];
	}		

	void Start() {
		float dist = 500f;
		cams = new Camera[] { Camera.main, null, null, null };
		for (int i = 1; i < players.Length; i++) {
			cams[i] = (Instantiate(cams[0].gameObject, cams[0].transform.position + Vector3.right * dist * i, 
								   cams[0].transform.rotation) as GameObject).GetComponent<Camera>();
			players[i] = cams[i].GetComponentInChildren<Player>();
			players[i].id = i + 1;
			cams[i].gameObject.SetActive(false);
			cams[i].GetComponentInChildren<WeaponSelection>().playerId = players[i].id;
		}
		foreach (Player pc in players) {
			com.CustomizeFromSave(pc);
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (int playerId in playingPlayers) {
			if (playerId == 1 && Input.GetMouseButton(0)) {
				float dir = Input.GetAxis("Mouse X");
				players[0].transform.RotateAround(players[0].transform.position, Vector3.up, dir * -rotationSpeed);
			} else {
				float dir = Input.GetAxis("Horizontal" + playerId) / 2f;
				players[playerId - 1].transform.RotateAround(players[playerId - 1].transform.position, Vector3.up, dir * -rotationSpeed);
			}
		}
		if (!readyPlayers[0] && Input.GetKeyDown("joystick 1 button 2")) {
			SceneManager.LoadScene("main menu");
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
		SceneManager.LoadScene("world creation");
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
}
