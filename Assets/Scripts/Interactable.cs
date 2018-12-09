using System;

/**
 * Something which is interactable is something which can be used
 * by a character. Basically, anything the player can walk up to
 * and press 'use'.
 */
public interface Interactable {

	void Interact(Character character, string action);
	void Uninteract(Character character);
	InteractAction[] GetActions(Character character);
}

public class InteractAction {
	public String action;
	public bool enabled;

	public InteractAction(String action, bool enabled) {
		this.action = action;
		this.enabled = enabled;
	}
}