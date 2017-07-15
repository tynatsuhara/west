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
		centerOfRoad = new GoToTask(l.guid, new Vector3(Random.Range(0, l.width), 0, Random.Range(0, l.height)), false, "Start the duel");
		sevenPaces = new GoToTask(l.guid, new Vector3(Random.Range(0, l.width), 0, Random.Range(0, l.height)), true, "Walk seven paces");
		killTask = new KillTask(duelingOpponent);
	}

	public override Task UpdateQuest() {
		if (killTask.complete && (!centerOfRoad.complete || !sevenPaces.complete)) {
			Debug.Log("killTask is complete I guess");
			failed = true;
			return null;
		}

		if (!centerOfRoad.complete) {
			return centerOfRoad;
		} else if (!sevenPaces.complete) {
			return sevenPaces;
		} else {
			return killTask;
		}
	}
}