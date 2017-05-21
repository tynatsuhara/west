using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : Menu {

	public MenuNode loadGame;
	public MenuNode newGame;
	public MenuNode quitGame;

	void Awake() {
		if (!SaveGame.SaveGameExists()) {
			quitGame.down = newGame;
			Destroy(loadGame.gameObject);
			selectedNode = newGame;
			newGame.transform.Translate(transform.up * 40f);
			quitGame.transform.Translate(transform.up * 40f);
		}
	}

	public override void Enter(MenuNode node) {
		if (node == newGame) {
			SaveGame.NewGame();
			GameManager.newGame = true;
			SceneManager.LoadScene("customization");
		} else if (node == loadGame) {
			SceneManager.LoadScene("game");
		} else if (node == quitGame) {
			Application.Quit();			
		}
	}

	public override void Back(MenuNode node) {
		Application.Quit();
	}
}
