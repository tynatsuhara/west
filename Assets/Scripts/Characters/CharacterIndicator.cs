using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIndicator : MonoBehaviour {

	private TextObject text;

	private Task.TaskDestination taskMarker;
	public Dialogue highestPriorityDialogue;

	public void Awake() {
		text = GetComponent<TextObject>();
	}

	public void Start() {
		UpdateDisplay();		
	}

	public void MarkForQuest(Task.TaskDestination taskMarker) {
		this.taskMarker = taskMarker;
		UpdateDisplay();
	}

	public void UnmarkForQuest() {
		this.taskMarker = null;
		UpdateDisplay();
	}

	public void UpdateDialogueIndicator(SortedList<int, Dialogue> dialogues) {
		Dialogue d = dialogues.Count > 0 ? dialogues.Values[0] : null;
		if (highestPriorityDialogue != d) {
			highestPriorityDialogue = d;
			UpdateDisplay();
		}
	}

	public void UpdateDisplay() {
		if (taskMarker != null) {  // no current task for a quest
			text.Say(taskMarker.icon, color: Quest.QUEST_MARKER_COLOR, permanent: true);
		} else if (highestPriorityDialogue != null) {
			text.Say(highestPriorityDialogue.icon, color: highestPriorityDialogue.color, permanent: true);				
		} else {
			text.Clear();
		}
	}

	public bool ShowOffScreenMarker() {
		return taskMarker != null;
	}
}
