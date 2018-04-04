using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class MainMenu : Menu {

	public GameObject title;
	public MenuNode loadGame;
	public MenuNode newGame;
	public MenuNode quitGame;

	private Dictionary<RectTransform, Vector2> nodeOffsets;
	private Vector2 baseOffset;

	public MenuNode[] saveSlots;
	public OffScreenSlide xForDelete;
	private Dictionary<MenuNode, FileInfo> saveFiles = new Dictionary<MenuNode, FileInfo>();
	private MenuNode backFromSaves;

	private static readonly float TEXT_SPACING = 40f;

	void FixedUpdate() {
		Vector2 selectedOffset = nodeOffsets[selectedNode.GetComponent<RectTransform>()];
		foreach (RectTransform rt in nodeOffsets.Keys) {
			if (rt == null)
				continue;

			Vector2 pos = nodeOffsets[rt] - selectedOffset + baseOffset;
			rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, pos, .2f);

			if (rt.gameObject == title) {
				rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
			}
		}
	}

	void Awake() {
		Time.timeScale = 1;  // gets stuck on pause when exiting game scene

		List<FileInfo> saves = SaveGame.GetSaves();
		if (saves.Count == 0) {
			quitGame.down = newGame;
			Destroy(loadGame.gameObject);
			selectedNode = newGame;
			newGame.transform.Translate(transform.up * TEXT_SPACING);
			quitGame.transform.Translate(transform.up * TEXT_SPACING);
		} else {
			SpawnSaveSlots(saves);			
		}

		baseOffset = selectedNode.GetComponent<RectTransform>().anchoredPosition;
		nodeOffsets = GetComponentsInChildren<RectTransform>()
				.Where(x => x.gameObject == title || x.GetComponent<MenuNode>() != null)
				.ToDictionary(x => x, x => x.anchoredPosition - baseOffset);
	}

	private void SpawnSaveSlots(List<FileInfo> saves) {
		saves = saves.OrderBy(x => x.LastAccessTime).Reverse().ToList();
		MenuNode prefab = saveSlots[0];
		RectTransform prefabRect = prefab.GetComponent<RectTransform>();
		prefabRect.anchoredPosition = new Vector2(Screen.width * 2, prefabRect.anchoredPosition.y);

		saveSlots = new MenuNode[saves.Count];

		// spawn save slots
		for (int i = 0; i < saves.Count; i++) {
			string name = saves[i].Name.Replace("save", "").Replace(".wst", "");
			string time = saves[i].LastAccessTime.ToLocalTime().ToString();

			saveSlots[i] = Instantiate(prefab);
			saveSlots[i].transform.SetParent(transform);
			RectTransform rt = saveSlots[i].GetComponent<RectTransform>();
			rt.anchoredPosition = prefabRect.anchoredPosition;
			rt.Translate(transform.up * TEXT_SPACING * -i);
			saveSlots[i].GetComponent<Text>().text = "SAVE #" + name + "  (" + time + ")";
			saveFiles.Add(saveSlots[i], saves[i]);
		}

		// link nodes
		for (int i = 1; i < saves.Count; i++) {
			saveSlots[i-1].down = saveSlots[i];
		}

		// spawn back button
		backFromSaves = Instantiate(prefab);
		backFromSaves.transform.SetParent(transform);
		RectTransform brt = backFromSaves.GetComponent<RectTransform>();
		brt.anchoredPosition = prefabRect.anchoredPosition;
		brt.Translate(transform.up * TEXT_SPACING * -saves.Count);
		brt.GetComponent<Text>().text = "BACK";
		saveSlots.Last().down = backFromSaves;
		backFromSaves.down = saveSlots.First();

		DestroyImmediate(prefab.gameObject);
	}

	public override void Enter(MenuNode node) {
		if (node == newGame) {
			GameManager.newGame = true;
			SaveGame.NewGame();
			SceneManager.LoadScene("customization");
		} else if (node == loadGame) {
			xForDelete.MoveOnScreen();
			NewSelect(saveSlots[0]);
		} else if (node == quitGame) {
			Application.Quit();			
		} else if (saveSlots.Contains(node)) {
			SaveGame.Load(saveFiles[node]);
			SceneManager.LoadScene("game");
		} else if (node == backFromSaves) {
			Back(backFromSaves);
		}
	}

	public override void Back(MenuNode node) {
		if (saveSlots.Contains(node) || node == backFromSaves) {
			xForDelete.MoveOffScreen();
			NewSelect(loadGame);
			return;
		}
		Application.Quit();
	}
}
