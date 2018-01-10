using UnityEngine;
using System.Linq;

[System.Serializable]
public class DuelQuest : Quest, NPCTaskSource {
	private System.Guid duelingOpponent;
	private GoToTask centerOfRoad;
	private GoToTask sevenPaces;
	private KillTask killTask;

	private Task playerLastReturnedTask;

	public DuelQuest(System.Guid duelingOpponent) {
		this.duelingOpponent = duelingOpponent;
		Location l = Map.Location(SaveGame.currentGame.savedCharacters[duelingOpponent].location);
		centerOfRoad = new GoToTask(l.guid, l.RandomUnoccupiedTile(), false, "Start the duel");
		sevenPaces = new GoToTask(l.guid, l.RandomUnoccupiedTile(), true, "Walk seven paces");
		killTask = new KillTask(duelingOpponent);
		title = "Duel " + SaveGame.currentGame.savedCharacters[duelingOpponent].name;
		SaveGame.currentGame.savedCharacters[duelingOpponent].taskSources.Add(this);
	}

	public override Task UpdateQuest() {
		if (killTask.complete && (!centerOfRoad.complete || !sevenPaces.complete)) {
			failed = true;
			return null;
		}

		playerLastReturnedTask = CurrentQuestTask();
		if (playerLastReturnedTask == null) {  // quest is over
			SaveGame.currentGame.savedCharacters[duelingOpponent].taskSources.Remove(this);			
		}
		return playerLastReturnedTask;
	}

	private Task CurrentQuestTask() {
		if (!centerOfRoad.complete) {
			return centerOfRoad;
		} else if (!sevenPaces.complete) {
			return sevenPaces;
		} else if (!killTask.complete) {
			return killTask;
		}
		return null;
	}

	public NPCTask GetTask(System.Guid character, float time) {
		// if (character != duelingOpponent) {
		// 	Debug.Log("what the fuck?");
		// 	return null;
		// }
		// if (playerLastReturnedTask == killTask) {
			return new NPCKillTask();
		// }
		// return null;
	}
}