using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public abstract class Quest {
	private List<Task> completedTasks;
	public abstract bool complete {
		get;
	}
}