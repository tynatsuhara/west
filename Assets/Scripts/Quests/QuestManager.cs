using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class QuestManager {
	private List<Quest> quests;
	private List<Quest> completedQuests;

	public QuestManager() {
		quests = new List<Quest>();
		completedQuests = new List<Quest>();
	}

	public void UpdateQuests() {
		bool anyComplete = false;
		foreach (Quest q in quests) {
			if (q.complete) {
				Debug.Log("quest complete");
				anyComplete = true;
			}
		}
		if (anyComplete) {
			completedQuests.AddRange(quests.Where(x => x.complete));
			quests = quests.Where(x => !x.complete).ToList();
		}
	}

	public void AddQuest(Quest q) {
		quests.Add(q);
	}
}