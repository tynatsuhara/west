using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldGeneration : MonoBehaviour {

	public Text loading;
	public Text display;  // details

	void Start() {
		StartCoroutine(GenerateWorld());
	}
	
	private IEnumerator GenerateWorld() {
		// Random.InitState(5);  // TODO: make seed work
		// Debug.Log(Random.Range(0, 10));
		// Debug.Log(Random.Range(0, 10));
		// Debug.Log(Random.Range(0, 10));
		
		SaveGame.currentGame.time = new WorldTime();
		SaveGame.currentGame.events = new EventQueue();
		SaveGame.currentGame.map = new Map();
		SaveGame.currentGame.groups = DefaultGroups();
		yield return StartCoroutine(SaveGame.currentGame.map.MakeMap(display));
		SaveGame.currentGame.quests = new QuestManager();	
		SaveGame.currentGame.stats = new Statistics();
		SaveGame.currentGame.crime = new Crime();
		SceneManager.LoadScene("game");
	}

	private Dictionary<string, Group> DefaultGroups() {
		Dictionary<string, Group> result = new Dictionary<string, Group>();
		result.Add(Group.LAW_ENFORCEMENT, new Group(Group.LAW_ENFORCEMENT));
		result.Add(Group.PLAYERS, new Group(Group.PLAYERS));
		return result;
	}
}
