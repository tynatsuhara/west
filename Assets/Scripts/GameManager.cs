﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public static bool paused = false;
	public static int[] playersToSpawn;
	public static bool newGame;

	public GameObject playerPrefab;
	public GameObject playerCamPrefab;
	public GameObject playerUIPrefab;
	
	public bool objectivesComplete;

	public static List<NPC> spawnedNPCs;
	public static List<Player> players;
	public static List<Character> allCharacters {
		get {
			List<Character> lst = spawnedNPCs.Select(x => (Character) x).ToList();
			lst.AddRange(players.Select(x => (Character) x));
			return lst;
		}
	}
	public static List<Horse> spawnedHorses {
		get {
			return Object.FindObjectsOfType<Horse>().ToList();
		}
	}

	public bool friendlyFireEnabled;

	void Awake() {
		instance = this;
	}

	void Start() {
		if (SaveGame.currentGame == null) {  // started this scene in editor
			SaveGame.LoadLastSave();
		}

		// needs to happen in start so that instances are set up
		bool isNewGame = newGame;
		if (newGame) {
			newGame = false;
			float timeOfDay = Random.Range(8, 12) * WorldTime.HOUR +  // between 8AM-12PM
							  Random.Range(0, 60) * WorldTime.MINUTE;
			SimulateWorld(SaveGame.currentGame.time.worldTime, SaveGame.currentGame.time.worldTime + timeOfDay);
		}

		LoadLocation(SaveGame.currentGame.map.currentLocation, firstLoadSinceStartup:true);
		players = SpawnPlayers(playersToSpawn);
		SetTimeScale(1f);
		VisualMap.instance.SpawnIcons();

		StartCoroutine(SaveGame.currentGame.events.Tick());
		StartCoroutine(CheckQuests());
		StartCoroutine(SimulateWorldDuringPlay());

		if (isNewGame) {
			StartCoroutine(SaveAfterNewGame());
		}
	}
	
	void Update() {
		if (!paused) {
			SaveGame.currentGame.time.worldTime += Time.deltaTime;
			SaveGame.currentGame.stats.timePlayed += Time.deltaTime;
		}

		// WIN!
		if (players.All(x => !x.isAlive)) {
			if (!SaveGame.currentGame.gameOver) {
				SaveGame.currentGame.gameOver = true;
				SaveGame.DeleteSave(SaveGame.currentGame.saveId);
				GameUI.instance.ShowEndScreen();
				gameEndTime = Time.time;
			} else if (Input.anyKeyDown && Time.time - gameEndTime >= 2.5f) {
				SceneManager.LoadScene("main menu");
			}
		} else {
			bool esc = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 9");
			bool m = Input.GetKeyDown(KeyCode.M);
			bool onMap = VisualMap.instance.active;
			bool onPauseMenu = GameUI.instance.pauseScreenShowing;
			if (Input.GetKeyDown(KeyCode.BackQuote)) {
				GameUI.instance.ShowPauseScreen(false);
				SetPaused(GameUI.instance.ToggleConsole());
			} else if (GameUI.instance.consoleShowing || players.First().playerUI.IsDialogueShowing()) {
				// capture console input
			} else if (!onMap && !onPauseMenu && esc) {  // game -> pause
				SetPaused(true);
				GameUI.instance.ShowPauseScreen(true);
			} else if (!onMap && !onPauseMenu && m) {    // game -> map
				SetPaused(true);
				VisualMap.instance.active = true;
			} else if (onMap && (esc || m)) {            // map -> game
				SetPaused(false);
				VisualMap.instance.active = false;
			} else if (onPauseMenu && esc) {             // pause -> game
				SetPaused(false);
				GameUI.instance.ShowPauseScreen(false);
			}
		}
	}
	private float gameEndTime;

	public readonly float SIMULATION_TICK = WorldTime.MINUTE;
	private IEnumerator SimulateWorldDuringPlay() {
		float lastSimulationTime = SaveGame.currentGame.time.worldTime;
		while (true) {
			float timePerChar = SIMULATION_TICK / SaveGame.currentGame.savedCharacters.Count;
			float newTime = SaveGame.currentGame.time.worldTime;
			List<NPCData> npcs = SaveGame.currentGame.savedCharacters.Values.ToList();
			foreach (NPCData npc in npcs) {
				if (npc.location == Map.CurrentLocation().guid) {
					continue;
				}
				npc.Simulate(lastSimulationTime, newTime, true);
				yield return new WaitForSeconds(timePerChar);
			}
			lastSimulationTime = newTime;
		}
	}

	// simulates in time increments of SIMULATION_TICK
	private void SimulateWorld(float startTime, float endTime) {
		for (float t = startTime; t < endTime - SIMULATION_TICK; t += SIMULATION_TICK) {
			foreach (NPCData npc in SaveGame.currentGame.savedCharacters.Values) {
				npc.Simulate(t, t + SIMULATION_TICK, false);
			}
			SaveGame.currentGame.events.CheckQueue(false);  // to help with traveling & other events
			SaveGame.currentGame.time.worldTime += SIMULATION_TICK;
		}
	}

	public void FastForward(float time) {
		GameManager.instance.loadReposition = players[0].transform.position;  // ugh
		LoadLocation(Map.CurrentLocation().guid, time);
	}

	private IEnumerator SaveAfterNewGame() {
		yield return new WaitForSeconds(1);
		SaveGame.Save(true);
	}

	private float currentTimeScale;
	public void SetPaused(bool paused) {
		if (GameManager.paused == paused)
			return;

		if (paused && !GameManager.paused)
			currentTimeScale = Time.timeScale;
		GameManager.paused = paused;
		Time.timeScale = paused ? 0 : currentTimeScale;
	}

	public void SetTimeScale(float timeScale) {
		currentTimeScale = timeScale;
		if (!paused)
			Time.timeScale = timeScale;
	}

	public void LoadLocation(System.Guid guid, float timeChange = 0f, bool firstLoadSinceStartup = false) {
		if (!firstLoadSinceStartup) {
			SaveGame.Save(false);
		}
		SaveGame.currentGame.map.Visit(guid);
		SaveGame.currentGame.map.currentLocation = guid;
		if (timeChange > 0) {
			SimulateWorld(SaveGame.currentGame.time.worldTime, SaveGame.currentGame.time.worldTime + timeChange);
		}
		SetPaused(false);
		LevelBuilder.instance.LoadLocation(guid, firstLoadSinceStartup);
	}

	// TODO: move to playersavedata, this is fucking stupid
	public Vector3 loadReposition;

	private IEnumerator CheckQuests() {
		while (true) {
			SaveGame.currentGame.quests.UpdateQuests();
			yield return new WaitForSeconds(.2f);
		}
	}

	// Alerts all characters to the given range to an event with the given
	// severity and range. If visual is nonnull, the character must have line
	// of sight to the visual to be alerted.
	public void AlertInRange(Character.Reaction importance, Vector3 location, float range, GameObject visual = null) {
		foreach (Character c in spawnedNPCs.Where(x => x.isAlive)) {
			if ((c.transform.position - location).magnitude < range) {
				if (visual != null && !c.CanSee(visual))
					continue;
				c.Alert(importance, location);
			}
		}
	}

	// Return all characters in the given range from the given point, ordered by increasing distance
	public List<Character> CharactersWithinDistance(Vector3 from, float range) {
		List<Character> ret = new List<Character>();
		foreach (Character c in spawnedNPCs) {
			if ((c.transform.position - from).magnitude < range) {
				ret.Add(c);
			}
		}
		return ret.OrderBy(c => (c.transform.position - from).magnitude).ToList();
	}

	// Call this to indicate it is no longer a stealth-possible mission,
	// ALARMS HAVE BEEN RAISED -- START SPAWNING ENEMIES
	// public void WereGoingLoudBoys() {
	// 	if (alarmsRaised)
	// 		return;

	// 	alarmsRaised = true;
	// 	foreach (PlayerControls pc in players)
	// 		pc.DrawWeapon();
	// 	GetComponent<EnemySpawner>().StartSpawning();
	// }

	private List<Player> SpawnPlayers(int[] playersToSpawn) {
		playersToSpawn = playersToSpawn == null || playersToSpawn.Length == 0 ? new int[] { 1 } : playersToSpawn;
		List<Player> result = new List<Player>();
		List<Camera> cams = new List<Camera>();
		for (int i = 0; i < playersToSpawn.Length; i++) {
			GameObject p = Instantiate(playerPrefab, new Vector3(i, 1f, 1f), Quaternion.identity) as GameObject;
			Player pc = p.GetComponent<Player>();
			pc.id = playersToSpawn[i];
			result.Add(pc);

			pc.playerCamera = Instantiate(playerCamPrefab).GetComponent<PlayerCamera>();
			pc.playerCamera.player = pc;
			cams.Add(pc.playerCamera.cam);

			pc.playerUI = (Instantiate(playerUIPrefab) as GameObject).GetComponent<PlayerUI>();
			pc.playerUI.GetComponent<Canvas>().worldCamera = pc.playerCamera.cam;
			pc.playerUI.GetComponent<Canvas>().planeDistance = 0;
			pc.playerUI.player = pc;
			pc.playerUI.transform.SetParent(pc.playerCamera.transform, false);

			pc.LoadSaveData(SaveGame.currentGame.savedPlayers[pc.id - 1]);
			pc.playerCamera.transform.position = pc.transform.position;

			// don't delete these guys when we load a new locations!
			LevelBuilder.instance.permanent.Add(pc.transform);
			LevelBuilder.instance.permanent.Add(pc.playerCamera.cam.transform.root);			
		}

		// split screen dimensions
		if (cams.Count == 2) {
			cams[0].rect = new Rect(0, .5f, 1, .5f);
			cams[1].rect = new Rect(0, 0, 1, .5f);
		} else if (cams.Count == 3) {
			cams[0].rect = new Rect(0, .5f, 1, .5f);
			cams[1].rect = new Rect(0, 0, .5f, .5f);
			cams[2].rect = new Rect(.5f, 0, .5f, .5f);
		} else if (cams.Count == 4) {
			cams[0].rect = new Rect(0, .5f, .5f, .5f);
			cams[1].rect = new Rect(.5f, .5f, .5f, .5f);
			cams[2].rect = new Rect(0, 0, .5f, .5f);
			cams[3].rect = new Rect(.5f, 0f, .5f, .5f);
		}
		foreach (Player pc in result) {
			pc.firstPersonCam.rect = pc.playerCamera.cam.rect;
		}
		return result;
	}

	private Dictionary<string, List<int>> lootAmounts = new Dictionary<string, List<int>>();
	public void AddLoot(string category, int dollarAmount) {
		if (category == null || category.Length == 0)
			return;
		
		category = category.ToUpper();
		List<int> loots = lootAmounts.ContainsKey(category) ? lootAmounts[category] : new List<int>();
		loots.Add(dollarAmount);
		lootAmounts[category] = loots;

		Debug.Log(category + " + $" + dollarAmount.ToString("#,##0"));
	}

	public Character GetCharacter(System.Guid guid) {
		return allCharacters.Where(x => x.guid == guid).FirstOrDefault();
	}

	public NPC GetNPC(System.Guid guid) {
		return spawnedNPCs.Where(x => x.guid == guid).FirstOrDefault();
	}
}
