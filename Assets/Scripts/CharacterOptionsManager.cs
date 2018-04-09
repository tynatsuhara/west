using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/*
	This name is kind of misleading. This class represents all
	the saving/loading functions for character options, as well
	as storing prefabs for spawning weapons in.
*/

public class CharacterOptionsManager : MonoBehaviour {

	public static CharacterOptionsManager instance;

	public Accessory[] accessories;
	public GameObject[] weapons;
	public GameObject[] sidearms;
	public Color32[] skinColors;
	public Color32[] hairColors;
	public Accessory[] hairstyles;

	void Awake() {
		instance = this;
	}

	public void CustomizeFromSave(Player p) {
		Accessory[] accs = new Accessory[] { hairstyles[LoadHairstyle(p.id)], accessories[LoadAccessory(p.id)] };
		p.GetComponent<CharacterCustomization>().ColorCharacter(LoadOutfitName(p.id), LoadSkinColor(p.id), LoadHairColor(p.id), accessories: accs);
	}

	public int CurrentSidearmId(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].sidearmId;
	}

	public void SetSidearmId(int id, byte val) {
		SaveGame.currentGame.savedPlayers[id - 1].sidearmId = val;
	}

	public int CurrentWeaponId(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].weaponId;
	}

	public void SetWeaponId(int id, byte val) {
		SaveGame.currentGame.savedPlayers[id - 1].weaponId = val;
	}
	
	public string LoadOutfitName(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].outfit;
	}

	public void SetOutfit(int id, string name) {
		SaveGame.currentGame.savedPlayers[id - 1].outfit = name;
	}

	public void SetSkinColor(int id, int color) {
		SaveGame.currentGame.savedPlayers[id - 1].skinColor = (byte) ((skinColors.Length + color) % skinColors.Length);
	}

	public int LoadSkinColor(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].skinColor;
	}

	public void SetHairColor(int id, int color) {
		SaveGame.currentGame.savedPlayers[id - 1].hairColor = (byte) ((hairColors.Length + color) % hairColors.Length);
	}

	public int LoadHairColor(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].hairColor;
	}

	public void SetHairstyle(int id, int style) {
		SaveGame.currentGame.savedPlayers[id - 1].hairStyle = (byte) ((hairstyles.Length + style) % hairstyles.Length);
	}

	public int LoadHairstyle(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].hairStyle;
	}

	public void SetAccessory(int id, int acc) {
		SaveGame.currentGame.savedPlayers[id - 1].accessory = (byte) ((accessories.Length + acc) % accessories.Length);
	}

	public int LoadAccessory(int id) {
		return SaveGame.currentGame.savedPlayers[id - 1].accessory;
	}
}
