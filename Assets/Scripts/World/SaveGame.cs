using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveGame {

	public static SaveGame currentGame;

	public bool gameOver;
	public Dictionary<System.Guid, PlayerControls.PlayerSaveData> savedCharacters;
	public Dictionary<System.Guid, Horse.HorseSaveData> horses;
	public PlayerControls.PlayerSaveData[] savedPlayers;
	public Map map;
	public QuestManager quests;

	// Establishes a new game with data for the player(s)
	public static void NewGame() {
		currentGame = new SaveGame();
		currentGame.horses = new Dictionary<System.Guid, Horse.HorseSaveData>();
		currentGame.savedCharacters = new Dictionary<System.Guid, PlayerControls.PlayerSaveData>();
		currentGame.savedPlayers = new PlayerControls.PlayerSaveData[4];
		for (int i = 0; i < 4; i++)
			currentGame.savedPlayers[i] = new PlayerControls.PlayerSaveData();
	}

	// Called once character creation has been done
	public static void GenerateWorld() {
		currentGame.map = new Map();
		currentGame.quests = new QuestManager();		
	}

	public static void Save() {
		// save players in scene
		currentGame.savedPlayers = GameManager.players.Select(x => x.SaveData()).ToArray();

		// update local wildlife
		foreach (Horse h in GameManager.localHorses) {
			Horse.HorseSaveData hsd = h.SaveData();
			currentGame.horses[hsd.guid] = hsd;
		}

		if (!Directory.Exists(DirPath()))
			Directory.CreateDirectory(DirPath());
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Create(SavePath());
		formatter.Serialize(saveFile, currentGame);
		saveFile.Close();

		// Debug.Log("game saved at " + SavePath());		
	}

	public static void Load() {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Open(SavePath(), FileMode.Open);
		currentGame = (SaveGame)formatter.Deserialize(saveFile);
		saveFile.Close();

		// Debug.Log("game loaded from " + SavePath());	
	}

	private static string DirPath() {
		return Application.persistentDataPath + "/saves";
	}

	private static string SavePath() {
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
