using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : Menu {

	public Text time;

	public MenuNode resume;
	public MenuNode save;
	public MenuNode quit;

	// called right before the pause menu is displayed
	public void Awaken() {
		time.text = Map.CurrentLocation().name + "\n" + SaveGame.currentGame.time.DateString();
		NewSelect(resume);
	}

	public override void Enter(MenuNode node) {
		if (node == resume) {
			GameManager.instance.SetPaused(false);
		} else if (node == save) {
			GameUI.instance.topCenterText.Say("GAME SAVED", showFlash: true);
			SaveGame.Save(true);
		} else if (node == quit) {
			LevelBuilder.instance.Clean(removePlayers: true);
			SceneManager.LoadScene("main menu");
		}
	}
}
