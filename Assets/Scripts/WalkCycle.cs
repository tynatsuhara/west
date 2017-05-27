using UnityEngine;
using System.Collections.Generic;

public class WalkCycle : MonoBehaviour {

	private PicaVoxel.Volume volume;
	private float timeElapsed = .1f;
	private int startFrame;
	private int endFrame;
	private Character walker;

	public float interval;
	public int sittingFrame;
	public int ridingFrame;
	public int standingFirstFrame;
	public int standingLastFrame;
	public int walkingFirstFrame;
	public int walkingLastFrame;

	public bool isWalking {
		get { return startFrame == walkingFirstFrame; }
	}

	void Awake () {
		volume = GetComponent<PicaVoxel.Volume>();
		walker = GetComponentInParent<Character>();
		StandStill(true);
	}

	public void StartWalk() {
		startFrame = walkingFirstFrame;
		endFrame = walkingLastFrame;
		timeElapsed = 0f;
		volume.SetFrame(walkingFirstFrame);
	}

	public void StandStill(bool immediate = false) {
		startFrame = standingFirstFrame;
		endFrame = standingLastFrame;
		timeElapsed = 0f;
		if (immediate)
			volume.SetFrame(standingFirstFrame);
	}

	public void Sit() {
		volume.SetFrame(sittingFrame);
	}

	public void Ride() {
		volume.SetFrame(ridingFrame);
	}
	
	void Update () {
		if (GameManager.paused || volume.CurrentFrame == sittingFrame || volume.CurrentFrame == ridingFrame) {
			return;
		}

		timeElapsed += Time.deltaTime;
		if (timeElapsed >= interval) {
			timeElapsed %= interval;
			int newFrame = volume.CurrentFrame + 1;
			if (newFrame > endFrame) {
				newFrame %= (endFrame + 1);
				newFrame += startFrame;
			}
			volume.SetFrame(newFrame);
			KickUpDirt();
		}
	}

	private static PicaVoxel.Voxel? dirtVoxel;
	protected void KickUpDirt() {
		if (!walker.walking)
			return;
		
		if (dirtVoxel == null) {
			List<byte> bytes = new List<byte>(new byte[2]);
			bytes[0] = (byte)PicaVoxel.VoxelState.Active;
			bytes.AddRange(new byte[] { 157, 140, 94, 0 });
			dirtVoxel = new PicaVoxel.Voxel(bytes.ToArray());
		}

		Floor f = LevelBuilder.instance.FloorAt(transform.position);
		if (f != null && f.kickUpDirt) {
			PicaVoxel.Voxel v = dirtVoxel.Value;
			v.Color = f.dirtColor == Color.white ? GetBiomeTint() : f.dirtColor;
			int dirts = Random.Range(1, 4);	
			for (int i = 0; i < dirts; i++) {
				Vector3 pos = transform.root.position;
				pos.y = 0;
				pos += transform.forward * .1f + Random.insideUnitSphere * .45f;
				pos.y = Mathf.Max(pos.y, .04f);

				Vector3 dir = transform.up * .25f + Random.insideUnitSphere * .5f;
				PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(pos, v, .1f, dir, Random.Range(.3f, .6f));
			}
		}
	}
	private Color32 GetBiomeTint() {
		Color32 c = (Color32)LevelBuilder.instance.mat.GetColor("_Tint");
		int diff = -16;
		c.r = (byte)Mathf.Clamp(c.r + diff, 0, 255);
		c.g = (byte)Mathf.Clamp(c.g + diff, 0, 255);
		c.b = (byte)Mathf.Clamp(c.b + diff, 0, 255);
		return c;
	}
}
