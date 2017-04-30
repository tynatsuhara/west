using UnityEngine;

[System.Serializable]
public abstract class Task {
	public bool optional;
	public abstract bool complete {
		get;
	}

	public abstract TaskDestination[] GetLocations();

	public class TaskDestination {
		public System.Guid location;
		public Vector3 position;

		public TaskDestination(System.Guid location, Vector3 position) {
			this.location = location;
			this.position = position;
		}
	}
}