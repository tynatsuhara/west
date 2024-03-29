﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	public static GameUI instance;

	public Material textWhite;
	public Material textRed;
	public Material textGreen;
	public Material textBlue;
	public Material textYellow;
	public Material textOrange;
	public Material textGrey;

	public TextObject topCenterText;
	public GameObject pauseMenu;
	public GameObject winScreen;
	public GameObject devConsole;

	public bool pauseScreenShowing {
		get { return pauseMenu.activeInHierarchy; }
	}

	void Awake () {
		instance = this;
	}

	public void ShowPauseScreen(bool paused) {
		pauseMenu.GetComponent<PauseMenu>().Awaken();
		pauseMenu.SetActive(paused);
	}

	public bool consoleShowing {
		get { return devConsole.activeSelf; }
	}
	public bool ToggleConsole() {
		devConsole.SetActive(!devConsole.activeSelf);
		return devConsole.activeSelf;
	}

	public void ShowEndScreen() {
		winScreen.SetActive(true);
		// string left = "";
		// string right = "";
		// int bigSum = 0;
		// foreach (string s in loot.Keys) {
		// 	if (loot[s].Count > 1)
		// 		left += loot[s].Count + " x ";
		// 	left += s.ToUpper();
		// 	int sum = 0;
		// 	foreach (int x in loot[s])
		// 		sum += x;
		// 	left += "\n";
		// 	right += "$" + sum.ToString("#,##0") + "\n";
		// 	bigSum += sum;
		// }
		// left += "TOTAL";
		// right += "$" + bigSum.ToString("#,##0");
		// Text textL = winScreen.GetComponentsInChildren<Text>().Where(x => x.name == "Loot left").First();
		// Text textR = winScreen.GetComponentsInChildren<Text>().Where(x => x.name == "Loot right").First();
		// textL.text = left;
		// textR.text = right;
		// textL.GetComponent<RectTransform>().position 
		// 		= textR.GetComponent<RectTransform>().position 
		// 		= new Vector3(Screen.width, Screen.height, 0) * .5f + new Vector3(0, 14 + 20 * loot.Keys.Count, 0);

		// PlayerPrefs.SetFloat("totalEarned", PlayerPrefs.GetFloat("totalEarned", 0) + bigSum);
	}
}
