using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class DevConsole : MonoBehaviour {

	public RectTransform tint;
	public InputField inputField;
	public Text log;

	private List<string> textLog = new List<string>();
	private List<string> cmdLog = new List<string>();

	// will only run if the terminal is open
	int goBackCommand = -1;
	void Update() {
		inputField.ActivateInputField();
		inputField.text = inputField.text.Replace("`", "");
		if (Input.GetKeyDown(KeyCode.Return)) {
			EnterCommand(inputField.text);
			inputField.text = "";
			goBackCommand = -1;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && goBackCommand < cmdLog.Count-1) {
			if (goBackCommand == -1) {
				currentCommand = inputField.text;
			}
			goBackCommand++;
			LoadOldCommand(goBackCommand);
		} else if (Input.GetKeyDown(KeyCode.DownArrow) && goBackCommand >= 0) {
			goBackCommand--;
			LoadOldCommand(goBackCommand);
		}
    }

	private string currentCommand = "";
	private void LoadOldCommand(int commandIndex) {
		inputField.text = commandIndex == -1 ? currentCommand : cmdLog[goBackCommand];
	}

	private readonly int COMMANDS_SHOWN = 6;
	public void EnterCommand(string cmd) {
		cmd = cmd.Trim();
		textLog.Add(cmd);
		string[] parts = Split(cmd);
		if (parts.Length > 0) {
			cmdLog.Insert(0, cmd);
			string command = parts[0];
			if (command.StartsWith("$")) {  // declaring a variable
				if (parts.Length == 2) {
					vars[command] = parts[1];
				} else {
					textLog.Add(vars.ContainsKey(command) ? vars[command] : "variable not found");
				}
			} else {
				ExecuteCommand(command, parts.Select(x => vars.ContainsKey(x) ? vars[x] : x).Skip(1).ToArray());
			}
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

	private Dictionary<string, string> vars = new Dictionary<string, string>();
	private void ExecuteCommand(string command, string[] args) {
		switch (command) {
			case "ff":
				ff(args);
				break;
			case "movetome":
				movetome(args);
				break;
			case "kill":
				kill(args);
				break;
			case "simdebug":
				simdebug(args);
				break;
			case "locationof":
				locationof(args);
				break;
			case "pathfor":
				pathfor(args);
				break;
			case "npcfield":
				npcfield(args);
				break;
			case "gotochar":
				gotochar(args);
				break;
			case "setspeed":
				setspeed(args);
				break;
			case "fpcam":
				fpcam();
				break;
			case "quest":
				quest(args);
				break;
			case "town":
				town();
				break;
			case "dialogue":
				dialogue(args);
				break;
			default:
				textLog.Add("unrecognized command: " + command);
				break;
		}
	}

	private void ff(string[] args) {
		try {
			GameManager.instance.FastForward(WorldTime.MINUTE * int.Parse(args[0]));
		} catch (System.Exception e) {
			textLog.Add("error: ff expects 1 integer argument");
		}
	}

	private void movetome(string[] args) {
		try {
			NPCData npc = NPCByName(args[0]);
			if (npc.location == Map.CurrentLocation().guid) {
				npc.TravelToLocation(Map.CurrentLocation().guid);
				GameManager.instance.GetCharacter(npc.guid).transform.position = GameManager.players[0].transform.position;
			} else {
				npc.TravelToLocation(Map.CurrentLocation().guid);
				npc.position = new SerializableVector3(GameManager.players[0].transform.position);
				LevelBuilder.instance.SpawnNPC(npc.guid);
			}
		} catch (System.Exception e) {
			if (args.Length > 0) {
				textLog.Add("error: could not find character \"" + args[0] + "\"");
			} else {
				textLog.Add("error: movetome expects 1 npc name");
			}
		}
	}

	private void kill(string[] args) {
		try {
			NPCData npc = NPCByName(args[0]);
			if (npc.location == Map.CurrentLocation().guid) {
				GameManager.instance.GetCharacter(npc.guid).Die();
			} else {
				npc.health = 0;
			}
		} catch (System.Exception e) {
			textLog.Add("error: kill expects 1 npc name");
		}
	}

	private void simdebug(string[] args) {
		try {
			NPCData npc = NPCByName(args[0]);
			if (args.Length == 1) {
				textLog.Add(npc.showSimDebug ? "enabled" : "disabled");
				return;
			}
			npc.showSimDebug = bool.Parse(args[1]);
			textLog.Add((npc.showSimDebug ? "enabled" : "disabled") + " simdebug for " + npc.name);			
		} catch (System.Exception e) {
			textLog.Add("error: simdebug expects 1 npc name and 1 optional bool");
		}
	}

	private void locationof(string[] args) {
		try {
			NPCData npc = NPCByName(args[0]);
			textLog.Add(Map.Location(npc.location).name);			
		} catch (System.Exception e) {
			textLog.Add("error: locationof expects 1 npc name");
		}
	}

	private void pathfor(string[] args) {
		try {
			NPCData npc = NPCByName(args[0]);
			textLog.Add(string.Join(" -> ", npc.path.Select(x => Map.Location(x).name).ToArray()));
		} catch (System.Exception e) {
			textLog.Add("error: pathfor expects 1 npc name");
		}
	}

	private void npcfield(string[] args) {
		try {
			NPCData npc = NPCByName(args[0]);
			System.Type npcType = typeof(NPCData);
			if (args[0] == "find") {
				string[] fields = npcType.GetFields()
						.Select(x => x.Name)
						.Where(x => x.ToLower().StartsWith(args[1]))
						.ToArray();
				textLog.Add(string.Join(", ", fields));				
			} else {
				string fieldName = args[1];
				FieldInfo fieldInfo = npcType.GetField(fieldName);
				textLog.Add(fieldInfo.GetValue(npc).ToString());
			}
		} catch (System.Exception e) {
			textLog.Add("error: npcfield expects 1 npc name and 1 optional field name");
		}
	}

	private void gotochar(string[] args) {
		try {
			NPCData npc = NPCByName(args[0]);
			if (npc.location == Map.CurrentLocation().guid) {
				GameManager.players[0].transform.position = GameManager.instance.GetCharacter(npc.guid).transform.position;
			} else {
				GameManager.instance.loadReposition = npc.position.val;
				GameManager.instance.LoadLocation(npc.location);
			}
		} catch (System.Exception e) {
			textLog.Add("error: gotochar expects 1 npc name");
		}
	}

	private void setspeed(string[] args) {
		try {
			NPCData npc = NPCByName(args[0]);
			GameManager.instance.GetCharacter(npc.guid).moveSpeed = float.Parse(args[0]);
		} catch (System.Exception e) {
			textLog.Add("error: setspeed expects 1 local npc name and 1 float");
		}
	}

	private void fpcam() {
		GameManager.players[0].SwitchCamera(!GameManager.players[0].firstPersonCam.enabled);
	}

	private void quest(string[] args) {
		try {
			switch (args[0]) {
				case "add":
					switch (args[1]) {
						case "kill":
						SaveGame.currentGame.quests.AddQuest(new KillQuest(NPCByName(args[2]).guid));
						break;
					}
				break;
			}
		} catch (System.Exception e) {
			textLog.Add("error: quest expects arguments <add> <kill>");
		}
	}

	private void town() {
		System.Guid town = Map.CurrentTown().guid;
		string characters = string.Join(", ", SaveGame.currentGame.savedCharacters.Values
				.Where(x => Map.Location(x.location).town == town || Map.Location(x.location).guid == town)
				.Select(x => x.name)
				.ToArray()
		);
		textLog.Add(characters);
	}

	private void dialogue(string[] args) {
		try {
			Player p = GameManager.players.First();
			switch (args[0]) {
				case "show":
					bool onRight = args.Length > 1 && args[1].Equals("right");
					p.playerUI.ShowDialogue(onRight);
					break;
				case "hide":
					p.playerUI.HideDialogue();
					break;
			}
		} catch (System.Exception e) {
			textLog.Add("error: dialogue expects argument <show|hide>");
		}
	}


	private NPCData NPCByName(string name) {
		return SaveGame.currentGame.savedCharacters.Values
				.Where(x => x.name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
				.First();
	}
}
