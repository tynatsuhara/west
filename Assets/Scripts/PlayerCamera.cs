﻿using UnityEngine;
using System.Linq;

public class PlayerCamera : MonoBehaviour {

	public Player player;
	public float minZoom;
	public float maxZoom;
	public float rotationAngle;
	public float rotationSpeed;
	public float followSpeed;
	public Camera cam;

	private Transform[] targets;

	private float power;
	private float duration;
	private float timeElapsed;
	private bool rotating;
	private Quaternion rotationGoal;
	private Vector3 diff;
	private Vector3 firstPersonInitPosition;

	private float startTime;

	void Start () {
		rotationGoal = transform.rotation;
		diff = transform.localPosition;
		cam.cullingMask |= (1 << LayerMask.NameToLayer("textCam" + (player.id - 1)));
		player.firstPersonCam.cullingMask = cam.cullingMask & ~(1 << LayerMask.NameToLayer("accessory" + player.id));
		player.firstPersonCam.GetComponent<AudioListener>().enabled = player.id == 1;  // can only have one at a time		
		firstPersonInitPosition = player.firstPersonCam.transform.localPosition;

		float x = PlayerPrefs.GetFloat("player" + player.id + "cameraX", transform.eulerAngles.x);
		float y = PlayerPrefs.GetFloat("player" + player.id + "cameraY", transform.eulerAngles.y);
		float z = PlayerPrefs.GetFloat("player" + player.id + "cameraZ", transform.eulerAngles.z);
		transform.eulerAngles = new Vector3(x, y, z);
		rotationGoal = transform.rotation;
		cam.orthographicSize = PlayerPrefs.GetFloat("player" + player.id + "cameraZoom", cam.orthographicSize);
	}

	void Update() {
		UpdatePosition();
	}
	
	void FixedUpdate () {
		transform.localPosition = diff;
		transform.position = Vector3.Lerp(transform.position, AveragePointBetweenTargets(), followSpeed);
		diff = transform.localPosition;		
		// shaking
		if (timeElapsed < duration && !GameManager.paused) {
			transform.position += Random.insideUnitSphere * power * (duration - timeElapsed);
			timeElapsed += Time.deltaTime;
		}
	}

	private int lastDpadValue;
	private void UpdatePosition() {
		if (!player.firstPersonCam.enabled) {
			Vector3 cameraLookAtPosition = transform.position;
			cam.transform.LookAt(transform.position);

			int newDpadValue = Input.GetAxis("DPX" + player.id) == 0 ? 0 : (int) Mathf.Sign(Input.GetAxis("DPX" + player.id));
			bool pressedDpad = newDpadValue != lastDpadValue;
			lastDpadValue = newDpadValue;
			bool rotateButtonPress = (player.id == 1 && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C))) || pressedDpad;
			bool halfDist = player.id == 1 && Input.GetKey(KeyCode.LeftShift);
			if (rotateButtonPress) {
				startTime = Time.realtimeSinceStartup;
				float dir = Input.GetKeyDown(KeyCode.Z) ? -1 : 1;
				if (pressedDpad)
					dir = newDpadValue;
				if (halfDist)
					dir *= .5f;
				Quaternion tempRot = transform.rotation;
				transform.rotation = rotationGoal;
				transform.RotateAround(cameraLookAtPosition, Vector3.up, -rotationAngle * dir);
				rotationGoal = transform.rotation;
				PlayerPrefs.SetFloat("player" + player.id + "cameraX", rotationGoal.eulerAngles.x);
				PlayerPrefs.SetFloat("player" + player.id + "cameraY", rotationGoal.eulerAngles.y);
				PlayerPrefs.SetFloat("player" + player.id + "cameraZ", rotationGoal.eulerAngles.z);
				transform.RotateAround(cameraLookAtPosition, Vector3.up, rotationAngle * dir);
				transform.rotation = tempRot;
				rotating = true;
			} 
			if (rotating) {
				// not linked to deltaTime, since time is frozen when paused
				float realTimeElapsed = (Time.realtimeSinceStartup - startTime);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotationGoal, rotationSpeed * realTimeElapsed);
			}

			// zoom in/out
			float zoom = player.id == 1 && Input.GetAxis("Mouse ScrollWheel") != 0 
					? Input.GetAxis("Mouse ScrollWheel") 
					: Input.GetAxis("DPY" + player.id) * .5f;
			cam.orthographicSize = Mathf.Min(Mathf.Max(minZoom, cam.orthographicSize - zoom), maxZoom);
			PlayerPrefs.SetFloat("player" + player.id + "cameraZoom", cam.orthographicSize);
		} else {

			// first person shake
			player.firstPersonCam.transform.localPosition = firstPersonInitPosition;
			Vector3 delta = Random.insideUnitSphere * power * (duration - timeElapsed);
			delta.z = 0;			
			delta.y *= .5f;
			if (timeElapsed < duration && !GameManager.paused) {
				player.firstPersonCam.transform.localPosition = firstPersonInitPosition + delta;
				timeElapsed += Time.deltaTime;
			}
		}
	}

	public void Shake(float power, float duration) {
		this.power = power;
		this.duration = duration;
		timeElapsed = 0;
	}

	// pre: at least one player in the players array
	private Vector3 AveragePointBetweenTargets() {
		if (player == null)
			return transform.position;
		if (targets == null || targets.Count() == 0)
			return player.transform.position;

		Vector3 minValues = player.transform.position;
		Vector3 maxValues = minValues;
		for (int i = 0; i < targets.Length; i++) {
			minValues.x = Mathf.Min(minValues.x, targets[i].position.x);
			minValues.y = Mathf.Min(minValues.y, targets[i].position.y);
			minValues.z = Mathf.Min(minValues.z, targets[i].position.z);
			maxValues.x = Mathf.Max(maxValues.x, targets[i].position.x);
			maxValues.y = Mathf.Max(maxValues.y, targets[i].position.y);
			maxValues.z = Mathf.Max(maxValues.z, targets[i].position.z);
		}
		return new Vector3(minValues.x + (maxValues.x - minValues.x) / 2,
						   minValues.y + (maxValues.y - minValues.y) / 2,
						   minValues.z + (maxValues.z - minValues.z) / 2);
	}
}
