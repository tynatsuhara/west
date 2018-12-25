using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class Horse : MonoBehaviour, Damageable {

	private HorseSaveData data;
	private Character rider;
	private bool canRide = true;
	private LivingThing lt;
	private bool walking;

	public Color32[] bodyColor;
	public Color32[] maneColor;
	public GameObject saddle;
	public bool tamed;
	public float jumpForce;
	public float fallingVelocityTrigger;
	public float canRideResetTime;
	public PicaVoxel.Exploder exploder;

	[System.Serializable]
	public enum HorseState {
		CHILLIN,          // staying in the same general area, moving around every once in a while
		RIDDEN,           // character is mounted
		FOLLOWING_OWNER
	}

	public void Awake() {
		lt = GetComponent<LivingThing>();
		// StartCoroutine(DelayCanRide());	// Should this be here?	
	}

	public void Start() {
		lt.bodyParts.Add(GetComponent<PicaVoxel.Volume>());
		lt.separateBodyParts.Add(GetComponent<PicaVoxel.Volume>());
		SetName();	
	}

	void Update() {
		data.health = lt.health;	

		if (!lt.isAlive || GameManager.paused)
			return;

		switch (data.state) {
			case HorseState.CHILLIN:
				// to-do: wander around
				break;
			case HorseState.FOLLOWING_OWNER:
				Character owner = GameManager.instance.GetCharacter(data.owner);
				NavMeshAgent agent = GetComponent<NavMeshAgent>();
				if (agent.isActiveAndEnabled) {
					agent.SetDestination(owner.transform.position);
					agent.stoppingDistance = 5f;
				}
				break;
			case HorseState.RIDDEN:
				break;
		}

		if (data.state != HorseState.RIDDEN) {
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			walking = agent.enabled && agent.velocity.magnitude > .1f;
			WalkCycle walk = GetComponent<WalkCycle>();
			if (walking && !walk.isWalking) {
				walk.StartWalk();
			} else if (!walking && walk.isWalking) {
				walk.StandStill(false);
			}
		}
	}

	void OnTriggerEnter(Collider collider) {
		if (!lt.isAlive || rider != null || !canRide)
			return;
		Character c = collider.transform.root.GetComponent<Character>();
		if (c == null || !c.isAlive)
			return;
		if (c.GetComponent<Rigidbody>().velocity.y < fallingVelocityTrigger && c.transform.position.y > 1.2) {
			// Debug.Log("Landed on horse! Velocity = " + c.GetComponent<Rigidbody>().velocity.y);
			Mount(c);
		} else if (Mathf.Abs(c.transform.position.y - transform.position.y - 1) < .1) {  // if the player ends up on top w/o falling
			// Debug.Log("On top of horse! Y diff = " + Mathf.Abs(c.transform.position.y - transform.position.y - 1));
			Mount(c);
		}
	}

	public void Mount(Character character) {
		if (!lt.isAlive || rider != null || !canRide)
			return;
		character.MountHorse(this);
		rider = character;
		data.state = HorseState.RIDDEN;
		SetName();
		if (character.GetComponent<NavMeshAgent>() == null) {  // only disable agent if it's a controllable character
			GetComponent<NavMeshAgent>().enabled = false;
			GetComponent<NavMeshObstacle>().enabled = true;
		}
		if (!tamed) {
			StartCoroutine(Tame());
		} else if (character.guid != data.owner && !data.stolen) {
			data.stolen = true;
			SaveGame.currentGame.crime.Commit(character.guid, Map.CurrentTown().guid, "Horse Theft", 10);
			GameManager.instance.AlertInRange(Stimulus.HORSE_THEFT, transform.position, 10f, visual: transform.root.gameObject, alerter: character);					
		}
	}

	public void Call() {
		data.state = HorseState.FOLLOWING_OWNER;
	}

	public void Dismount() {
		rider = null;
		GetComponent<WalkCycle>().StandStill();
		SetName();
		GetComponent<NavMeshObstacle>().enabled = false;
		GetComponent<NavMeshAgent>().enabled = true;
		StartCoroutine(DelayCanRide());
		data.state = HorseState.CHILLIN;
	}
	private IEnumerator DelayCanRide() {
		canRide = false;
		yield return new WaitForSeconds(canRideResetTime);
		canRide = true;
	}

	private void SetName() {
		if (data.owner != System.Guid.Empty) {
			name = SaveGame.GetCharacterData(data.owner).name + "'s Horse";
		} else if (tamed) {  // TODO: Is this case ever going to happen?
			name = "Horse";
		} else {
			name = "Wild Horse";
		}
	}

	public bool Damage(Vector3 location, Vector3 angle, float damage, Character attacker = null, DamageType type = DamageType.BULLET) {
		bool wasAlive = lt.isAlive;
		exploder.transform.position = location + angle * Random.Range(-.1f, .15f) - Vector3.up * Random.Range(.2f, .6f);
		if (lt.isAlive)
			lt.Bleed(exploder.transform.position, Random.Range(1, 10), angle);
		lt.health -= damage;
		lt.RegenDelay(damage);
		float forceVal = Random.Range(500, 900);
		GetComponent<Rigidbody>().AddForceAtPosition(forceVal * angle.normalized, exploder.transform.position, ForceMode.Impulse);
		GameManager.instance.AlertInRange(Stimulus.VIOLENCE, transform.position, 10f, visual: transform.root.gameObject, alerter: attacker);
		if (wasAlive && !lt.isAlive) {
			if (attacker is Player && type != DamageType.NONLETHAL) {
				SaveGame.currentGame.stats.animalsKilled++;
			}
			if (attacker != null) {
				SaveGame.currentGame.crime.Commit(attacker.guid, Map.CurrentTown().guid, "Horse Murder", 30);
			}
			if (rider != null) {
				rider.Dismount();  // dismount first so that character doesn't get damaged by exploder
			}
			GetComponent<NavMeshAgent>().enabled = false;
			GetComponent<NavMeshObstacle>().enabled = true;
			lt.DamageEffects(exploder, angle, type);
			lt.FallOver(800);
		}
		return false;
	}

	private IEnumerator Tame() {
		int jumpsNeeded = Random.Range(4, 8);
		for (int i = 0; i < jumpsNeeded; i++) {
			yield return new WaitForSeconds(Random.Range(.2f, .7f));
			// buck the rider off
			Jumper j = GetComponent<Jumper>();
			Vector3 force = Random.insideUnitSphere * j.jumpForce.y * .4f;
			j.Jump(new Vector3(force.x, Random.Range(.6f, 1f) * j.jumpForce.y, force.z));
			yield return new WaitForSeconds(.1f);			
			if (rider != null && Random.Range(0, 6) == 0) {
				rider.Dismount();
			}
			if (rider == null) {
				break;
			}
			if (i == jumpsNeeded - 1) {
				SetTamed(rider.guid);
			}
		}
	}

	private void SetTamed(System.Guid owner) {
		tamed = true;
		saddle.SetActive(true);	
		data.owner = owner;
	}

	private void Color() {
		Dictionary<byte, Color32> palette = new Dictionary<byte, Color32>();
		palette.Add(0, bodyColor[data.bodyColor]);
		palette.Add(10, bodyColor[data.maneColor]);

		HashSet<Vector3> speckles = new HashSet<Vector3>();
		bool firstFrame = true;

		PicaVoxel.Volume volume = GetComponent<PicaVoxel.Volume>();
		foreach (PicaVoxel.Frame frame in volume.Frames) {
			for (int x = 0; x < frame.XSize; x++) {
				for (int y = 0; y < frame.YSize; y++) {
					for (int z = 0; z < frame.ZSize; z++) {
						PicaVoxel.Voxel? voxq = frame.GetVoxelAtArrayPosition(x, y, z);
						PicaVoxel.Voxel vox = (PicaVoxel.Voxel)voxq;
						if (voxq == null || vox.State != PicaVoxel.VoxelState.Active)
							continue;

						if (palette.ContainsKey(vox.Value)) {
							Color32 c = palette[vox.Value];
							Vector3 specktor3 = new Vector3(x, y, z);
							if (data.speckled && ((firstFrame && Random.Range(0, 10) == 0) || (!firstFrame && speckles.Contains(specktor3)))) {
								int lightness = 20;
								c.r = (byte) Mathf.Clamp(c.r + lightness, 0, 255);
								c.g = (byte) Mathf.Clamp(c.g + lightness, 0, 255);
								c.b = (byte) Mathf.Clamp(c.b + lightness, 0, 255);
								speckles.Add(specktor3);
							}
							int r = 4;
							vox.Color = new Color32(CharacterCustomization.JiggleByte(c.r, r), 
													CharacterCustomization.JiggleByte(c.g, r), 
													CharacterCustomization.JiggleByte(c.b, r), 0);
						} else if (vox.Value == 128) {
							// guts
							vox.Color = WorldBlood.instance.BloodColor();
						}
						frame.SetVoxelAtArrayPosition(new PicaVoxel.PicaVoxelPoint(new Vector3(x, y, z)), vox);
					}
				}
			}
			frame.UpdateChunks(true);
			firstFrame = false;
		}
	}

	public System.Guid GetGuid() {
		return data.guid;
	}

	public HorseSaveData SaveData() {
		data.bytes = GetComponent<PicaVoxel.Volume>().GetBytes();
		data.location = new SerializableVector3(transform.position);
		data.eulerAngles = new SerializableVector3(transform.eulerAngles);
		data.tamed = tamed;
		data.health = lt.health;
		return data;
	}

	public void LoadSaveData(HorseSaveData hsd) {
		data = hsd;
		transform.position = data.location.val;
		transform.eulerAngles = data.eulerAngles.val;
		if (data.tamed)
			SetTamed(data.owner);
		saddle.SetActive(tamed);
		if (!float.IsNaN(data.health))
			lt.health = data.health;
		if (!float.IsNaN(data.healthMax))
			lt.healthMax = data.healthMax;
		if (!lt.isAlive)
			lt.FallOver(800);
		Color();
		if (data.bytes != null)
			GetComponent<PicaVoxel.Volume>().SetBytes(data.bytes);
	}

	[System.Serializable]
	public class HorseSaveData {
		public HorseSaveData(GameObject horsePrefab, System.Guid owner) {
			Horse h = horsePrefab.GetComponent<Horse>();
			bodyColor = Random.Range(0, h.bodyColor.Length);
			maneColor = Random.Range(0, h.maneColor.Length);
			speckled = Random.Range(0, 5) == 0;
			if (owner != System.Guid.Empty) {
				tamed = true;
				this.owner = owner;
			}
		}

		public System.Guid guid = System.Guid.NewGuid();
		public List<byte[]> bytes;
		public float health = float.NaN;
		public float healthMax = float.NaN;
		public int bodyColor;
		public int maneColor;
		public bool speckled;
		public bool tamed;
		public bool stolen;
		public System.Guid owner = System.Guid.Empty;
		public SerializableVector3 location = new SerializableVector3(Vector3.zero);
		public SerializableVector3 eulerAngles = new SerializableVector3(new Vector3(0, Random.Range(0, 360), 0));
		public HorseState state = HorseState.CHILLIN;
	}
}
