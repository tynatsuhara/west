using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DevConsole : MonoBehaviour {

	public RectTransform tint;
	public InputField inputField;
	public Text log;

	private List<string> prevCommands = new List<string>();

	void Start () {
		tint.sizeDelta = new Vector2(Screen.width, tint.rect.height);
	}

	void Update() {
		inputField.ActivateInputField();
    }

	public void EnterCommand(string cmd) {
		inputField.text = "";
		prevCommands.Add(cmd);
		log.text = string.Join("\n", prevCommands.ToArray()) + "\n> ";
	}
}
