﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Horse : LivingThing, Interactable, Damageable {

	private HorseSaveData data;
	private Character rider;

	public Color32[] bodyColor;
	public Color32[] maneColor;
	public bool tamed;
	public float jumpForce;
	public PicaVoxel.Exploder exploder;

	public void Start() {
		bodyParts.Add(GetComponent<PicaVoxel.Volume>());
		separateBodyParts.AddRange(bodyParts);
	}

	// Ride
	public void Interact(Character character) {
		if (!isAlive || rider != null)
			return;
		character.MountHorse(this);
		rider = character;
		if (!tamed)
			StartCoroutine(Tame());
	}
	public void Uninteract(Character character) {}

	public void Dismount() {
		rider = null;
	}

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool playerAttacker = false, DamageType type = DamageType.BULLET) {
		bool wasAlive = isAlive;
		exploder.transform.position = location + angle * Random.Range(-.1f, .15f) - Vector3.up * Random.Range(.2f, .6f);
		if (isAlive)
			Bleed(exploder.transform.position, Random.Range(1, 10), angle);
		health -= damage;		
		float forceVal = Random.Range(500, 900);
		GetComponent<Rigidbody>().AddForceAtPosition(forceVal * angle.normalized, exploder.transform.position, ForceMode.Impulse);
		if (wasAlive && !isAlive) {
			if (rider != null) {
				rider.Dismount();  // dismount first so that character doesn't get damaged by exploder
			}
			if (type != DamageType.MELEE && type != DamageType.NONLETHAL) {
				BleedEverywhere();
				exploder.Explode(angle * 3);
			} else {
				SpurtBlood();
			}
			StartCoroutine(FallOver(800));
		}
		return false;
	}

	public IEnumerator Tame() {
		int jumpsNeeded = Random.Range(4, 8);
		for (int i = 0; i < jumpsNeeded; i++) {
			yield return new WaitForSeconds(Random.Range(.2f, .7f));
			Jump();
			yield return new WaitForSeconds(.1f);			
			if (rider != null && Random.Range(0, 6) == 0) {
				rider.Dismount();
			}
			if (rider == null) {
				break;
			}
			if (i == jumpsNeeded - 1) {
				tamed = true;
			}
		}
	}

	public void Jump() {
		Vector3 force = Random.insideUnitSphere * jumpForce * .2f;
		force.y = jumpForce * Random.Range(.8f, 1.3f);
		GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
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


	public HorseSaveData SaveData() {
		data.location = new SerializableVector3(transform.position);
		data.eulerAngles = new SerializableVector3(transform.eulerAngles);
		data.tamed = tamed;
		data.health = health;
		return data;
	}

	public void LoadSaveData(HorseSaveData hsd) {
		data = hsd;
		transform.position = data.location.val;
		transform.eulerAngles = data.eulerAngles.val;
		tamed = hsd.tamed;
		health = hsd.health;
		if (!isAlive)
			StartCoroutine(FallOver(800));
		Color();		
	}

	[System.Serializable]
	public class HorseSaveData {
		public HorseSaveData(GameObject horsePrefab) {
			Horse h = horsePrefab.GetComponent<Horse>();
			health = h.healthMax;
			bodyColor = Random.Range(0, h.bodyColor.Length);
			maneColor = Random.Range(0, h.maneColor.Length);
			speckled = Random.Range(0, 5) == 0;
			tamed = h.tamed;
		}

		public System.Guid guid = System.Guid.NewGuid();
		public float health;
		public int bodyColor;
		public int maneColor;
		public bool speckled;
		public bool tamed;
		public SerializableVector3 location = new SerializableVector3(Vector3.zero);
		public SerializableVector3 eulerAngles = new SerializableVector3(new Vector3(0, Random.Range(0, 360), 0));
	}
}
