using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

public class Player : Character {

	// fields assigned by GameManager when spawned
	public int id;
	public PlayerCamera playerCamera;
	public Camera firstPersonCam;
	public PlayerUI playerUI;


	public override void Start() {
		name = "Player " + id;
		base.Start();
		InitFirstPersonCamera();
	}

	void Update() {
		playerUI.UpdateHealth(health, healthMax);
		
		if (!isAlive || GameManager.paused)
			return;

		GetInput();
		if (currentGun != null)
			currentGun.UpdateUI();
		LookAtMouse();
		Rotate();
	}

	private float interactHoldTime;
	private bool interactWasHeld;
	void GetInput() {
		bool p1 = id == 1;

		// F - Draw/hide weapon
		if ((p1 && Input.GetKey(KeyCode.F)) || Input.GetKey("joystick " + id + " button 3")) {
			interactHoldTime += Time.deltaTime;
			if (interactHoldTime >= .7f) {
				Shout();
				interactWasHeld = true;		
			}
		} else if ((p1 && Input.GetKeyUp(KeyCode.F)) || Input.GetKeyUp("joystick " + id + " button 3")) {
			interactHoldTime = 0;
			if (interactWasHeld) {
				interactWasHeld = false;
			} else if (weaponDrawn) {
				HideWeapon();
			} else {
				DrawWeapon();				
			}
		}

		// E - Interact
		if ((p1 && Input.GetKeyDown(KeyCode.E)) || Input.GetKeyDown("joystick " + id + " button 1")) {
			Teleporter teleporter = GameObject.FindObjectsOfType<Teleporter>()
					.Where(x => x.CollidingWith(this))
					.OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
					.FirstOrDefault();
			if (teleporter != null) {
				teleporter.Teleport(this);
			} else if (ridingHorse) {
				Dismount();
			} else {
				bool drag = draggedBody || DragBody();
				if (!drag) {
					Interact();
				}
			}
		} else if ((p1 && Input.GetKeyUp(KeyCode.E)) || Input.GetKeyUp("joystick " + id + " button 1")) {
			ReleaseBody();
			InteractCancel();
		}

		// Q - Drop bag
		if ((p1 && Input.GetKeyDown(KeyCode.Q)) || Input.GetKeyDown("joystick " + id + " button 2")) {
			DropBag();
		}

		if ((p1 && Input.GetMouseButton(0)) || Input.GetKey("joystick " + id + " button 7")) {
			Ray ray = firstPersonCam.enabled ? new Ray(transform.position, firstPersonCam.transform.forward) : playerCamera.cam.ScreenPointToRay(playerUI.mousePos);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit) && 
					(hit.transform.root.tag != "Ground" && hit.collider.transform.root.GetComponentInChildren<Floor>() == null || firstPersonCam.enabled) && 
					hit.collider.transform.root != transform.root) {
				Damageable d = hit.collider.GetComponentInParent<Damageable>();
				MonoBehaviour db = (MonoBehaviour) d;
				// Shoot(d == null ? hit.point : db.transform.position);
				Shoot(hit.point + (ray.direction) * .35f);
			} else {
				Shoot();
			}
		} else if ((p1 && Input.GetMouseButtonDown(1)) || Input.GetKeyDown("joystick " + id + " button 6")) {
			Melee();
		// } else if (Input.GetKeyDown(KeyCode.Alpha3)) {      // TODO: ¿improve explosions?
			// TriggerExplosive();
		}

		if ((p1 && Input.GetKeyDown(KeyCode.R)) || Input.GetKeyDown("joystick " + id + " button 0")) {
			Reload();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			SelectGun(0);
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			SelectGun(1);
		}

		if ((p1 && Input.GetKeyDown(KeyCode.P)) || Input.GetKeyDown("joystick " + id + " button 5")) {
			SwitchCamera(!firstPersonCam.enabled);
		}

		playerUI.JoystickCursorMove(Input.GetAxis("RSX" + id), Input.GetAxis("RSY" + id));

		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			Damage(transform.position, Random.insideUnitCircle, 1);
		}

		if (Input.GetKeyDown(KeyCode.F5)) {
			GameUI.instance.topCenterText.Say("QUICKSAVING...", showFlash: true);
			SaveGame.Save(true);
		} else if (Input.GetKeyDown(KeyCode.F9)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
 
	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;
		Walk();		
		Drag();
    }

	private void InitFirstPersonCamera() {
		List<Transform> transforms = new List<Transform>();
		foreach (Accessory a in GetComponentsInChildren<Accessory>())
			transforms.AddRange(a.GetComponentsInChildren<Transform>());
		transforms.AddRange(head.GetComponentsInChildren<Transform>());

		foreach (Transform t in transforms)
			t.gameObject.layer = LayerMask.NameToLayer("accessory" + id);
	}

	public void SwitchCamera(bool firstPerson) {
		firstPersonCam.enabled = firstPerson;
		playerCamera.cam.enabled = !firstPerson;		
		firstPersonCam.GetComponent<BoxCollider>().enabled = firstPerson;  // so you won't push into walls and clip
		playerUI.GetComponent<Canvas>().worldCamera = firstPerson ? firstPersonCam : playerCamera.cam;  // put the UI on the right camera
		playerUI.GetComponent<Canvas>().planeDistance = 20f;
	}

	private void Walk() {
		float h = 0;
		float v = 0;
		if (id == 1) {  // player 1 can use keyboard
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
			walking = h != 0 || v != 0;
		}
		if ((id == 1 && !walking) || id != 1) {
			h = Input.GetAxis("Horizontal" + id);
			v = Input.GetAxis("Vertical" + id);
			walking = h != 0 || v != 0;
		}
		Move(h, v);
	}

	private void Move(float x, float z) {
		if (ridingHorse) {
			walk.Ride();
			if (!mount.SaveData().tamed)
				return;
		}

		float cameraRotation = playerCamera.transform.eulerAngles.y;

		float speed = CalculateSpeed() * Time.fixedDeltaTime;
		if (Cheats.instance.IsCheatEnabled("konami"))
			speed *= 3f;

		GameObject mover = ridingHorse ? mount.gameObject : gameObject;

		Vector3 mountFaceDir = Vector3.zero;
		if (firstPersonCam.enabled) {
			Vector3 dir = transform.right * speed * x + transform.forward * speed * z;
			mover.GetComponent<Rigidbody>().MovePosition(mover.transform.position + dir);
			mountFaceDir = mover.transform.position + dir;
		} else {
			Vector3 pos = mover.transform.position;
			pos.x += speed * (z * Mathf.Sin(cameraRotation * Mathf.Deg2Rad) + 
				x * Mathf.Sin((cameraRotation + 90) * Mathf.Deg2Rad));
			pos.z += speed * (z * Mathf.Cos(cameraRotation * Mathf.Deg2Rad) + 
				x * Mathf.Cos((cameraRotation + 90) * Mathf.Deg2Rad));
			mountFaceDir = pos;
			mover.GetComponent<Rigidbody>().MovePosition(pos);
		}

		if (ridingHorse) {
			if (x != 0 || z != 0) {
				Quaternion rider = transform.rotation;
				Quaternion q = Quaternion.LookRotation(mount.transform.position - mountFaceDir);
				mount.transform.rotation = Quaternion.Lerp(mount.transform.rotation, q, .1f);
				if (firstPersonCam.enabled) {
					transform.rotation = rider;
				}
				if (!mount.GetComponent<WalkCycle>().isWalking)
					mount.GetComponent<WalkCycle>().StartWalk();
			} else {
				mount.GetComponent<WalkCycle>().StandStill(true);
			}
		} else if ((x != 0 || z != 0) && !walk.isWalking) {
			walk.StartWalk();
		} else if (x == 0 && z == 0 && walk.isWalking) {
			walk.StandStill(true);
		}
		if (x != 0 || z != 0) {
			lastMoveDirection = new Vector3(x, 0, z).normalized;
		}
	}

	void LookAtMouse() {
		if (firstPersonCam.enabled) {
			LoseLookTarget();
			transform.RotateAround(transform.position, transform.up, Input.GetAxis("Mouse X") * 150f * Time.deltaTime);
			firstPersonCam.transform.RotateAround(firstPersonCam.transform.position, transform.right, Input.GetAxis("Mouse Y") * -150f * Time.deltaTime);
		} else {
			// Generate a plane that intersects the transform's position with an upwards normal.
			Plane playerPlane = new Plane(Vector3.up, transform.position);
			Ray ray = playerCamera.cam.ScreenPointToRay(playerUI.mousePos);
			float hitdist = 0f;
			// If the ray is parallel to the plane, Raycast will return false.
			if (playerPlane.Raycast(ray, out hitdist)) {
				LookAt(ray.GetPoint(hitdist));
			}
		}
	}

	public void Shout() {
		if (!speech.currentlyDisplaying) {
			speech.SayRandom(Speech.PLAYER_SHOUT, showFlash: true, color:"yellow");
		}
		GameManager.instance.AlertInRange(Reaction.AGGRO, transform.position, 4f);
	}

	public override void Alert(Reaction importance, Vector3 position) {}


	///////////////////// SAVE STATE FUNCTIONS /////////////////////

	public PlayerSaveData SaveData() {
		PlayerSaveData data = (PlayerSaveData) base.SaveData(SaveGame.currentGame.savedPlayers[id - 1]);
		return data;
	}

	public void LoadSaveData(PlayerSaveData psd) {
		base.LoadSaveData(psd);
		CharacterOptionsManager.instance.SetOutfit(id, psd.outfit);
		CharacterOptionsManager.instance.SetSkinColor(id, psd.skinColor);
		CharacterOptionsManager.instance.SetHairColor(id, psd.hairColor);
		CharacterOptionsManager.instance.SetHairstyle(id, psd.hairStyle);
		CharacterOptionsManager.instance.SetAccessory(id, psd.accessory);		
	}

	[System.Serializable]
	public class PlayerSaveData : CharacterSaveData {
	}
}