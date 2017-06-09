using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : Weapon {

	public float throwRate;
	public float dropTime;
	public float speed;

	private bool canThrow = true;

	public override bool Shoot(Vector3 target) {
		if (!canThrow)
			return false;

		StartCoroutine(Throw());
		StartCoroutine(Reset());
		return true;
	}
	private IEnumerator Throw() {
		yield return new WaitForSeconds(dropTime);
	}
	private IEnumerator Reset() {
		canThrow = false;
		yield return new WaitForSeconds(throwRate);
		canThrow = true;
	}

	public override void Release() {}
	public override void Reload() {}
	public override void CancelReload() {}
	public override bool NeedsToReload() {
		return false;
	}
	public override void UpdateUI() {
		player.playerUI.UpdateAmmo(name);
	}

	// We don't actually need to save any state
	public override System.Object SaveData() {
		return null;
	}
	public override void LoadSaveData(System.Object saveData) {}
}
