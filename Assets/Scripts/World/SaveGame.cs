using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveGame {

	public static SaveGame currentGame;

	public bool gameOver;
	public Dictionary<System.Guid, int> savedCharacters;
	public PlayerControls.PlayerSaveData[] savedPlayers;
	public QuestManager quests;

	public static void NewGame() {
		currentGame = new SaveGame();
		currentGame.savedPlayers = new PlayerControls.PlayerSaveData[4];
		for (int i = 0; i < 4; i++)
			currentGame.savedPlayers[i] = new PlayerControls.PlayerSaveData();
		currentGame.quests = new QuestManager();
	}

	// Static functions

	public static void Save() {
		currentGame.savedPlayers = GameManager.players.Select(x => x.SaveData()).ToArray();

		if (!Directory.Exists(DirPath()))
			Directory.CreateDirectory(DirPath());
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Create(SavePath());
		formatter.Serialize(saveFile, currentGame);
		saveFile.Close();

		Debug.Log("game saved at " + SavePath());		
	}

	public static void Load() {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Open(SavePath(), FileMode.Open);
		currentGame = (SaveGame)formatter.Deserialize(saveFile);
		saveFile.Close();

		Debug.Log("game loaded from " + SavePath());	
	}

	public static string DirPath() {
		return Application.persistentDataPath + "/saves";
	}

	public static string SavePath() {
		return Application.persistentDataPath + "/saves/save0.wst";
	}

	public static bool SaveGameExists() {
		return File.Exists(SavePath());
	}

	public static void DeleteSave() {
		if (SaveGameExists())
			File.Delete(SavePath());
	}
}
