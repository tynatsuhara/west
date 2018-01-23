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

	private List<string> prevCommands = new List<string>();

	void Start () {
		tint.sizeDelta = new Vector2(Screen.width, tint.rect.height);
	}

	void Update() {
		inputField.ActivateInputField();
    }

	private readonly int COMMANDS_SHOWN = 6;
	public void EnterCommand(string cmd) {
		inputField.text = "";
		prevCommands.Add(cmd);
		int toShow = Mathf.Min(COMMANDS_SHOWN, prevCommands.Count);
		log.text = string.Join("\n", prevCommands.Skip(prevCommands.Count-toShow).ToArray()) + "\n> ";
	}
}
