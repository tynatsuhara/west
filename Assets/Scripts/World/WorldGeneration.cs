using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldGeneration : MonoBehaviour {

	public Text loading;
	public Text display;  // details

	void Start () {
		StartCoroutine(GenerateWorld());
	}
	
	private IEnumerator GenerateWorld() {
		GameManager.newGame = true;
		SaveGame.NewGame();
		SaveGame.currentGame.time = new WorldTime();
		SaveGame.currentGame.events = new EventQueue();
		SaveGame.currentGame.groups = DefaultGroups();
		SaveGame.currentGame.map = new Map();
		yield return StartCoroutine(SaveGame.currentGame.map.MakeMap(display));
		SaveGame.currentGame.quests = new QuestManager();	
		SaveGame.currentGame.stats = new Statistics();
		SaveGame.currentGame.crime = new Crime();
		SceneManager.LoadScene("customization");
	}

	private Dictionary<string, Group> DefaultGroups() {
		Dictionary<string, Group> result = new Dictionary<string, Group>();
		result.Add(Group.CRIMINALS, new Group(Group.CRIMINALS));
		return result;
	}
}
