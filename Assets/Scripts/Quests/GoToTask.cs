using UnityEngine;
using System.Linq;

[System.Serializable]
public class GoToTask : Task {
	private System.Guid location;
	private SerializableVector3 position;
	private bool setIncompleteOnLeave;
	private float radius;
	private string message_;
	public override string message {
        get { return message_; }
    }

	public GoToTask(System.Guid location, Vector3 position, bool setIncompleteOnLeave = false, string message = "", float radius = 1f) {
		if (message == "") {
			message = "Go to " + Map.Location(location).name;
		}
		this.location = location;
		this.position = new SerializableVector3(position);
		this.setIncompleteOnLeave = setIncompleteOnLeave;
		this.message_ = message;
		this.radius = radius;
	}

	public override bool complete {
		get { return Map.CurrentLocation().guid == location && SaveGame.currentGame.savedPlayers.Any(x => Vector3.Distance(x.position.val, position.val) < radius); }
	}

	public override TaskDestination[] GetLocations() {
		return new TaskDestination[] { new TaskDestination(location, position.val) };
	}
}