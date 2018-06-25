using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float speed;

	// set by weapon
	public float damage;
	public float range;
	public Character shooter;
	public bool shooterIsPlayer;

	private float distTraveled;

	void Update () {
		float dist = speed * Time.deltaTime;
		transform.Translate(transform.forward * dist);
		distTraveled += dist;
		if (distTraveled >= range) {
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision other) {
		if (other.transform.root == shooter.transform.root)
			return;
		Damageable damageScript = other.transform.GetComponentInParent<Damageable>();
		if (damageScript == null)
			return;
		LivingThing c = other.transform.GetComponentInParent<LivingThing>();
		if (shooterIsPlayer && c != null && c.isAlive && (!(c is Player) || GameManager.instance.friendlyFireEnabled)) {
			(shooter as Player).playerUI.HitMarker();
		}
		if (damageScript.Damage(other.contacts[0].point, transform.forward, damage, shooter)) {
			Destroy(gameObject);
		}
		GameManager.instance.AlertInRange(Stimulus.VIOLENCE, transform.position, 10f, visual: other.transform.root.gameObject, alerter: shooter);	
	}
}
