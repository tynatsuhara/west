﻿using System.Collections;
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

	void Start () {
		tint.sizeDelta = new Vector2(Screen.width, tint.rect.height);
	}

	void Update() {
		inputField.ActivateInputField();
    }

	private readonly int COMMANDS_SHOWN = 6;
	public void EnterCommand(string cmd) {
		inputField.text = "";
		if (cmd.EndsWith("`")) {
			return;
		}
		textLog.Add(cmd);
		string[] parts = Split(cmd);
		if (parts.Length > 0) {
			string command = parts[0];
			if (command.StartsWith("$")) {
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
			NPCData npc = SaveGame.currentGame.savedCharacters.Values.Where(x => x.name.ToLower() == args[0].ToLower()).First();
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

	private void simdebug(string[] args) {
		try {
			NPCData npc = SaveGame.currentGame.savedCharacters.Values.Where(x => x.name.ToLower() == args[0].ToLower()).First();
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
			NPCData npc = SaveGame.currentGame.savedCharacters.Values.Where(x => x.name.ToLower() == args[0].ToLower()).First();
			textLog.Add(Map.Location(npc.location).name);			
		} catch (System.Exception e) {
			textLog.Add("error: locationof expects 1 npc name");
		}
	}

	private void pathfor(string[] args) {
		try {
			NPCData npc = SaveGame.currentGame.savedCharacters.Values.Where(x => x.name.ToLower() == args[0].ToLower()).First();
			textLog.Add(string.Join(" -> ", npc.path.Select(x => Map.Location(x).name).ToArray()));
		} catch (System.Exception e) {
			textLog.Add("error: pathfor expects 1 npc name");
		}
	}

	private void npcfield(string[] args) {
		try {
			NPCData npc = SaveGame.currentGame.savedCharacters.Values.Where(x => x.name.ToLower() == args[0].ToLower()).First();
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
}
