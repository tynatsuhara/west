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
		public string icon;

		public TaskDestination(System.Guid location, Vector3 position, string icon) {
			this.location = location;
			this.position = position;
			this.character = System.Guid.Empty;
			this.icon = icon;
		}

		public TaskDestination(System.Guid character, string icon) {
			this.location = SaveGame.currentGame.savedCharacters[character].location;
			this.character = character;
			this.icon = icon;
		}
	}
}