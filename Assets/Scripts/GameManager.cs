using UnityEngine;
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

	// FOR TESTING
	public GameObject civPrefab;
	
	public bool objectivesComplete;

	public static List<NPC> characters {
		get { return new List<NPC>(); }	
	}
	public static List<PlayerControls> players;
	public static List<Character> allCharacters {
		get {
			List<Character> lst = characters.Select(x => (Character) x).ToList();
			lst.AddRange(players.Select(x => (Character) x));
			return lst;
		}
	}
	public static List<Horse> localHorses {
		get {
			return Object.FindObjectsOfType<Horse>().ToList();
		}
	}

	public bool friendlyFireEnabled;

	void Awake() {
		instance = this;
	}

	void Start () {
		// needs to happen in start so that instances are set up
		if (newGame) {
			SaveGame.GenerateWorld();
			newGame = false;
		} else {
			SaveGame.Load();
		}

		LoadLocation(SaveGame.currentGame.map.currentLocation);
		players = SpawnPlayers(playersToSpawn);		
		GameObject.Find("Map").GetComponent<VisualMap>().Draw();
		SetTimeScale(1f);

		StartCoroutine(CheckQuests());
	}
	
	void Update () {
		if (!paused) {
			SaveGame.currentGame.time.worldTime += Time.deltaTime;
			SaveGame.currentGame.stats.timePlayed += Time.deltaTime;
		}

		// WIN!
		if (players.All(x => !x.isAlive)) {
			if (!SaveGame.currentGame.gameOver) {
				SaveGame.currentGame.gameOver = true;
				SaveGame.DeleteSave();
				GameUI.instance.ShowEndScreen();
			} else if (Input.anyKeyDown) {
				SceneManager.LoadScene("main menu");
			}
		} else {
			CheckPause();
			CheckSceneReload();
		}
	}

	private void CheckPause() {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 9")) {
			SetPaused(!paused);
		}
	}

	private float currentTimeScale;
	public void SetPaused(bool paused) {
		if (GameManager.paused == paused)
			return;

		if (paused && !GameManager.paused)
			currentTimeScale = Time.timeScale;
		GameManager.paused = paused;
		GameUI.instance.ShowPauseScreen(paused);
		Time.timeScale = paused ? 0 : currentTimeScale;
	}

	public void SetTimeScale(float timeScale) {
		currentTimeScale = timeScale;
		if (!paused)
			Time.timeScale = timeScale;
	}

	private void CheckSceneReload() {
		if ((SaveGame.currentGame.gameOver || paused) && (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown("joystick button 8"))) {
			SetPaused(false);
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	public void LoadLocation(System.Guid guid, float timeChange = 0f) {
		SaveGame.currentGame.time.worldTime += timeChange;
		SaveGame.currentGame.map.currentLocation = guid;
		SetPaused(false);
		GetComponent<LevelBuilder>().LoadLocation(SaveGame.currentGame.map.currentLocation);
	}

	public Vector3 loadReposition;

	private IEnumerator CheckQuests() {
		while (true) {
			SaveGame.currentGame.quests.UpdateQuests();
			yield return new WaitForSeconds(.3f);
		}
	}

	// Alerts all characters to the given range to an event with the given
	// severity and range. If visual is nonnull, the character must have line
	// of sight to the visual to be alerted.
	public void AlertInRange(Character.Reaction importance, Vector3 location, float range, GameObject visual = null) {
		foreach (Character c in characters.Where(x => x.isAlive)) {
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
		foreach (Character c in characters) {
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

	private List<PlayerControls> SpawnPlayers(int[] playersToSpawn) {
		playersToSpawn = playersToSpawn == null || playersToSpawn.Length == 0 ? new int[] { 1 } : playersToSpawn;
		List<PlayerControls> result = new List<PlayerControls>();
		List<Camera> cams = new List<Camera>();
		for (int i = 0; i < playersToSpawn.Length; i++) {
			GameObject p = Instantiate(playerPrefab, new Vector3(i, 1f, 1f), Quaternion.identity) as GameObject;
			PlayerControls pc = p.GetComponent<PlayerControls>();
			pc.id = playersToSpawn[i];
			result.Add(pc);

			pc.playerCamera = (Instantiate(playerCamPrefab) as GameObject).GetComponent<PlayerCamera>();
			pc.playerCamera.player = pc;
			cams.Add(pc.playerCamera.cam);

			pc.playerUI = (Instantiate(playerUIPrefab) as GameObject).GetComponent<PlayerUI>();
			pc.playerUI.GetComponent<Canvas>().worldCamera = pc.playerCamera.cam;
			pc.playerUI.GetComponent<Canvas>().planeDistance = 0;
			pc.playerUI.player = pc;
			pc.playerUI.transform.SetParent(pc.playerCamera.transform, false);

			pc.LoadSaveData(SaveGame.currentGame.savedPlayers[pc.id - 1]);		
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
		foreach (PlayerControls pc in result) {
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
}
