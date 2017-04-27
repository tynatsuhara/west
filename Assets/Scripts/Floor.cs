using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour, Damageable {

	public bool restricted;
	public bool kickUpDirt;
	public Color32 dirtColor;  // the color that should be kicked up. White = set to biome color

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool playerAttacker = false, DamageType type = DamageType.BULLET) {
		return true;
	}
}
