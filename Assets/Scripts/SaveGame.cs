using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveGame {

	public static SaveGame currentGame;

	public GameObject[] generatedCharacters;

	public static void Save() {
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Create(Application.persistentDataPath + "/saves/save.wst");
		formatter.Serialize(saveFile, currentGame);
		saveFile.Close();
	}

	public static void Load() {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Open(Application.persistentDataPath + "/saves/save.wst", FileMode.Open);
		currentGame = (SaveGame)formatter.Deserialize(saveFile);
		saveFile.Close();		
	}
}
