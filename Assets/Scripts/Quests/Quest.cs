using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public abstract class Quest {
	public System.Guid guid = System.Guid.NewGuid();
	protected List<Task> completedTasks = new List<Task>();
	protected List<Task> tasks = new List<Task>();

	public bool complete {
		get { return tasks.Count == 0; }
	}

	public virtual void Tick() {}

	public Task.TaskDestination[] GetLocations() {
		return complete ? new Task.TaskDestination[0] : tasks[0].GetLocations();
	}
}