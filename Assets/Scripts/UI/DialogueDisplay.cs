using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour {

	// character display
	public Vector3 rotation;
	public float leftRightRotation;
	public float sidePadding;
	public float bottomPadding;
	public float scale;
	public float playerDepth;

	// background
	public RectTransform tint;
	public float tintPadding;
	public float tintShift;
	public Text textDisplay;
	
	// ui
	private bool onScreen;
	private GameObject displayedCharacter;
	private OffScreenSlide slide;

	// dialogue
	private Dialogue dialogue;
	private Player player;
	private NPC npc;

	public void Start() {
		slide = GetComponent<OffScreenSlide>();
	}

	public void Hide() {
		onScreen = false;
		slide.MoveOffScreen();
		npc.CancelDialogue();		
	}

	public void StartDialogue(Dialogue dialogue, Player player, NPC npc) {
		this.dialogue = dialogue;
		this.player = player;
		this.npc = npc;
		this.onScreen = true;
		Refresh();
		slide.MoveOnScreen();
	}

	public void Refresh() {
		Dialogue.DialogueFrame frame = dialogue.GetCurrentFrame();
		DisplayCharacter(frame.player ? player as Character : npc, !frame.player);
		DisplayText(frame.text);
		DisplayOptions(frame.options);
	}

	private void DisplayText(string text) {
		
	}

	private void DisplayOptions(List<DialogueOption> options) {
		GetComponent<DialogueMenu>().LoadDialogue(dialogue, this, options);
	}

	private void DisplayCharacter(Character c, bool onRight) {
		if (displayedCharacter != null)
			Destroy(displayedCharacter);

		GameObject clone = displayedCharacter = Instantiate(c.gameObject, c.transform.position, c.transform.rotation);
		Destroy(clone.GetComponent<Character>());
		Destroy(clone.GetComponent<Rigidbody>());
		Destroy(clone.GetComponentInChildren<AudioListener>());
		clone.transform.SetParent(transform);

		// position clone
		clone.GetComponent<NavMeshAgent>().enabled = false;
		RectTransform rect = clone.AddComponent(typeof(RectTransform)) as RectTransform;
		rect.anchorMin = rect.anchorMax = rect.pivot = onRight ? Vector2.right : Vector2.zero;
		rect.anchoredPosition3D = new Vector3((onRight ? -1 : 1) * sidePadding, bottomPadding, playerDepth);
		rect.localScale = Vector3.one * scale;
		rect.localEulerAngles = rotation;
		rect.transform.RotateAround(rect.transform.position, rect.transform.up, (onRight ? 1 : -1) * leftRightRotation);
	}

	public bool IsShowing() {
		return onScreen;
	}

	public void FinishConvo() {
		npc.FinishDialogue();
	}
}
