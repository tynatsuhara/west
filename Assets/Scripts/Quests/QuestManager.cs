using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class QuestManager {
	public Dictionary<System.Guid, Quest> quests = new Dictionary<System.Guid, Quest>();
	public List<System.Guid> activeQuests = new List<System.Guid>();	
	public List<System.Guid> completedQuests = new List<System.Guid>();
	public List<Quest> markedQuests {
		get { return activeQuests.Select(g => quests[g]).ToList(); }	
	}

	public QuestManager() {
		var characters = SaveGame.currentGame.savedCharacters.Keys.ToArray();

		// temp quest spawning
		AddQuest(new DuelQuest(characters[Random.Range(0, characters.Length)]));
	}

	public void UpdateQuests() {
		bool anyComplete = false;
		foreach (Quest q in quests.Values) {
			q.Tick();
			if (q.complete) {
				Debug.Log("quest complete");
				anyComplete = true;
				activeQuests.Remove(q.guid);
				completedQuests.Add(q.guid);
			}
		}
		if (anyComplete) {
			LevelBuilder.instance.MarkQuestDestinations();
		}
	}

	public void AddQuest(Quest q) {
		quests[q.guid] = q;
	}
}