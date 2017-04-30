using UnityEngine;
using System.Linq;

[System.Serializable]
public class DuelQuest : Quest {
	private GoToTask centerOfRoad;
	private GoToTask sevenPaces;

	// public DuelQuest(System.Guid characterToDuel) {
	// 	centerOfRoad = new GoToTask()
	// 	sevenPaces = new GoToTask()
	// }

	public DuelQuest() {
		// TEMP: choose a random location for the duel
		var ls = SaveGame.currentGame.map.locations;
		Location l = Map.Location(ls.Keys.ToArray()[Random.Range(0, ls.Keys.Count)]);
		centerOfRoad = new GoToTask(l.guid, new Vector3(Random.Range(0, l.width), 0, Random.Range(0, l.height)), true);
		tasks.Add(centerOfRoad);
	}
}