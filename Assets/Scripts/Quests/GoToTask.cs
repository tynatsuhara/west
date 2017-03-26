using UnityEngine;

[System.Serializable]
public class GoToTask : Task {
	private System.Guid location;
	private SerializableVector3 position;

	public GoToTask(System.Guid location, Vector3 position) {
		this.location = location;
		this.position = new SerializableVector3(position);
	}

	public override bool complete {
		get { return false; }
	}
}