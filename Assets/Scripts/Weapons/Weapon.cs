using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour {

	public float damage;
	public float range;
	public int maxEnemiesMelee = 1;
	public Vector3 inPlayerPos;
	public Collider droppedCollider;

	public GameObject projectilePrefab;
	public Vector3 projectileSpawnOffset;

	public Character owner;	
	public PicaVoxel.Volume volume;	
	public GunAnimation anim;
	protected bool enqueuedReload = false;

	// Gun frames
	public const int GUN_BASE_FRAME = 0;
	public const int DROPPED_GUN_FRAME = 1;	 
	public const int ANIM_START_FRAME = 2;

	public bool isPlayer;
	public Player player;

	public virtual void Awake() {
		owner = GetComponentInParent<Character>();
		volume = GetComponent<PicaVoxel.Volume>();
		anim = GetComponent<GunAnimation>();
	}

	// returns true if the attack executed that frame (ie gun fired)
	abstract public bool Shoot(Vector3 target);
	abstract public void Release();
	abstract public void Reload();
	abstract public void CancelReload();
	abstract public bool NeedsToReload();
	abstract public void UpdateUI();

	// Used for saving/loading game
	abstract public System.Object SaveData();
	abstract public void LoadSaveData(System.Object saveData);

	public void ProjectileShoot() {
		Vector3 pos = owner.transform.TransformPoint(projectileSpawnOffset);
		Projectile projectile = Instantiate(projectilePrefab, pos, Quaternion.identity).GetComponent<Projectile>();
		projectile.transform.RotateAround(projectile.transform.position, Vector3.up, owner.transform.eulerAngles.y/2);
		projectile.damage = damage;
		projectile.range = range;
		projectile.shooter = owner;
		projectile.shooterIsPlayer = isPlayer;
	}	

	public void RaycastShoot(Vector3 source, Vector3 direction) {
		source.y = Mathf.Min(source.y, 1.2f);
		Debug.DrawLine(source, source + direction * range, Color.red, 1f, true);
		RaycastHit[] hits = Physics.RaycastAll(source, direction, range)
			.Where(h => h.transform.root != transform.root)
			.OrderBy(h => h.distance)
			.ToArray();
		bool keepGoing = true;
		bool hitMarker = false;
		for (int i = 0; i < hits.Length && keepGoing; i++) {
			Damageable damageScript = hits[i].transform.GetComponentInParent<Damageable>();
			if (damageScript == null)
				break;
			if (!hitMarker) {
				LivingThing c = hits[i].transform.GetComponentInParent<LivingThing>();
				hitMarker = c != null && c.isAlive && (!(c is Player) || GameManager.instance.friendlyFireEnabled);
			}
			keepGoing = damageScript.Damage(hits[i].point, direction.normalized, damage, owner);
			GameManager.instance.AlertInRange(Stimulus.VIOLENCE, transform.position, 10f, visual: hits[i].transform.root.gameObject, alerter: owner);			
		}
		if (hitMarker && isPlayer) {
			player.playerUI.HitMarker();
		}
	}

	public virtual void Drop(Vector3 force) {
		CancelInvoke();
		volume.SetFrame(DROPPED_GUN_FRAME);
		droppedCollider.enabled = true;
		transform.parent = null;
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * Random.Range(10f, 100f), ForceMode.Force);
		owner = null;
	}

	public bool meleeing;
	private float meleeDirection;
	public virtual void Melee(DamageType type = DamageType.MELEE, int dir = 0) {
		if (meleeing || delayed)
			return;
		meleeDirection = dir != 0 ? Mathf.Clamp(dir, -1, 1) : Random.Range(0, 2) * 2 - 1;
		meleeing = true;
		CancelReload();  // interrupt reloading to melee, if necessary
		StartCoroutine("MeleeAnimation");
		int range = 8;
		List<Damageable> hits = new List<Damageable>();
		for (int i = -range/2; i <= range/2; i++) {
			// Debug.DrawLine(owner.transform.position, 
			// 		owner.transform.position + owner.transform.forward * 1.3f + meleeDirection * i * owner.transform.right / 5f, 
			// 		Color.red, 5f);
			RaycastHit[] rhits = Physics.RaycastAll(owner.transform.position, owner.transform.forward * 1.3f + meleeDirection * i * owner.transform.right / 5f, 1.5f)
					.Where(h => h.transform.root != transform.root && h.collider.GetComponentInParent<Damageable>() != null)
					.OrderBy(h => h.distance)
					.ToArray();
			if (rhits.Length > 0) {
				if (hits.Contains(rhits[0].collider.GetComponentInParent<Damageable>()))
					continue;
				if (isPlayer)
					player.playerUI.HitMarker();
				Damageable d = rhits[0].collider.GetComponentInParent<Damageable>();
				d.Damage(rhits[0].collider.transform.root.position, owner.transform.forward, 1f, owner, type);
				if (rhits[0].collider.GetComponentInParent<LivingThing>() != null)
					MeleeHitPlayerCallback();
				GameManager.instance.AlertInRange(Stimulus.VIOLENCE, transform.position, 10f, visual: rhits[0].transform.root.gameObject, alerter: owner);
				hits.Add(d);
			}
			if (hits.Count == maxEnemiesMelee)
				break;
		}
	}
	private IEnumerator MeleeAnimation() {
		int angle = 40;
		transform.localPosition = inPlayerPos;
		transform.localEulerAngles = Vector3.up * 180;
		Quaternion initialRotation = transform.localRotation;
		Vector3 initialPosition = transform.localPosition;
		transform.RotateAround(transform.parent.position, Vector3.up, angle * meleeDirection);
		Quaternion end = transform.localRotation;
		transform.RotateAround(transform.parent.position, Vector3.up, -2.4f * angle * meleeDirection);
		float diff = 100f;			
		while (diff > .03f) {
			Vector3 nextRot = Quaternion.Lerp(transform.localRotation, end, .3f).eulerAngles;
			diff = Mathf.Abs(nextRot.y - transform.localRotation.eulerAngles.y);
			transform.RotateAround(transform.parent.position, Vector3.up, diff * meleeDirection);
			yield return new WaitForSeconds(.01f);
		}
		diff = 100f;
		end = initialRotation;
		while (diff > .05f) {
			Vector3 nextRot = Quaternion.Lerp(transform.localRotation, end, .4f).eulerAngles;
			diff = Mathf.Abs(transform.localRotation.eulerAngles.y - nextRot.y);
			transform.RotateAround(transform.parent.position, Vector3.up, -diff * meleeDirection);
			yield return new WaitForSeconds(.01f);
		}
		transform.localRotation = initialRotation;
		transform.localPosition = initialPosition;

		meleeing = false;
		if (enqueuedReload) {
			Reload();
		}
	}
	protected virtual void MeleeHitPlayerCallback() {}

	public void SetLoweredPosition(bool lowered) {
		transform.localPosition = inPlayerPos + (Vector3.down + Vector3.back) * (lowered ? .1f : 0f);
	}

	protected bool delayed = false;
	public void DelayAttack(float delay) {
		CancelInvoke("UnDelay");
		delayed = true;
		SetLoweredPosition(true);
		Invoke("UnDelay", delay);
	}
	
	private void UnDelay() {
		SetLoweredPosition(false);		
		delayed = false;
	}

	public void PlayerEffects(float power, float duration) {
		if (isPlayer) {
			GetComponentInParent<Player>().playerCamera.Shake(power, duration);
		}
	}
}
