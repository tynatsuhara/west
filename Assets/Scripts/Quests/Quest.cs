using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public abstract class Quest {
	private List<Task> tasks;	
	public bool complete {
		get { return tasks.All(x => x.complete || x.optional); }	
	}
}