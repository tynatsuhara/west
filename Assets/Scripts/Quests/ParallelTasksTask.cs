using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ParallelTasksTask : Task {
	private Task[] tasks;

	public override bool complete {
		get { return tasks.All(x => x.complete); }
	}
	public override string message {
		get { return tasks.Select(x => x.message).Aggregate((s1, s2) => s1 + "\n" + s2); }
	}

	public ParallelTasksTask(params Task[] tasks) {
		this.tasks = tasks.ToArray();
	}

	public override TaskDestination[] GetLocations() {
		List<TaskDestination> all = new List<TaskDestination>();
		foreach (Task t in tasks) {
			all.AddRange(t.GetLocations());
		}
		return all.ToArray();
	}
}