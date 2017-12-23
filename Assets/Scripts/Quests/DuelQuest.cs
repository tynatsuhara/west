using UnityEngine;
using System.Linq;

[System.Serializable]
public class DuelQuest : Quest {
	private System.Guid duelingOpponent;
	private GoToTask centerOfRoad;
	private GoToTask sevenPaces;
	private KillTask killTask;

	public DuelQuest(System.Guid duelingOpponent) {
		this.duelingOpponent = duelingOpponent;
		Location l = Map.LocationOfCharacter(duelingOpponent);
		centerOfRoad = new GoToTask(l.guid, l.TileVectorPosition(l.RandomUnoccupiedTile()), false, "Start the duel");
		sevenPaces = new GoToTask(l.guid, l.TileVectorPosition(l.RandomUnoccupiedTile()), true, "Walk seven paces");
		killTask = new KillTask(duelingOpponent);
		title = "Duel " + SaveGame.currentGame.savedCharacters[duelingOpponent].name;
	}

	public override Task UpdateQuest() {
		if (killTask.complete && (!centerOfRoad.complete || !sevenPaces.complete)) {
			failed = true;
			return null;
		}

		if (!centerOfRoad.complete) {
			return centerOfRoad;
		} else if (!sevenPaces.complete) {
			return sevenPaces;
		} else if (!killTask.complete) {
			return killTask;
		}

		return null;  // complete!
	}
}