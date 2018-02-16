using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/**
  TODO: clean this all the fuck up into one simple coroutine
 */
public class TextObject : MonoBehaviour {

	private Text[] text;
	private List<string> wordQueue;

	private float flashSpeed = .1f;
	private float togglingStartTime;
	private float togglesLeft;
	private bool shouldClear;
	private float timeToClear;
	private float lastSayTime;
	private float cycleInterval;
	private bool looping;
	private bool cycling;

	public bool pausable = true;
	public bool currentlyDisplaying = false;
	public bool ui = false;
	public bool frozen = false;

	void Awake() {
		text = GetComponentsInChildren<Text>();
		if (!ui) {
			for (int i = 0; i < text.Length; i++) {
				text[i].transform.parent.gameObject.layer = LayerMask.NameToLayer("textCam" + i);
			}
		}
		wordQueue = new List<string>();
	}

	void Update() {
		AdjustScale();
		if (GameManager.paused && pausable) {
			// push the start times further into the future
			togglingStartTime += Time.unscaledDeltaTime;
			timeToClear += Time.unscaledDeltaTime;
			lastSayTime += Time.unscaledDeltaTime;
			return;
		}
		if (!frozen) {
			CheckLoopTime();	
			CheckToggleTime();
			CheckClearTime();
		}
	}

	private void AdjustScale() {
		if (!ui) {
			foreach (Player p in GameManager.players) {
				if (p.firstPersonCam.enabled) {
					text[p.id-1].transform.localScale = Vector3.one;					
				} else {
					float overallScale = .13f;
					text[p.id-1].transform.localScale = overallScale * (p.playerCamera.cam.orthographicSize) * Vector3.one;
				}
			}
		}
	}

	void LateUpdate() {
		string s = wordQueue.Count > 0 ? ParseString(wordQueue[0]) : "";
		if (ui) {
			if (!frozen) {
				GetComponent<Text>().text = s;
			}
		} else {
			for (int i = 0; i < GameManager.players.Count; i++) {
				if (!frozen) {
					text[i].text = s;
				}
				text[i].transform.rotation = GameManager.players[i].firstPersonCam.enabled 
						? GameManager.players[i].firstPersonCam.transform.rotation
						: GameManager.players[i].playerCamera.cam.transform.rotation;
			}
		}
	}

	private string ParseString(string str) {
		return str.ToUpper().Replace("%", "È");
	}

	private void CheckLoopTime() {
		if (!cycling || Time.realtimeSinceStartup - lastSayTime < cycleInterval || wordQueue.Count == 0)
			return;

		string str = wordQueue[0];
		wordQueue.RemoveAt(0);
		if (looping) {
			wordQueue.Add(str);
			lastSayTime = Time.realtimeSinceStartup;
		}
	}

	private void CheckClearTime() {
		if (shouldClear && Time.realtimeSinceStartup >= timeToClear) {
			shouldClear = false;
			Clear();
		}
	}

	private void CheckToggleTime() {
		if (togglesLeft <= 0)
			return;

		float elapsed = Time.realtimeSinceStartup - togglingStartTime;
		if (elapsed > flashSpeed) {
			togglesLeft--;
			togglingStartTime += flashSpeed;
			foreach (Text t in text)
				t.enabled = !t.enabled;
		}
	}

	// Dispaly a message in the text box
	// if player != 0, it will only display for that player
	public void Say(string message, bool showFlash = false, float duration = 2f, string color = "white", bool permanent = false, int player=0) {
		looping = false;
		currentlyDisplaying = true;
		SetColor(color);
		wordQueue.Clear();
		wordQueue.Add(message);
		if (player != 0) {
			text[player-1].enabled = true;
		} else {
			foreach (Text t in text)		
				t.enabled = true;
		}
		int flashTimes = 6;
		lastSayTime = Time.realtimeSinceStartup;
		cycling = false;
		if (showFlash) {
			togglingStartTime = Time.realtimeSinceStartup;
			togglesLeft = flashTimes;  // make it an even number
		}

		if (!permanent) {
			shouldClear = true;
			timeToClear = Time.realtimeSinceStartup + duration +  (showFlash ? flashTimes * flashSpeed : 0);
		}
	}

	// Display a series of messages in the text box
	public void Say(string[] messages, float interval = 1f, string color = "white", bool loop = false) {
		looping = loop;
		currentlyDisplaying = true;
		SetColor(color);
		wordQueue.Clear();
		wordQueue.AddRange(messages);
		foreach (Text t in text)
			t.enabled = true;
		shouldClear = false;
		cycling = true;
		cycleInterval = interval;
	}

	// Say a random message from the array
	public void SayRandom(string[] messages, bool showFlash = false, float duration = 2f, string color = "white") {
		int index = Random.Range(0, messages.Length);
		Say(messages[index], showFlash, duration, color);
	}

	// Remove any displayed text
	public void Clear() {
		print("clear()");
		foreach (Text t in text)
			t.text = "";
		currentlyDisplaying = false;
		wordQueue.Clear();
	}

	private void SetColor(string color) {
		Material m;
		if (color == "red") {
			m = GameUI.instance.textRed;
		} else if (color == "green") {
			m = GameUI.instance.textGreen;
		} else if (color == "blue") {
			m = GameUI.instance.textBlue;
		} else if (color == "orange") {
			m = GameUI.instance.textOrange;
		} else if (color == "yellow") {
			m = GameUI.instance.textYellow;
		} else if (color == "grey") {
			m = GameUI.instance.textGrey;
		} else {
			m = GameUI.instance.textWhite;
		}
		foreach (Text t in text)
			t.material = m;		
	}
}
