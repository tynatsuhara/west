using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveGame {

	public static SaveGame currentGame;

	public bool gameOver;
	public Dictionary<System.Guid, NPCData> savedCharacters;
	public Dictionary<string, Group> groups;
	public Dictionary<System.Guid, Horse.HorseSaveData> horses;
	public Player.PlayerSaveData[] savedPlayers;
	public Map map;
	public QuestManager quests;
	public WorldTime time;
	public Statistics stats;
	public Crime crime;
	public EventQueue events;
	public int saveId;

	// Establishes a new game with data for the player(s)
	public static void NewGame() {
		currentGame = new SaveGame();
		currentGame.saveId = NextSaveId();
		currentGame.horses = new Dictionary<System.Guid, Horse.HorseSaveData>();
		currentGame.savedCharacters = new Dictionary<System.Guid, NPCData>();
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
			NPCData data = npc.SaveData();
			currentGame.savedCharacters[data.guid] = data;
		}

		if (writeToDisk) {
			if (!Directory.Exists(DirPath()))
				Directory.CreateDirectory(DirPath());
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile = File.Create(SavePath(SaveGame.currentGame.saveId));
			formatter.Serialize(saveFile, currentGame);
			saveFile.Close();
		}

		Debug.Log("game saved at " + SavePath(SaveGame.currentGame.saveId));		
	}

	public static void Load(FileInfo saveFile) {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = saveFile.Open(FileMode.Open);
		currentGame = (SaveGame)formatter.Deserialize(stream);
		stream.Close();

		Debug.Log("game loaded from " + saveFile);	
	}

	private static string DirPath() {
		return Application.persistentDataPath + "/saves";
	}

	private static string SavePath(int id) {
		return Application.persistentDataPath + "/saves/save" + id + ".wst";
	}

	public static List<FileInfo> GetSaves() {
		DirectoryInfo dir = new DirectoryInfo(DirPath());
		Debug.Log("game saves stored at " + dir);		
		return dir.GetFiles("*.wst").ToList();
	}

	public static void DeleteSave(int id) {
		File.Delete(SavePath(id));
	}

	public static int NextSaveId() {
		for (int i = 1; ; i++) {
			if (!File.Exists(SavePath(i))) {
				return i;
			}
		}
	}
}
