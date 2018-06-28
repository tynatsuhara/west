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
		List<string> messages = new List<string>();

		foreach (Quest q in quests.Values.Where(x => !x.complete && !x.failed)) {
			Task task = q.UpdateQuest();
			if (q.failed) {
				Debug.Log("quest failed: " + q.title);
				q.active = false;			
			} else if (task == null) {
				Debug.Log("quest complete: " + q.title);
				q.complete = true;
				q.active = false;
			} else {
				destinations.AddRange(task.GetLocations());
				messages.Add(task.message);
			}
		}

		LevelBuilder.instance.MarkQuestDestinations(destinations);
		VisualMap.instance.MarkQuestDestinations(destinations);
		GameManager.players.ForEach(x => x.playerUI.UpdateObjectives(messages));
	}

	public void AddQuest(Quest q) {
		quests[q.guid] = q;
	}
}