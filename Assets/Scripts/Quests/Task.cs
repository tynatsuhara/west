using UnityEngine;

[System.Serializable]
public abstract class Task {
	public bool optional;
	public abstract bool complete {
		get;
	}
	public abstract string message {
		get;
	}

	public virtual TaskDestination[] GetLocations() {
		return new TaskDestination[]{};
	}

	public class TaskDestination {
		public System.Guid location;
		public Vector3 position;
		public System.Guid character;

		public TaskDestination(System.Guid location, Vector3 position) {
			this.location = location;
			this.position = position;
			this.character = System.Guid.Empty;
		}

		public TaskDestination(System.Guid character) {
			this.location = SaveGame.currentGame.savedCharacters[character].location;
			this.character = character;
		}
	}
}