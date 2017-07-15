using UnityEngine;
using System.Linq;

[System.Serializable]
public class DuelQuest : Quest {
	private System.Guid duelingOpponent;
	private GoToTask centerOfRoad;
	private GoToTask sevenPaces;

	// public DuelQuest(System.Guid characterToDuel) {
	// 	centerOfRoad = new GoToTask()
	// 	sevenPaces = new GoToTask()
	// }

	public DuelQuest(System.Guid duelingOpponent) {
		this.duelingOpponent = duelingOpponent;
		Location l = Map.LocationOfCharacter(duelingOpponent);
		centerOfRoad = new GoToTask(l.guid, new Vector3(Random.Range(0, l.width), 0, Random.Range(0, l.height)), true);
		tasks.Add(centerOfRoad);
	}

	public override void Tick() {
		Debug.Log("go duel " + duelingOpponent);
	}
}