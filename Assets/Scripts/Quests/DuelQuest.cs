[System.Serializable]
public class DuelQuest : Quest {
	private GoToTask centerOfRoad;
	private GoToTask sevenPaces;

	public DuelQuest(System.Guid characterToDuel) {
		// centerOfRoad = new GoToTask()
		// sevenPaces = new GoToTask()
	}

	public override bool complete {
		get { return false; }
	}
}