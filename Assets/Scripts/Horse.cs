﻿using UnityEngine;
using System.Collections.Generic;

public class Horse : MonoBehaviour, Interactable, Damageable {

	public float healthMax;
	private HorseSaveData data;
	private Character rider;

	public Color32[] bodyColor;
	public Color32[] maneColor;

	void Start () {
		if (data == null) {  // hasn't been spawned before, no data saved
			data = new HorseSaveData();
			data.health = healthMax;
			data.bodyColor = Random.Range(0, bodyColor.Length);
			data.maneColor = Random.Range(0, maneColor.Length);
			data.speckled = Random.Range(0, 5) == 0;
		}
		Color();		
	}

	// Ride
	public void Interact(Character character) {
		character.MountHorse(this);
		rider = character;

	}
	public void Uninteract(Character character) {}

	public void Dismount() {
		rider = null;
	}

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool playerAttacker = false, DamageType type = DamageType.BULLET) {
		data.health -= damage;
		if (data.health <= 0) {
			if (rider != null)
				rider.Dismount();
		}
		return false;
	}

	private void Color() {
		Dictionary<byte, Color32> palette = new Dictionary<byte, Color32>();
		palette.Add(0, bodyColor[data.bodyColor]);
		palette.Add(10, bodyColor[data.maneColor]);

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
							if (data.speckled && Random.Range(0, 10) == 0) {
								int lightness = 20;
								c.r = (byte) Mathf.Clamp(c.r + lightness, 0, 255);
								c.g = (byte) Mathf.Clamp(c.g + lightness, 0, 255);
								c.b = (byte) Mathf.Clamp(c.b + lightness, 0, 255);
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
		}
	}


	public HorseSaveData SaveData() {
		return data;
	}

	public void LoadSaveData(HorseSaveData hsd) {
		data = hsd;
	}

	[System.Serializable]
	public class HorseSaveData {
		public System.Guid guid = System.Guid.NewGuid();
		public float health;
		public int bodyColor;
		public int maneColor;
		public bool speckled;
	}
}