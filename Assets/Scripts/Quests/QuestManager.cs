using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class QuestManager {
	public List<Quest> quests = new List<Quest>();
	public List<Quest> completedQuests = new List<Quest>();
	public List<Quest> markedQuests {
		get { return quests; }	
	}

	public QuestManager() {
		var characters = SaveGame.currentGame.savedCharacters.Keys.ToArray();
		quests.Add(new DuelQuest(characters[Random.Range(0, characters.Length)]));  // TEMP
	}

	public void UpdateQuests() {
		bool anyComplete = false;
		foreach (Quest q in quests) {
			q.Tick();
			if (q.complete) {
				Debug.Log("quest complete");
				anyComplete = true;
			}
		}
		if (anyComplete) {
			completedQuests.AddRange(quests.Where(x => x.complete));
			quests = quests.Where(x => !x.complete).ToList();
			LevelBuilder.instance.MarkQuestDestinations();
		}
	}

	public void AddQuest(Quest q) {
		quests.Add(q);
	}
}