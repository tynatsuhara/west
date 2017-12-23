using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public abstract class Quest {
	public System.Guid guid = System.Guid.NewGuid();
	public bool active;
	public bool complete;
	public bool failed;
	public string title;
	protected List<string> completedTaskMessages = new List<string>();

	// returns null if the quest is complete
	public abstract Task UpdateQuest();
}