using UnityEngine;
using System.Collections.Generic;

public class Horse : MonoBehaviour, Interactable, Damageable {

	public float health;
	public float healthMax;

	public Color32[] bodyColor;
	public Color32[] maneColor;

	void Start () {
		Color();
	}
	
	void Update () {
	
	}

	public void Interact(Character character) {}
	public void Uninteract(Character character) {}

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool playerAttacker = false, DamageType type = DamageType.BULLET) {
		return false;
	}

	private void Color() {
		Dictionary<byte, Color32> palette = new Dictionary<byte, Color32>();
		palette.Add(0, bodyColor[Random.Range(0, bodyColor.Length)]);
		palette.Add(10, maneColor[Random.Range(0, maneColor.Length)]);

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
							int r = 8;
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
}
