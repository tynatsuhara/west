using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerControls : Character {

	// fields assigned by GameManager when spawned
	public int id;
	public PlayerCamera playerCamera;
	public Camera firstPersonCam;
	public PlayerUI playerUI;


	public override void Start() {
		name = "Player " + id;
		
		base.Start();
		CharacterOptionsManager.instance.CustomizeFromSave(this);

		// hide accessories from first person camera
		foreach (Accessory a in GetComponentsInChildren<Accessory>())
			foreach (Transform t in a.GetComponentsInChildren<Transform>())
				t.gameObject.layer = LayerMask.NameToLayer("accessory" + id);	
	}

	void Update() {
		playerUI.UpdateHealth(health, healthMax);
		
		if (!isAlive || GameManager.paused)
			return;

		GetInput();
		if (currentGun != null)
			currentGun.UpdateUI();
		LookAtMouse();
		Walk();
		Rotate();
	}

	private float interactHoldTime;
	void GetInput() {
		bool p1 = id == 1;

		// F - Draw/hide weapon
		if ((p1 && Input.GetKey(KeyCode.F)) || Input.GetKey("joystick " + id + " button 3")) {
			interactHoldTime += Time.deltaTime;
			if (interactHoldTime >= 1f) {
				Shout();				
			}
		} else if ((p1 && Input.GetKeyUp(KeyCode.F)) || Input.GetKeyUp("joystick " + id + " button 3")) {
			if (weaponDrawn) {
				HideWeapon();
			} else {
				DrawWeapon();				
			}
		}

		// E - Interact
		if ((p1 && Input.GetKeyDown(KeyCode.E)) || Input.GetKeyDown("joystick " + id + " button 1")) {
			if (ridingHorse) {
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
			Shoot();
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

		if (Input.GetKeyDown(KeyCode.P)) {
			SwitchCamera(!firstPersonCam.enabled);
		}

		playerUI.JoystickCursorMove(Input.GetAxis("RSX" + id), Input.GetAxis("RSY" + id));		
	}
 
	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;
		
		Drag();
    }

	public void SwitchCamera(bool firstPerson) {
		firstPersonCam.enabled = firstPerson;
		firstPersonCam.GetComponent<BoxCollider>().enabled = firstPerson;
		playerUI.GetComponent<Canvas>().worldCamera = firstPerson ? firstPersonCam : playerCamera.cam;
		playerUI.GetComponent<Canvas>().planeDistance = 20f;
		playerCamera.cam.enabled = !firstPerson;
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

		float speed = CalculateSpeed() * Time.deltaTime;
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



	public PlayerSaveData SaveData() {
		PlayerSaveData psd = SaveGame.currentGame.savedPlayers[id - 1];
		psd.position = new SerializableVector3(transform.position);
		psd.inv = inventory;
		psd.weaponId = weaponId;
		psd.sidearmId = sidearmId;
		psd.equippedWeapon = gunIndex;
		psd.isWeaponDrawn = weaponDrawn;
		psd.gunSaves = new System.Object[] {
			guns[0] == null ? null : guns[0].GetComponent<Gun>().SaveData(),
			guns[1] == null ? null : guns[1].GetComponent<Gun>().SaveData(),
		};
		psd.health = health;
		psd.ridingHorse = ridingHorse;
		psd.mountGuid = ridingHorse ? mount.GetComponent<Horse>().SaveData().guid : System.Guid.Empty;
		return psd;
	}

	public void LoadSaveData(PlayerSaveData psd) {
		guid = psd.guid;
		if (psd.position != null)
			transform.position = psd.position.val;
		inventory = psd.inv;
		weaponId = psd.weaponId;
		sidearmId = psd.sidearmId;
		gunIndex = psd.equippedWeapon;
		SpawnGun();
		if (psd.gunSaves != null) {
			for (int i = 0; i < guns.Length; i++) {
				if (guns[i] != null && psd.gunSaves[i] != null) {
					guns[i].GetComponent<Gun>().LoadSaveData(psd.gunSaves[i]);
				}
			}
		}
		if (psd.isWeaponDrawn)
			DrawWeapon();
		if (psd.health >= 0)
			health = psd.health;
		if (psd.ridingHorse)
			MountHorse(GameManager.localHorses.Where(x => x.SaveData().guid == psd.mountGuid).First());
		CharacterOptionsManager.instance.SetOutfit(id, psd.outfit);
		CharacterOptionsManager.instance.SetSkinColor(id, psd.skinColor);
		CharacterOptionsManager.instance.SetHairColor(id, psd.hairColor);
		CharacterOptionsManager.instance.SetHairstyle(id, psd.hairStyle);
		CharacterOptionsManager.instance.SetAccessory(id, psd.accessory);		
	}

	[System.Serializable]
	public class PlayerSaveData {
		public System.Guid guid = System.Guid.NewGuid();
		public SerializableVector3 position;
		public Inventory inv = new Inventory();
		public float health = -1;
		public int weaponId = -1;
		public int sidearmId = 0;  // start with pistol
		public int equippedWeapon = 1;  // start wielding sidearm
		public System.Object[] gunSaves;		
		public bool isWeaponDrawn;
		public string outfit = "default";
		public int skinColor;
		public int hairColor;
		public int hairStyle;
		public int accessory;

		public bool ridingHorse;
		public System.Guid mountGuid;
	}
}
