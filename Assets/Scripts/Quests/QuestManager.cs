using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class QuestManager {
	public Dictionary<System.Guid, Quest> quests = new Dictionary<System.Guid, Quest>();

	public QuestManager() {
		var characters = SaveGame.currentGame.savedCharacters.Keys.ToArray();

		// temp quest spawning
		AddQuest(new DuelQuest(characters[Random.Range(0, characters.Length)]));
	}

	public void UpdateQuests() {
		List<Task.TaskDestination> destinations = new List<Task.TaskDestination>();
		foreach (Quest q in quests.Values.Where(x => !x.complete)) {
			Task task = q.UpdateQuest();
			if (q.failed) {
				Debug.Log("quest failed: " + q.title);
				q.active = true;
				q.active = false;			
			} else if (task == null) {
				Debug.Log("quest complete: " + q.title);
				q.complete = true;
				q.active = false;
			} else {
				Debug.Log(q.title + ": " + task.message);
				destinations.AddRange(task.GetLocations());
			}
		}
		LevelBuilder.instance.MarkQuestDestinations(destinations);
		VisualMap.instance.MarkQuestDestinations(destinations);
	}

	public void AddQuest(Quest q) {
		quests[q.guid] = q;
	}
}