﻿using System;
using UnityEngine;

public interface Damageable {

	// The return value is used for projectile damage. If the bullet should go
	// through the object and continue, return true. Otherwise return false.
	bool Damage(Vector3 location, Vector3 angle, float damage, Character attacker = null, DamageType type = DamageType.BULLET);
}

public enum DamageType {
	BULLET,
	MELEE,
	EXPLOSIVE,
	SLICE,
	NONLETHAL,
	RANGED  // tomahawks, knives, etc
}