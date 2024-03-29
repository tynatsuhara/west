﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : Weapon {

	public GameObject thrownPrefab;
	public Vector3 thrownSpawnPoint;
	public float throwRate;

	private bool canThrow = true;
	public const int THROWN_FRAME = 3;

	public override bool Shoot(Vector3 target) {
		if (meleeing || !canThrow)
			return false;

		// Spawn projectile
		GetComponent<GunAnimation>().Shoot(THROWN_FRAME);
		GameObject go = Instantiate(thrownPrefab, transform.position, transform.rotation);
		go.transform.parent = transform;
		go.transform.localPosition = thrownSpawnPoint;
		go.transform.parent = null;
		go.GetComponent<ThrownWeaponInstance>().direction = (target - transform.root.position).normalized;
		go.GetComponent<ThrownWeaponInstance>().thrower = this;
		foreach (Collider characterCollider in owner.transform.root.GetComponentsInChildren<Collider>())
			foreach (Collider weaponCollider in go.GetComponentsInChildren<Collider>())
				Physics.IgnoreCollision(weaponCollider, characterCollider, true);

		StartCoroutine(Reset());
		return true;
	}
	private IEnumerator Reset() {
		canThrow = false;
		yield return new WaitForSeconds(throwRate);
		volume.SetFrame(GUN_BASE_FRAME);
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

	public override void Melee(DamageType type = DamageType.MELEE, int dir = 0) {
		if (meleeing || !canThrow)
			return;
		volume.SetFrame(ANIM_START_FRAME);
		StartCoroutine(ResetFrame());
		base.Melee(type, -1);
	}
	private IEnumerator ResetFrame() {
		yield return new WaitForSeconds(.3f);
		volume.SetFrame(GUN_BASE_FRAME);
	}

	// We don't actually need to save any state
	public override System.Object SaveData() {
		return null;
	}
	public override void LoadSaveData(System.Object saveData) {}
}
