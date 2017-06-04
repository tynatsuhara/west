using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldGenerationWait : MonoBehaviour {

	public Text display;

	void Start () {
		StartCoroutine(Delay());
	}
	
	private IEnumerator Delay() {
		yield return new WaitForSeconds(.1f);
		GameManager.newGame = true;
		SaveGame.NewGame();		
		SaveGame.GenerateWorld();
		SceneManager.LoadScene("customization");		
	}
}
