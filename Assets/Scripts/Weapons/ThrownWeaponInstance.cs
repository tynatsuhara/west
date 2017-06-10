using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeaponInstance : MonoBehaviour {

	public ThrownWeapon thrower;
	public float damage;
	public float dropTime;
	public float angle;
	public float force;
	public Vector3 direction;

	private float startTime;

	void Start() {
		startTime = Time.time;
		GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);		
	}

	void Update() {
		transform.RotateAround(transform.position, transform.right, angle * Time.deltaTime);
		if (Time.time - startTime >= dropTime) {
			GetComponent<Rigidbody>().useGravity = true;
			this.enabled = false;
		}
	}

	void OnCollisionEnter(Collision collision) {
		this.enabled = false;
		GetComponent<Rigidbody>().useGravity = true;		
		// stop from glitching out (weird scaling issues)
		if (collision.transform.root.tag == "Ground" || collision.transform.root.tag == "Wall")
			return;			
		GetComponent<Rigidbody>().isKinematic = true;
		transform.parent = collision.transform;
		foreach (var c in GetComponentsInChildren<Collider>()) {
			c.enabled = false;
		}
		Damageable d = collision.transform.GetComponentInParent<Damageable>();
		if (d != null)
			d.Damage(collision.contacts[0].point, direction, damage, thrower.isPlayer, DamageType.RANGED);
		if (thrower.isPlayer && collision.transform.GetComponentInParent<LivingThing>() != null)
			thrower.player.playerUI.HitMarker();
	}
}
