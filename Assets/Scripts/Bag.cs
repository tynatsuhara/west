using UnityEngine;
using System.Collections.Generic;

public class Bag : MonoBehaviour, Interactable {

	private bool onGround;
	public float speedMultiplier = .75f;
	public Collider collider;
	public string lootCategory;
	public int dollarAmount;
	private Character holder;

	void Start() {
		SetOnGround(true);
		GetComponent<Rigidbody>().mass = (1 - speedMultiplier) * 180 + 5;
	}

	public InteractAction[] GetActions(Character character) {
		return new InteractAction[] {
			new InteractAction("Pick up", (transform.position - character.transform.position).magnitude < 2f && character.CanSee(gameObject))
		};
	}

	public void Interact(Character character, string action) {
		if (character.hasBag || (holder != null && holder.isAlive))
			return;

		if (holder != null)
			holder.DropBag(false);
		holder = character;
		SetOnGround(false);
		character.AddBag(this);
	}

	public void Uninteract(Character character) {}

	public void DropBag() {
		holder = null;
		SetOnGround(true);
	}

	private void SetOnGround(bool onGround) {
		GetComponent<Rigidbody>().isKinematic = !onGround;
		GetComponent<PicaVoxel.Volume>().SetFrame(onGround ? 0 : 1);
		collider.enabled = onGround;
		this.onGround = onGround;
	}

	public void SaveLoot() {
		GameManager.instance.AddLoot(lootCategory, dollarAmount);
		gameObject.SetActive(false);
	}
}
