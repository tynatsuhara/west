using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkCycle : MonoBehaviour {

	private PicaVoxel.Volume volume;
	private float timeElapsed = .1f;
	private int startFrame;
	private int endFrame;
	private Character walker;

	public float interval;
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
		StopWalk();
	}

	public void StartWalk() {
		startFrame = walkingFirstFrame;
		endFrame = walkingLastFrame;
		timeElapsed = 0f;
		volume.SetFrame(walkingFirstFrame);
	}

	public void StopWalk(bool immediate = false) {
		startFrame = standingFirstFrame;
		endFrame = standingLastFrame;
		timeElapsed = 0f;
		if (immediate)
			volume.SetFrame(standingFirstFrame);
	}
	
	void Update () {
		if (GameManager.paused) {
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
			for (int i = 0; i < 2; i++) {
				Vector3 pos = transform.root.position;
				pos.y = transform.position.y - .9f;
				PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(pos + transform.forward * .1f + Random.insideUnitSphere * .45f, 
						dirtVoxel.Value, .1f, transform.up * .25f + Random.insideUnitSphere * .5f, .55f);
			}
		}
	}
}
