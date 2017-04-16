using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ParallelTasksTask : Task {
	private Task[] tasks;

	public ParallelTasksTask(params Task[] tasks) {
		this.tasks = tasks.ToArray();
	}

	public override bool complete {
		get { return tasks.All(x => x.complete); }
	}

	public override TaskDestination[] GetLocations() {
		List<TaskDestination> all = new List<TaskDestination>();
		foreach (Task t in tasks) {
			all.AddRange(t.GetLocations());
		}
	}
}