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

		LoadWeapon(ccm.sidearms, ccm.CurrentSidearmId(playerId), 0, sidearm);
		LoadWeapon(ccm.weapons, ccm.CurrentWeaponId(playerId), 0, weapon);
	}

	public override void Carousel(MenuNode node, int dir) {
		if (node == sidearm) {
			ccm.SetSidearmId(playerId, LoadWeapon(ccm.sidearms, ccm.CurrentSidearmId(playerId), dir, sidearm));
		} else if (node == weapon) {
			ccm.SetWeaponId(playerId, LoadWeapon(ccm.weapons, ccm.CurrentWeaponId(playerId), dir, weapon));
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

	private byte LoadWeapon(GameObject[] arr, int currentIndex, int dir, MenuNode node) {
		int oldIndex = 0;
		if (currentIndex == 1 && dir == -1) {
			currentIndex = arr.Length-1;
		} else if (currentIndex == arr.Length-1 && dir == 1) {
			currentIndex = 1;
		} else {
			currentIndex += dir;
		}
		Debug.Log(oldIndex + " => " + currentIndex);
		node.SetText(arr[currentIndex].name.ToUpper());
		return (byte)currentIndex;
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
