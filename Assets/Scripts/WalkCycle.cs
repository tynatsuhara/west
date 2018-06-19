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
	
	void Update() {
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

	protected void KickUpDirt() {
		if (!isWalking)
			return;
		Floor f = LevelBuilder.instance.FloorAt(transform.position);
		if (f == null)
			return;
		f.KickUpDirtWalking(transform.root.position + transform.forward * .1f);
	}
}
