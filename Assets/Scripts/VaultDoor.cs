﻿using UnityEngine;

public class VaultDoor : MonoBehaviour, Interactable, Powerable {

	public Transform axis;
	public Transform door;
	public float openSpeed = 1.5f;
	public float openAngle = 90f;
	public float thermiteTime = 10f;
	public PicaVoxel.Exploder exploder;

	private bool burning;
	private bool opening;

	void Update() {
		if (GameManager.paused) {
			return;
		}

		if (burning)
			Burn();

		if (opening)
			ContinueOpen();
	}

	private void Burn() {
		thermiteTime -= Time.deltaTime;
		if (thermiteTime <= 0) {
			exploder.Explode();
			Open();
		}
	}
	
	public void Open() {
		if (opening)
			return;
		
		opening = true;
	}
	private void ContinueOpen() {
		float dist = Time.deltaTime * openSpeed;
		door.RotateAround(axis.position, Vector3.up, dist);
		openAngle -= dist;
		if (openAngle <= 0) {
			this.enabled = false;
		}
	}

	public void Interact(Character character, string action) {
		if (!character.inventory.Has(Inventory.Item.THERMITE))
			return;

		character.inventory.Remove(Inventory.Item.THERMITE);
		burning = true;
	}

	public void Uninteract(Character character) {}

	public InteractAction[] GetActions(Character character) {
		return new InteractAction[] {
			new InteractAction("Open", (transform.position - character.transform.position).magnitude < 2f && character.CanSee(gameObject))
		};
	}

	public void Power() {
		Invoke("Open", 2f);
	}
	public void Unpower() {}
}
