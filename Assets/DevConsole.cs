using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DevConsole : MonoBehaviour {

	public RectTransform tint;
	public InputField inputField;
	public Text log;

	private List<string> textLog = new List<string>();

	void Start () {
		tint.sizeDelta = new Vector2(Screen.width, tint.rect.height);
	}

	void Update() {
		inputField.ActivateInputField();
    }

	private readonly int COMMANDS_SHOWN = 6;
	public void EnterCommand(string cmd) {
		inputField.text = "";
		if (cmd == "`") {
			return;
		}
		textLog.Add(cmd);
		string[] parts = Split(cmd);
		if (parts.Length > 0) {
			ExecuteCommand(parts[0], parts.Skip(1).ToArray());
		}
		UpdateLog();
	}

	private void UpdateLog() {
		int toShow = Mathf.Min(COMMANDS_SHOWN, textLog.Count);
		log.text = string.Join("\n", textLog.Skip(textLog.Count-toShow).ToArray()) + "\n> ";
	}

	private string[] Split(string cmd) {
		List<string> parts = new List<string>();
		string word = "";
		foreach (char ch in cmd) {
			if (ch == ' ') {
				if (word.Length > 0 && (!word.StartsWith("\"") || word.EndsWith("\""))) {
					parts.Add(word.Trim());
					word = "";
				}
			} else {
				word += ch;
			}
		}
		if (word.Length > 0) {
			parts.Add(word.Trim());
		}
		return parts.ToArray();
	}

	private Object ExecuteCommand(string command, string[] args) {
		if (command == "ff") {
			ff(args);
		} else {
			textLog.Add("unrecognized command: " + command);			
		}
		return null;
	}

	private void ff(string[] args) {
		try {
			GameManager.instance.FastForward(WorldTime.MINUTE * int.Parse(args[0]));
		} catch (System.Exception e) {
			textLog.Add("error: ff expects 1 integer argument");
		}
	}
}
