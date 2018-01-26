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

	// todo: make different classes inheriting TaskDestination
	public class TaskDestination {
		public System.Guid location {
			get { return character == System.Guid.Empty ? location_ : SaveGame.currentGame.savedCharacters[character].location; }
		}
		private System.Guid location_;
		public Vector3 position;
		public System.Guid character;
		public string icon;

		public TaskDestination(System.Guid location, Vector3 position, string icon) {
			this.location_ = location;
			this.position = position;
			this.character = System.Guid.Empty;
			this.icon = icon;
		}

		public TaskDestination(System.Guid character, string icon) {
			this.character = character;
			this.icon = icon;
		}
	}
}