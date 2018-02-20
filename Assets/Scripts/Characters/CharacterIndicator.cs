using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIndicator : MonoBehaviour {

	private TextObject text;

	private Task.TaskDestination taskMarker;
	public bool hasQuestsToGive;

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

	public void UpdateQuestsToGive(bool hasQuestsToGive) {
		if (this.hasQuestsToGive != hasQuestsToGive) {
			this.hasQuestsToGive = hasQuestsToGive;
			UpdateDisplay();
		}
	}

	public void UpdateDisplay() {
		if (taskMarker != null) {  // no current task for a quest
			text.Say(taskMarker.icon, color: Quest.QUEST_MARKER_COLOR, permanent: true);
		} else if (hasQuestsToGive) {
			text.Say("!", color: "yellow", permanent: true);				
		} else {
			text.Clear();
		}
	}

	public bool ShowOffScreenMarker() {
		return taskMarker != null;
	}
}
