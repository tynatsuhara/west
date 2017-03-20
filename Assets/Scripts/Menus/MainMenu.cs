using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : Menu {

	public MenuNode newGame;
	public MenuNode loadGame;

	void Start() {

	}

	public override void Enter(MenuNode node) {
		if (node == newGame) {
			SaveGame.NewGame();
			SceneManager.LoadScene("customization");
		} else if (node == loadGame) {
			SaveGame.Load();
			SceneManager.LoadScene("game");
		}
	}

	public override void Back(MenuNode node) {
		Application.Quit();
	}
}
