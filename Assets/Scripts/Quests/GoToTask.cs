using UnityEngine;
using System.Linq;

[System.Serializable]
public class GoToTask : Task {
	private System.Guid location;
	private SerializableVector3 position;
	private bool setIncompleteOnLeave;
	private float radius;
	private string message_;
	
	private bool wasThere;

	public override bool complete {
		get {
			bool near = Map.CurrentLocation().guid == location && GameManager.players.Any(x => Vector3.Distance(x.transform.position, position.val) < radius);
			if (setIncompleteOnLeave) {
				return near;
			} else {
				wasThere = wasThere || near;
				return wasThere;
			}
		}
	}

	public override string message {
        get { return message_; }
    }

	public GoToTask(System.Guid location, Vector3 position, bool setIncompleteOnLeave = false, string message = "", float radius = 2.5f) {
		if (message == "") {
			message = "Go to " + Map.Location(location).name;
		}
		this.location = location;
		this.position = new SerializableVector3(position);
		this.setIncompleteOnLeave = setIncompleteOnLeave;
		this.message_ = message;
		this.radius = radius;
	}

	public override TaskDestination[] GetLocations() {
		return new TaskDestination[] { new TaskDestination(location, position.val) };
	}
}