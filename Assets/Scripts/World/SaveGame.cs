using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveGame {

	public static SaveGame currentGame;

	public bool gameOver;
	public Dictionary<System.Guid, NPC.NPCSaveData> savedCharacters;
	public Dictionary<string, Group> groups;
	public Dictionary<System.Guid, Horse.HorseSaveData> horses;
	public Player.PlayerSaveData[] savedPlayers;
	public Map map;
	public QuestManager quests;
	public WorldTime time;
	public Statistics stats;
	public Crime crime;
	public EventQueue events;

	// Establishes a new game with data for the player(s)
	public static void NewGame() {
		currentGame = new SaveGame();
		currentGame.horses = new Dictionary<System.Guid, Horse.HorseSaveData>();
		currentGame.savedCharacters = new Dictionary<System.Guid, NPC.NPCSaveData>();
		currentGame.savedPlayers = new Player.PlayerSaveData[4];
		for (int i = 0; i < 4; i++)
			currentGame.savedPlayers[i] = new Player.PlayerSaveData();
	}

	public static void Save(bool writeToDisk) {
		// save players in scene
		currentGame.savedPlayers = GameManager.players.Select(x => x.SaveData()).ToArray();

		// save local wildlife
		foreach (Horse h in GameManager.spawnedHorses) {
			Horse.HorseSaveData data = h.SaveData();
			if (Map.CurrentLocation().horses.Contains(data.guid)) {
				currentGame.horses[data.guid] = data;
			}
		}

		// save local NPCs
		foreach (NPC npc in GameManager.spawnedNPCs) {
			NPC.NPCSaveData data = npc.SaveData();
			currentGame.savedCharacters[data.guid] = data;
		}

		// save any necessary things in the locations
		currentGame.map.Save();

		if (writeToDisk) {
			if (!Directory.Exists(DirPath()))
				Directory.CreateDirectory(DirPath());
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile = File.Create(SavePath());
			formatter.Serialize(saveFile, currentGame);
			saveFile.Close();
		}

		Debug.Log("game saved at " + SavePath());		
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
