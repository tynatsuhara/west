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

		print(string.Join(", ", Split("movetome \"inez king\"")));
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
		for (int i = 0; i <= cmd.Length; i++) {
			if (i == cmd.Length || cmd[i] == ' ') {
				if (word.StartsWith("\"")) {
					if (word.EndsWith("\"")) {
						parts.Add(word.Trim('\"', ' '));
						word = "";
					} else {
						word += ' ';						
					}
				} else if (word.Length > 0) {
					parts.Add(word.Trim());
					word = "";
				}
			} else {
				word += cmd[i];
			}
		}
		return parts.ToArray();
	}

	private Object ExecuteCommand(string command, string[] args) {
		if (command == "ff") {
			ff(args);
		} else if (command == "movetome") {
			movetome(args);
		} else if (command == "kill") {
			kill(args);
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

	private void kill(string[] args) {
		try {
			NPCData npc = SaveGame.currentGame.savedCharacters.Values.Where(x => x.name.ToLower() == args[0].ToLower()).First();
			if (npc.location == Map.CurrentLocation().guid) {
				GameManager.instance.GetCharacter(npc.guid).Die();
			} else {
				npc.health = 0;
			}
		} catch (System.Exception e) {
			textLog.Add("error: kill expects 1 npc name");
		}
	}

	private void movetome(string[] args) {
		try {
			NPCData npc = SaveGame.currentGame.savedCharacters.Values.Where(x => x.name.ToLower() == args[0].ToLower()).First();
			if (npc.location == Map.CurrentLocation().guid) {
				GameManager.instance.GetCharacter(npc.guid).transform.position = GameManager.players[0].transform.position;
			} else {
				npc.location = Map.CurrentLocation().guid;
				npc.position = new SerializableVector3(GameManager.players[0].transform.position);
				LevelBuilder.instance.SpawnNPC(npc.guid);
			}
		} catch (System.Exception e) {
			textLog.Add("error: movetome expects 1 npc name");
		}
	}
}
