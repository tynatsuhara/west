using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : Menu {

	public MenuNode newGame;
	public MenuNode loadGame;
	public MenuNode quitGame;

	void Start() {
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
