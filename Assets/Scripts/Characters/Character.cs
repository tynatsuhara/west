using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour, Damageable {

	// Ranked in order of ascending priority
	public LayerMask sightLayers;
	public TextObject speech;
	
	protected Rigidbody rb;
	protected LivingThing lt;

	public System.Guid guid;  // null the first time a unique character is spawned
	protected bool firstSetup {
		get { return guid == System.Guid.Empty; }
	}
	public WalkCycle walk;

	// each inheriting class should define walking
	public bool walking;

	public Inventory inventory;
	public Bag bag;
	public bool hasBag {
		get { return bag != null; }
	}

	public bool isAlive {
		get { return GetComponent<LivingThing>().health > 0; }
	}

	// customization stuff
	private bool female;
	private string outfit;
	private byte skinColor;
	private byte hairColor;
	private byte hairStyle;
	private byte accessory;

	public PicaVoxel.Volume head;
	public PicaVoxel.Volume body {
		get { return female ? bodyF : bodyM; }
	}
	public PicaVoxel.Volume bodyM;
	public PicaVoxel.Volume bodyF;
	public PicaVoxel.Volume arms;
	public PicaVoxel.Volume legs;
	private PicaVoxel.Volume[] volumesToSave {
		get { return new PicaVoxel.Volume[]{ head, body }; }
	}
	private List<List<byte[]>> voxelBlobs;

	public float moveSpeed;
	public float rotationSpeed;
	public Vector3 lastMoveDirection;

	public GameObject[] guns;
	protected Weapon currentGun;
	public byte sidearmId;
	public byte weaponId;
	protected byte gunIndex = 0;
	public PicaVoxel.Exploder exploder;
	public Explosive explosive;
	public byte zipties;
	protected bool weaponDrawn_;
	public bool weaponDrawn {
		get { return weaponDrawn_; }
	}

	protected bool hasLookTarget = false;
	public Transform lookTarget;
	public Vector3 lookPosition;

	public Rigidbody draggedBody;
	public bool isDragging {
		get { return draggedBody != null; }
	}

	public System.Guid killedBy;
	public List<string> groups = new List<string>();

	public virtual void Awake() {
		rb = GetComponent<Rigidbody>();
		lt = GetComponent<LivingThing>();
	}

	public virtual void Start() {
		lt.separateBodyParts.Add(body);
		lt.bodyParts.Add(body);
		lt.bodyParts.Add(head);

		Accessory[] accs = new Accessory[] {		
			CharacterOptionsManager.instance.hairstyles[hairStyle],
			CharacterOptionsManager.instance.accessories[accessory],
		};
		GetComponent<CharacterCustomization>().ColorCharacter(outfit, skinColor, hairColor, accs);

		if (voxelBlobs != null) {
			var vols = volumesToSave;
			for (int i = 0; i < vols.Length; i++) {
				vols[i].SetBytes(voxelBlobs[i]);
			}
		}

		StartCoroutine(AlertStuff());
	}

	private IEnumerator AlertStuff() {
		while (true) {
			if (isAlive) {
				GameManager.instance.AlertInRange(Stimulus.JUST_CHILLIN, transform.position, 10f, visual: transform.root.gameObject, alerter: this);				
			}
			if (isAlive && weaponDrawn) {
				GameManager.instance.AlertInRange(Stimulus.GUN_DRAWN, transform.position, 10f, visual: transform.root.gameObject, alerter: this);
			} else if (!isAlive) {
				GameManager.instance.AlertInRange(Stimulus.DEAD_BODY, transform.position, 10f, visual: transform.root.gameObject);
			}
			yield return new WaitForSeconds(.5f);
		}
	}

	// is C a friend?
	// returns true if they share groups or have a rep >= rep
	public bool IsFriendsWith(Character c, float rep = (float) Reputation.Rank.LIKE) {
		if (c.groups.Intersect(groups).Count() > 0) {
			return true;
		}
		foreach (string g1 in groups) {
			Group g = SaveGame.currentGame.groups[g1];
			foreach (string g2 in c.groups) {
				if (g.GetReputationWith(g2).CurrentRep() >= rep) {
					return true;
				}
			}
		}
		return false;
	}

	// is C an enemy?
	// returns false if they share groups, or true if c has a rep <= rep
	public bool IsEnemiesWith(Character c, float rep = (float) Reputation.Rank.HATE) {
		if (c.groups.Intersect(groups).Count() > 0) {
			return false;
		}
		foreach (string g1 in groups) {
			Group g = SaveGame.currentGame.groups[g1];
			foreach (string g2 in c.groups) {
				if (g.GetReputationWith(g2).CurrentRep() <= rep) {
					return true;
				}
			}
		}
		return false;
	}

	public void LookAt(Transform target) {
		hasLookTarget = true;
		lookTarget = target;
	}

	public void LookAt(Vector3 target) {
		hasLookTarget = true;
		lookTarget = null;
		lookPosition = target;
	}

	public void LoseLookTarget() {
		hasLookTarget = false;
		lookTarget = null;
	}

	protected void Rotate() {
		if (lookTarget != null) {
			lookPosition = lookTarget.position;
			hasLookTarget = true;  // so we can drag a look target in the inspector
		}
		if (hasLookTarget) {
			lookPosition.y = transform.position.y;
			Vector3 vec = lookPosition - transform.position;
			if (vec == Vector3.zero)
				return;
			Quaternion targetRotation = Quaternion.LookRotation(vec);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		}
	}

	public virtual bool Damage(Vector3 location, Vector3 angle, float damage, Character attacker = null, DamageType type = DamageType.BULLET) {
		bool isPlayer = this is Player;

		if (!weaponDrawn)
			damage *= 2f;  // double damage on stealth kills (TODO: perk that makes this higher)

		if (isPlayer && (attacker is Player) && !GameManager.instance.friendlyFireEnabled) {
			damage = 0f;
		} else {
			lt.RegenDelay(damage);
		}

		if (isAlive)
			lt.Bleed(location - Vector3.up * (Random.Range(0, 3) == 1 ? .5f : 0f), Random.Range(1, 10), angle);

		exploder.transform.position = location + angle * Random.Range(-.1f, .15f) + new Vector3(0, Random.Range(-.7f, .3f), 0);			
		bool wasAlive = isAlive;  // save it beforehand

		lt.health = Mathf.Max(0, lt.health - damage);
		if (!isAlive && wasAlive)
			Die(location, angle, attacker, type);

		if (isAlive && !isPlayer)
			(this as NPC).Alert(Stimulus.ATTACKED, location - angle.normalized, attacker);

		// regular knockback
		// if (!isPlayer || !isAlive) {
			float forceVal = Random.Range(500, 900);
			if ((wasAlive && !isAlive) || type == DamageType.MELEE || type == DamageType.SLICE) {
				forceVal *= 1.5f;
			}
			foreach (PicaVoxel.Volume vol in lt.separateBodyParts) {
				Rigidbody body = vol.GetComponentInParent<Rigidbody>();
				body.AddForceAtPosition(forceVal * angle.normalized, type == DamageType.SLICE 
									? transform.position + Vector3.up * Random.Range(-.4f, .3f)  // make the head fly
									: exploder.transform.position, ForceMode.Impulse);
			}
		// }

		return !wasAlive;
	}

	public void Die() {		
		Die(transform.position, Vector3.one);
	}

	public virtual void Die(Vector3 location, Vector3 angle, Character attacker = null, DamageType type = DamageType.MELEE) {
		if (ridingHorse) {
			Dismount();
		}
		GameManager.instance.AlertInRange(Stimulus.MURDER, transform.position, 10f, visual: transform.root.gameObject, alerter: attacker);		
		InteractCancel();
		SetDeathPhysics();
		walk.StandStill();		

		if (weaponDrawn) {
			DropWeapon(angle * Random.Range(5, 10) + Vector3.up * Random.Range(2, 6));
		}

		if (type == DamageType.SLICE && Random.Range(0, 2) == 0) {
			Decapitate();
		}
		lt.DamageEffects(exploder, angle, type);

		UnMarkForQuest();
		if (Random.Range(0, 2) == 0) {
			speech.SayRandom(Speech.DEATH_QUOTES, showFlash: true);
		}

		if (!(this is Player) && (attacker is Player) && type != DamageType.NONLETHAL) {
			SaveGame.currentGame.stats.peopleKilled++;
		}
		if (attacker != null) {
			killedBy = attacker.guid;
			int bounty = SaveGame.currentGame.crime.CharacterBounty(guid);
			if (bounty == 0) {  // killed an innocent
				SaveGame.currentGame.crime.Commit(attacker.guid, Map.CurrentTown().guid, "Murder", 100);
			} else {
				// give them the bounty
			}
		}

		lt.FallOver(400);
		// Invoke("RemoveBody", 60f);
	}

	protected void SetDeathPhysics() {
		UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		if (agent != null)
			agent.enabled = false;
		UnityEngine.AI.NavMeshObstacle obstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();
		if (obstacle != null)
			obstacle.enabled = true;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
	}

	private void Decapitate() {
		head.transform.parent = null;
		Rigidbody b = head.gameObject.AddComponent<Rigidbody>() as Rigidbody;
		b.mass = rb.mass;
		lt.separateBodyParts.Add(head);
	}

	public void RemoveBody() {
		Destroy(gameObject);
	}

	public float CalculateSpeed() {
		float speed = moveSpeed;
		if (ridingHorse)
			return speed * 1.7f;
		if (draggedBody != null)
			speed *= .5f;
		if (hasBag)
			speed *= bag.speedMultiplier;
		return speed;
	}

	public void DrawWeapon() {
		SetWeaponDrawn(true);
	}

	public void HideWeapon() {
		SetWeaponDrawn(false);
	}

	public void DropWeapon(Vector3 force) {
		if (guns == null || currentGun == null)
			return;
		
		SetWeaponDrawn(false, true);
		currentGun.Drop(force);
		currentGun = null;
	}

	protected void SetWeaponDrawn(bool drawn, bool weaponDropped = false) {
		if (drawn == weaponDrawn_)
			return;

		weaponDrawn_ = drawn;
		arms.gameObject.SetActive(!drawn);
		currentGun = guns[gunIndex].GetComponent<Weapon>();

		if (!weaponDropped)
			guns[gunIndex].SetActive(drawn);
	}

	public virtual void Shoot() {
		Shoot(transform.position + transform.forward);
	}
	public virtual void Shoot(Vector3 target) {
		if (weaponDrawn_ && currentGun != null && !isDragging) {
			currentGun.Shoot(target);
		} 
	}

	public void Reload() {
		if (weaponDrawn_ && currentGun != null && !isDragging) {
			currentGun.Reload();
		} 
	}

	public void Melee() {
		if (weaponDrawn_ && currentGun != null && !isDragging) {
			currentGun.Melee();
		}
	}

	public void TriggerExplosive() {
		if (weaponDrawn_ && currentGun != null && !isDragging && explosive != null) {		
			explosive.Trigger();
		}
	}

	protected Interactable currentInteractScript;
	public void Interact() {
		if (currentInteractScript != null) {
			currentInteractScript.Interact(this);
			return;
		}

		float interactDist = 1.8f;
		float interactStep = .1f;
		// look straight forward, then downwards if you don't see anything
		for (float i = 0; i < interactDist - interactStep * 5; i += interactStep) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, 
								(transform.forward * (interactDist - i) - transform.up * i), 
								out hit, (1 + i * .7f), sightLayers)) {
				currentInteractScript = hit.collider.GetComponentInParent<Interactable>();
				if (currentInteractScript != null) {
					currentInteractScript.Interact(this);
					return;
				}
			}
		}
	}
	public void InteractCancel() {
		if (currentInteractScript != null) {
			currentInteractScript.Uninteract(this);
			currentInteractScript = null;
		}
	}

	public bool ridingHorse;
	public Horse mount;
	public void MountHorse(Horse h) {
		if (ridingHorse)
			return;
		
		mount = h;
		SetMount(h, true);
		LevelBuilder.instance.permanent.Add(h.transform);
		transform.parent = h.transform;		
		transform.localPosition = new Vector3(0f, .82f, .2f);  // horseback position
		walk.Sit();
	}
	public void Dismount() {
		SetMount(mount, false);
		mount.Dismount();
		transform.parent = null;
		LevelBuilder.instance.permanent.Remove(mount.transform);		
		transform.Translate((Random.Range(0, 2) == 0 ? 1 : -1) * mount.transform.right * .7f);
		walk.StandStill(true);
	}
	private void SetMount(Horse h, bool isMounted) {
		Collider[] hc = h.GetComponentsInChildren<Collider>();
		Collider[] pc = GetComponentsInChildren<Collider>();
		foreach (Collider horseCollider in hc)
			foreach (Collider myCollider in pc)
				Physics.IgnoreCollision(horseCollider, myCollider, isMounted);
		GetComponent<Rigidbody>().isKinematic = isMounted;
		ridingHorse = isMounted;
	}

	public void SpawnGuns() {
		if (weaponId == 0 && sidearmId == 0)
			return;

		if (guns == null || guns.Length == 0) {
			guns = new GameObject[] {
				weaponId > 0 ? CharacterOptionsManager.instance.weapons[weaponId] : null,
				sidearmId > 0 ? CharacterOptionsManager.instance.sidearms[sidearmId] : null,
			};
		}
		
		List<PicaVoxel.Volume> gunVolumes = new List<PicaVoxel.Volume>();

		for (int i = guns.Length - 1; i >= 0; i--) {
			if (guns[i] == null)
				continue;
			guns[i].SetActive(false);
			GameObject gun = guns[i] = Instantiate(guns[i]) as GameObject;
			gun.name = gun.name.Replace("(Clone)", "");
			gunVolumes.AddRange(guns[i].GetComponentsInChildren<PicaVoxel.Volume>());
			currentGun = gun.GetComponent<Weapon>();
			currentGun.isPlayer = this is Player;
			gun.transform.parent = transform;
			gun.transform.localPosition = currentGun.inPlayerPos;
			gun.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
			if (currentGun.isPlayer)
				currentGun.player = (Player) this;
		}

		GetComponent<CharacterCustomization>().body = body;
		GetComponent<CharacterCustomization>().head = head;
		GetComponent<CharacterCustomization>().legs = walk.GetComponent<PicaVoxel.Volume>();
		GetComponent<CharacterCustomization>().arms = arms;
		GetComponent<CharacterCustomization>().gunz = gunVolumes.ToArray();
		currentGun = null;
		SelectGun(gunIndex);
	}

	public void SelectGun(byte index) {
		index = (byte) Mathf.Clamp(index, 0, guns.Length);
		if (guns[index] == null)
			return;
		if (this is Player)
			guns[index].GetComponent<Weapon>().UpdateUI();
		if (gunIndex == index || (guns[gunIndex] != null && guns[gunIndex].GetComponent<Weapon>().meleeing)) {
			return;
		} else if (!weaponDrawn) {
			gunIndex = index;
			return;
		}
		float totalSwapTime = .3f;
		currentGun.CancelReload();
		currentGun.DelayAttack(totalSwapTime / 2f);
		gunIndex = index;
		Invoke("SelectGunSwap", totalSwapTime / 2f);
	}
	private void SelectGunSwap() {
		foreach (GameObject g in guns)
			g.SetActive(false);
		guns[gunIndex].SetActive(true);
		currentGun = guns[gunIndex].GetComponent<Weapon>();
		if (!(currentGun is MeleeWeapon))
			currentGun.DelayAttack(.25f);
	}

	// Basically, they're not a civilian. Has a weapon/mask/whatever. Cops should attack!
	public bool IsEquipped() {
		Floor f = LevelBuilder.instance.FloorAt(transform.position);
		bool inRestrictedArea = f != null && f.restricted;
		return weaponDrawn || hasBag || inRestrictedArea;
	}

	public bool CanSee(GameObject target, float fov = 130f, float viewDist = 20f) {
		Vector3 targetPos = target.transform.position;
		targetPos.y = transform.position.y;
		float angle = Vector3.Dot(Vector3.Normalize(transform.position - targetPos), transform.forward);
		float angleDegrees = 90 + Mathf.Asin(angle) * Mathf.Rad2Deg;
		if (angleDegrees > fov / 2f) {
			// Debug.Log(target.name + " is outside FOV for " + name);
			return false;
		}

		Debug.DrawLine(transform.position, target.transform.position, Color.blue, 2f, true);
		RaycastHit[] hits = Physics.RaycastAll(transform.position, target.transform.position - transform.position, viewDist, sightLayers)
					  .Where(x => x.transform.root != transform.root)
					  .OrderBy(x => (x.point - transform.position).magnitude)
					  .ToArray();
		if (hits.Length > 0) {
			// Debug.Log(name + " sees " + string.Join(", ", hits.Select(x => x.transform.root.name + " (" + x.transform.name + ")").ToArray()));
			return hits[0].collider.transform.root == target.transform.root;
		} else {
			// Debug.Log(name + " can't see anything");
		}

		return false;
	}

	private List<Character> CharactersInFront() {
		Vector3 inFrontPos = transform.position + transform.forward * .75f;
		return GameManager.instance.CharactersWithinDistance(inFrontPos, 1.2f);
	}

	// returns true if you grab someone
	public bool DragBody() {
		if (draggedBody != null)
			return false;
		
		List<Character> draggableChars = GameManager.allCharacters.Where(x => {
			if (x == this)
				return false;
			if (x is NPC) {
				NPC z = (NPC) x;
				return !x.isAlive;  // || z.currentState == NPC.NPCState.DOWN_TIED;
			}
			return !x.isAlive;
		}).ToList();

		List<Rigidbody> rbs = new List<Rigidbody>();
		foreach (Character c in draggableChars) {
			rbs.AddRange(c.lt.separateBodyParts.Select(x => x.GetComponentInParent<Rigidbody>()));
		}

		float grabRange = 1.5f;
		rbs = rbs.Where(x => (x.transform.position - transform.position).magnitude < grabRange)
				 .OrderBy(x => (x.transform.position - transform.position).magnitude).ToList();
		foreach (Rigidbody limb in rbs) {
			if (CanSee(limb.gameObject, 100f, grabRange)) {
				draggedBody = limb;
				return true;
			}
		}
		return false;
	}

	protected void Drag() {
		if (draggedBody != null) {
			Vector3 dragPos = transform.position + transform.forward.normalized * 1.2f;
			// add a bit of a buffer between the floor and character to avoid friction
			dragPos.y = Mathf.Max(draggedBody.transform.position.y, .4f);
			Vector3 force = (dragPos - draggedBody.transform.position).normalized;
			draggedBody.AddForce(force * 10000f, ForceMode.Force);
		}
	}

	public void ReleaseBody() {
		if (draggedBody == null)
			return;
		draggedBody = null;
	}

	public void AddBag(Bag bag) {
		if (hasBag) return;

		this.bag = bag;
		bag.transform.parent = transform;
		bag.transform.localPosition = new Vector3(.5f, -.8f, -.1f);
		bag.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
	}

	public void DropBag(bool launch = true) {
		if (!hasBag) return;
		GameObject facingObject = FacingObstruction();
		if (facingObject != null) {
			return;
		}

		bag.DropBag();
		bag.transform.parent = null;
		if (launch) {
			bag.transform.position = transform.position + transform.forward * 1f - transform.up * .6f;
			bag.GetComponent<Rigidbody>().AddForce(transform.forward * (walking ? 800f : 400f), ForceMode.Impulse);
		}
		bag = null;
	}

	// Returns the gameObject the player is facing, or null if there isn't one
	public GameObject FacingObstruction(out RaycastHit hit, float distance = 1f) {
		if (Physics.Raycast(transform.position, transform.forward, out hit, distance)) {
			Debug.Log(hit.collider.transform.parent.name);
			return hit.collider.gameObject;
		}
		return null;
	}
	public GameObject FacingObstruction(float distance = 1f) {
		RaycastHit hit;
		return FacingObstruction(out hit, distance);
	}

	private void UnMarkForQuest() {}


	///////////////////// SAVE STATE FUNCTIONS /////////////////////

	public CharacterData SaveData(CharacterData data) {
		data.voxelBlobs = SaveVoxelBytes();
		data.position = new SerializableVector3(transform.position);
		data.inv = inventory;
		data.weaponId = weaponId;
		data.sidearmId = sidearmId;
		data.equippedWeapon = gunIndex;
		data.isWeaponDrawn = weaponDrawn;
		data.gunSaves = new System.Object[] {
			guns.Length < 1 || guns[0] == null ? null : guns[0].GetComponent<Weapon>().SaveData(),
			guns.Length < 2 || guns[1] == null ? null : guns[1].GetComponent<Weapon>().SaveData(),
		};
		data.health = lt.health;
		data.ridingHorse = ridingHorse;
		data.mountGuid = ridingHorse ? mount.GetComponent<Horse>().SaveData().guid : System.Guid.Empty;
		data.outfit = outfit;
		data.skinColor = skinColor;
		data.hairColor = hairColor;
		data.hairStyle = hairStyle;
		data.accessory = accessory;
		data.groups = groups;
		data.killedBy = killedBy;
		return data;
	}

	public void LoadSaveData(CharacterData data) {
		guid = data.guid;
		voxelBlobs = data.voxelBlobs;
		if (data.position != null)
			transform.position = data.position.val;
		inventory = data.inv;
		weaponId = data.weaponId;
		sidearmId = data.sidearmId;
		gunIndex = data.equippedWeapon;

		female = data.female;		
		(female ? bodyM : bodyF).gameObject.SetActive(false);

		SpawnGuns();
		if (data.gunSaves != null) {
			for (int i = 0; i < guns.Length; i++) {
				if (guns[i] != null && data.gunSaves[i] != null) {
					guns[i].GetComponent<Weapon>().LoadSaveData(data.gunSaves[i]);
				}
			}
		}
		if (data.isWeaponDrawn)
			DrawWeapon();
		if (!float.IsNaN(data.health))
			lt.health = data.health;
		if (!float.IsNaN(data.healthMax))
			lt.healthMax = data.healthMax;
		if (data.ridingHorse)
			GameManager.spawnedHorses.Where(x => x.SaveData().guid == data.mountGuid).First().Mount(this);
		outfit = data.outfit;
		skinColor = data.skinColor;
		hairColor = data.hairColor;
		hairStyle = data.hairStyle;
		accessory = data.accessory;
		groups = data.groups;
		killedBy = data.killedBy;
	}

	private List<List<byte[]>> SaveVoxelBytes() {
		List<List<byte[]>> result = new List<List<byte[]>>();
		foreach (PicaVoxel.Volume vol in volumesToSave) {
			result.Add(vol.GetBytes());
		}
		return result;
	}
}
