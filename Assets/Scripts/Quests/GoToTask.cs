using UnityEngine;

[System.Serializable]
public class GoToTask : Task {
	private System.Guid location;
	private SerializableVector3 position;
	private bool setIncompleteOnLeave;

	public GoToTask(System.Guid location, Vector3 position, bool setIncompleteOnLeave) {
		this.location = location;
		this.position = new SerializableVector3(position);
		this.setIncompleteOnLeave = setIncompleteOnLeave;
	}

	public override bool complete {
		get { return false; }
	}

	public override TaskDestination[] GetLocations() {
		return new TaskDestination[] { new TaskDestination(location, position.val) };
	}
}