﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// TODO: make this a separate component?
public abstract class LivingThing : MonoBehaviour {

	public bool isAlive {
		get { return health > 0; }
	}
	public float healthMax;
	public float health;
	public float healthRegenRate;  // gain 1 health every healthRegenRate seconds

	protected float regenDelay;  // time, in real seconds, before the character heals 1 hp
	protected List<PicaVoxel.Volume> bodyParts = new List<PicaVoxel.Volume>();          // all body parts which can bleed/be damaged
	protected List<PicaVoxel.Volume> separateBodyParts = new List<PicaVoxel.Volume>();  // parts which are considered separate entities (eg decapitation)

	public virtual void Start() {
		regenDelay = healthRegenRate;  // TODO: should we bother saving this?
	}

	// needs to be called every update tick to keep regenerating health
	public void HeartBeat() {
		regenDelay -= Time.deltaTime;
		if (regenDelay <= 0) {
			health = Mathf.Min(health + 1, healthMax);
			regenDelay += healthRegenRate;
		}
	}

	protected IEnumerator FallOver(float force) {
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		yield return new WaitForSeconds(Random.Range(.2f, 1f));
		for (int i = 0; i < 10; i++) {
			int decidingAngle = 8;
			if (Mathf.Abs(transform.eulerAngles.z) < decidingAngle && Mathf.Abs(transform.eulerAngles.x) < decidingAngle) {	
				Vector3 fallDir = Random.insideUnitCircle;
				fallDir.z = fallDir.y;
				fallDir.y = 0;
				for (int j = 0; j < 3; j++) {
					GetComponent<Rigidbody>().AddForce(fallDir * (force + i * 150f), ForceMode.Impulse);
					yield return new WaitForSeconds(.08f);
				}
			}
			if (Mathf.Abs(transform.eulerAngles.z) >= decidingAngle || Mathf.Abs(transform.eulerAngles.x) >= decidingAngle) {
				yield return new WaitForSeconds(.5f);
			}
		}
	}

	// reset health regen
	public void RegenDelay(float damage) {
		regenDelay = Mathf.Max(regenDelay, damage * healthRegenRate);
	}

	public void DamageEffects(PicaVoxel.Exploder exploder, Vector3 angle, DamageType type) {
		if (type != DamageType.MELEE && type != DamageType.SLICE && type != DamageType.NONLETHAL && type != DamageType.RANGED) {
			exploder.Explode(angle * 3);
		}
		if (type == DamageType.NONLETHAL) {
			SpurtBlood();
			PuddleBlood();
		} else {
			BleedEverywhere();
		}

		if (type != DamageType.MELEE && type != DamageType.NONLETHAL && type != DamageType.SLICE && type != DamageType.RANGED) {
			BleedEverywhere();
			exploder.Explode(angle * 3);
		} else if (type == DamageType.SLICE || type == DamageType.RANGED) {
			BleedEverywhere();
		} else {
			SpurtBlood();
		}
	}

	// GORE GORE GORE

	protected void BleedEverywhere() {
		int bloodSpurtAmount = Random.Range(3, 15);
		for (int i = 0; i < bloodSpurtAmount; i++) {
			Invoke("SpurtBlood", Random.Range(.3f, 1.5f) * i);
		}
		InvokeRepeating("PuddleBlood", .5f, .2f);
		Invoke("CancelPuddling", Random.Range(10f, 30f));
	}

	// Puts a few drops on the ground
	protected void PuddleBlood() {
		int times = Random.Range(1, 5);
		for (int i = 0; i < times; i++) {
			Vector3 pos = separateBodyParts[Random.Range(0, separateBodyParts.Count)].transform.position;		
			WorldBlood.instance.BleedFrom(gameObject, pos);
		}
	}

	protected void CancelPuddling() {
		CancelInvoke("PuddleBlood");
	}

	// One lil spurt
	protected void SpurtBlood() {
		Bleed(separateBodyParts[Random.Range(0, separateBodyParts.Count)].transform.position, Random.Range(5, 10), Vector3.up);
	}

	// puts blood on the NPC
	protected void Bleed(Vector3 position, int amount, Vector3 velocity) {
		for (int i = 0; i < amount * WorldBlood.bloodMultiplier; i++) {
			Vector3 spawnPos = position + Random.insideUnitSphere * .1f;
			foreach (PicaVoxel.Volume volume in bodyParts) {			
				PicaVoxel.PicaVoxelPoint pos = volume.GetVoxelArrayPosition(spawnPos);
				PicaVoxel.Voxel? hit = volume.GetVoxelAtArrayPosition(pos.X, pos.Y, pos.Z);
				if (hit != null && hit.HasValue && hit.Value.Active) {
					PicaVoxel.Voxel voxel = new PicaVoxel.Voxel();
					voxel.Value = hit.Value.Value;
					voxel.Color = WorldBlood.instance.BloodColor();
					voxel.State = PicaVoxel.VoxelState.Active;
					volume.SetVoxelAtArrayPosition(pos, voxel);
					PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(spawnPos, 
						voxel, .1f, 2.5f * velocity + 2 * Random.insideUnitSphere + Vector3.up * 3f);
					break;
				}
			}
		}
	}

	// TODO: Revisit this?
	protected void BloodSplatter(Vector3 pos, float radius = 2f, int rayAmount = 30) {
		for (int k = 0; k < rayAmount; k++) {
			float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
			var off = 2f / rayAmount;
			var y = k * off - 1 + (off / 2);
			var r = Mathf.Sqrt(1 - y * y);
			var phi = k * inc;
			var x = (float)(Mathf.Cos(phi) * r);
			var z = (float)(Mathf.Sin(phi) * r);
			Debug.DrawRay(pos, new Vector3(x, y, z) * radius, Color.red, 5f);
			
			RaycastHit[] hits = Physics.RaycastAll(pos, new Vector3(x, y, z), radius)
				.Where(h => h.transform.root != transform.root && 
						h.transform.GetComponentInParent<PicaVoxel.Volume>() != null &&
						h.transform.GetComponentInParent<Damageable>() != null &&
						h.transform.GetComponentInParent<Wall>() == null)
				.OrderBy(h => h.distance)
				.ToArray();
			if (hits.Length > 0) {
				int splatSize = Random.Range(2, 70);
				for (int i = 0; i < splatSize; i++) {
					Vector3 splatRange = Random.insideUnitSphere * .3f;
					Vector3 point = hits[0].point + new Vector3(x, y, z).normalized * .1f + splatRange;
					PicaVoxel.Volume vol = hits[0].transform.GetComponentInParent<PicaVoxel.Volume>();
					PicaVoxel.Voxel? voxq = vol.GetVoxelAtWorldPosition(point);
					if (voxq == null || !voxq.Value.Active) {
						// Debug.Log("Couldn't paint!");
					} else {
						PicaVoxel.Voxel vox = (PicaVoxel.Voxel)voxq;
						vox.Color = WorldBlood.instance.BloodColor();
						vol.SetVoxelAtWorldPosition(point, vox);
					}
				}
			}
		}
	}
}
