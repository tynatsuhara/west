using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIndicator : MonoBehaviour {

	private TextObject text;

	private Task.TaskDestination taskMarker;
	public Dialogue highestPriorityDialogue;
	private Vector3 startPos;

	public void Awake() {
		text = GetComponent<TextObject>();
		startPos = transform.localPosition;
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

	public void UpdateVerticalPositioning(bool displayingSpeech) {
		transform.localPosition = startPos + new Vector3(0, displayingSpeech ? 1 : 0, 0);
	}

	// d should be the highest priority dialogue or null to clear
	public void UpdateDialogueIndicator(Dialogue d) {
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
