using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor : MonoBehaviour {

	public bool restricted;
	public bool kickUpDirt;
	public Color32 dirtColor;  // the color that should be kicked up. White = set to biome color

	private static PicaVoxel.Voxel dirtVoxel;
	private const float MIN_HEIGHT = .04f;

	static Floor() {
		List<byte> bytes = new List<byte>(new byte[2]);
		bytes[0] = (byte)PicaVoxel.VoxelState.Active;
		bytes.AddRange(new byte[] { 157, 140, 94, 0 });
		dirtVoxel = new PicaVoxel.Voxel(bytes.ToArray());
	}

	public void KickUpDirtLanding(Vector3 pos, float blastPower) {
		if (!kickUpDirt)
			return;
		SetDirtColor();
		pos.y = MIN_HEIGHT;
		int dirts = Random.Range(6, 10);
		for (int i = 0; i < dirts; i++) {
			Vector3 dir = Random.insideUnitSphere * blastPower;
			dir.y = Mathf.Abs(dir.y);
			PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(pos, dirtVoxel, .1f, dir, Random.Range(.3f, .6f));			
		}
	}

	public void KickUpDirtWalking(Vector3 pos) {
		if (!kickUpDirt)
			return;
		SetDirtColor();
		pos.y = 0;
		int dirts = Random.Range(1, 4);	
		for (int i = 0; i < dirts; i++) {
			pos += Random.insideUnitSphere * .45f;
			pos.y = Mathf.Max(pos.y, MIN_HEIGHT);
			Vector3 dir = Vector3.up * .25f + Random.insideUnitSphere * .5f;
			PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(pos, dirtVoxel, .1f, dir, Random.Range(.3f, .6f));
		}
	}

	private void SetDirtColor() {
		dirtVoxel.Color = dirtColor == Color.white ? GetBiomeTint() : dirtColor;		
	}

	private Color32 GetBiomeTint() {
		Color32 c = (Color32) LevelBuilder.instance.mat.GetColor("_Tint");
		int diff = -16;
		c.r = (byte)Mathf.Clamp(c.r + diff, 0, 255);
		c.g = (byte)Mathf.Clamp(c.g + diff, 0, 255);
		c.b = (byte)Mathf.Clamp(c.b + diff, 0, 255);
		return c;
	}
}
