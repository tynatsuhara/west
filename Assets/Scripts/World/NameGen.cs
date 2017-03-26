using System.Collections.Generic;
using UnityEngine;

public class NameGen {
	private Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
	public NameGen(string[] rules) {
		foreach (string s in rules) {
			string[] keyVal = s.Split(':');
			dict.Add(keyVal[0], keyVal[1].Split('|'));
		}
	}
	public string Generate(string nonTerminal) {
		string rule = dict[nonTerminal][Random.Range(0,dict[nonTerminal].Length)];
		foreach (string nonTerm in dict.Keys) {
			while (rule.Contains(nonTerm)) {
				rule = ReplaceFirst(rule, nonTerm, Generate(nonTerm));
			}
		}
		return rule;
	}
	public string ReplaceFirst(string text, string search, string replace) {
		int pos = text.IndexOf(search);
		if (pos < 0)
			return text;
		return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
	}


	private static NameGen townNameGen = new NameGen(new string[] {
		"<name>:<prefix><suffix>|<firstword> <lastword>",
		"<prefix>:Plain|Stag|High|Dread|Stark|Yellow|Red|Dead|Long|Dark|Black",
		"<suffix>:brook|vale|cliff|ridge|canyon|port|crag|worth|reach|range|stone|water|river",
		"<firstword>:Old|Windy|Silent|Lawless|Death's|Wolf's|Bear's|Yellow|Red|Black|Dead",
		"<lastword>:Landing|Canyon|Ridge|Ranch|Hollow|Stead|Stream|Spring"
	});
	public static string TownName() {
		return townNameGen.Generate("<name>");
	}
}