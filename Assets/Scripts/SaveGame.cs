using UnityEngine;
using System.IO;
using System.Runtime.Serialization;


[System.Serializable]
public class SaveGame {

	public GameObject[] characterPrefabs;

	public static void Save() {
		Debug.Log(Application.persistentDataPath);
	}

	public static void Load() {

	}
}
