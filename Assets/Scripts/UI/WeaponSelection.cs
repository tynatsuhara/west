using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
	The menu for weapon selection, character customization, and lobby functionality.
*/

public class WeaponSelection : Menu {

	public MenuNode sidearm;
	public MenuNode weapon;
	private CharacterOptionsManager ccm;
	private List<string> outfitNames;

	void Start() {
		ccm = CharacterOptionsManager.instance;
		outfitNames = Outfits.fits.Keys.OrderBy(x => x.ToString()).ToList();
	}

	public override void Carousel(MenuNode node, int dir) {
		if (node == sidearm) {
			node.SetText(LoadWeapon(ccm.sidearms, ccm.CurrentSidearmId(playerId), dir, "_sidearm"));
		} else if (node == weapon) {
			node.SetText(LoadWeapon(ccm.weapons, ccm.CurrentWeaponId(playerId), dir, "_weapon"));
		} else if (node.name == "Outfit") {
			int outfitIndex = (outfitNames.Count + outfitNames.IndexOf(ccm.LoadOutfitName(playerId)) + dir) % outfitNames.Count;
			ccm.SetOutfit(playerId, outfitNames[outfitIndex]);
		} else if (node.name == "Hair") {
			ccm.SetHairstyle(playerId, ccm.LoadHairstyle(playerId) + dir);
		} else if (node.name == "Hair Color") {
			ccm.SetHairColor(playerId, ccm.LoadHairColor(playerId) + dir);
		} else if (node.name == "Skin Color") {
			ccm.SetSkinColor(playerId, ccm.LoadSkinColor(playerId) + dir);
		} else if (node.name == "Accessory") {
			ccm.SetAccessory(playerId, ccm.LoadAccessory(playerId) + dir);
		}
		ccm.CustomizeFromSave(Lobby.instance.players[playerId - 1]);
	}

	private string LoadWeapon(GameObject[] arr, int currentIndex, int dir, string prefString) {
		int index = (arr.Length + currentIndex + dir) % arr.Length;
		GameObject gun = arr[index];
		PlayerPrefs.SetInt("p" + playerId + prefString, index);
		return gun.name.ToUpper();
	}

	public override void Enter(MenuNode node) {
		if (node.name == "Ready") {
			bool nowReady = Lobby.instance.ToggleReady(playerId);
			if (nowReady) {
				node.SetText("READY");
			} else {
				node.SetText("READY?");
			}
		}
	}

	public override void Back(MenuNode node) {
		SceneManager.LoadScene("main menu");
	}
}
