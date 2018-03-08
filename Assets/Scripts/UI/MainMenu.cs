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
	private Dictionary<MenuNode, FileInfo> saveFiles = new Dictionary<MenuNode, FileInfo>();	

	private static readonly float TEXT_SPACING = 40f;

	public override void Update() {
		base.Update();

		Vector2 selectedOffset = nodeOffsets[selectedNode.GetComponent<RectTransform>()];
		foreach (RectTransform rt in nodeOffsets.Keys) {
			Vector2 pos = nodeOffsets[rt] - selectedOffset + baseOffset;
			rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, pos, .2f);

			if (rt.gameObject == title) {
				rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
			}
		}
	}

	void Awake() {
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
				.ToDictionary(x => x, x => x.anchoredPosition - baseOffset);
	}

	private void SpawnSaveSlots(List<FileInfo> saves) {
		MenuNode prefab = saveSlots[0];
		RectTransform prefabRect = prefab.GetComponent<RectTransform>();
		prefabRect.anchoredPosition = new Vector2(Screen.width * 2, prefabRect.anchoredPosition.y);

		saveSlots = new MenuNode[saves.Count];

		for (int i = 0; i < saves.Count; i++) {
			string name = saves[i].Name.Replace("save", "").Replace(".wst", "");
			string time = saves[i].LastAccessTime.ToShortDateString();

			saveSlots[i] = Instantiate(prefab);
			saveSlots[i].transform.SetParent(transform);
			RectTransform rt = saveSlots[i].GetComponent<RectTransform>();
			rt.anchoredPosition = prefabRect.anchoredPosition;
			rt.Translate(transform.up * TEXT_SPACING * -i);
			saveSlots[i].GetComponent<Text>().text = "SAVE #" + name + " (" + time + ")";
			saveFiles.Add(saveSlots[i], saves[i]);
		}

		for (int i = 1; i < saves.Count; i++) {
			saveSlots[i-1].down = saveSlots[i];
		}
		saveSlots.Last().down = saveSlots.First();		

		DestroyImmediate(prefab.gameObject);
	}

	public override void Enter(MenuNode node) {
		if (node == newGame) {
			SceneManager.LoadScene("world creation");
		} else if (node == loadGame) {
			NewSelect(saveSlots[0]);
		} else if (node == quitGame) {
			Application.Quit();			
		} else if (saveSlots.Contains(node)) {
			SaveGame.Load(saveFiles[node]);
			SceneManager.LoadScene("game");
		}
	}

	public override void Back(MenuNode node) {
		if (saveSlots.Contains(node)) {
			NewSelect(loadGame);
			return;
		}
		Application.Quit();
	}
}
