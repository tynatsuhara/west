using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	public Player player;

	public Text objectiveText;
	public Text ammoText;
	public Text healthText;
	public OffScreenMarker questMarkerPrefab;

	public Transform cursor;
	private Vector3 lastMousePos;
	public Vector3 mousePos;

	public Dialogue dialogue;

	void Start() {
		lastMousePos = mousePos = player.id == 1 
				? Input.mousePosition
				: player.playerCamera.cam.WorldToScreenPoint(player.transform.position);
	}

	void Update() {
		UpdateCursor();
		Cursor.visible = false;

		UpdateQuestMarkers();
	}

	private List<OffScreenMarker> questMarkers = new List<OffScreenMarker>();
	private void UpdateQuestMarkers() {
		// combine all into List<Vector3 position>, account for x/y offsets that get fucky between objects
		List<Vector3> positions = GameManager.spawnedNPCs
				// get all marked npc positions
				.Where(x => x.questMarker.activeSelf)
				.Select(x => x.questMarker.transform.position + x.questMarker.transform.up * .4f)
				// get all non-npc marked destinations
				.Union(LevelBuilder.instance.markedDestinations.Values.Where(x => x != null).Select(x => x.transform.position + 1.6f * Vector3.up))
				// get all marked teleporters
				.Union(LevelBuilder.instance.teleporters.Where(x => x.HasQuest()).Select(x => x.transform.position - Vector3.up))
				.Select(x => player.playerCamera.cam.WorldToViewportPoint(x))
				.Where(p => p.x < 0 || p.x > 1 || p.y < 0 || p.y > 1)
				.ToList();

		while (questMarkers.Count < positions.Count) {
			OffScreenMarker m = Instantiate(questMarkerPrefab);
			m.transform.SetParent(transform);
			m.transform.localScale = Vector3.one;
			questMarkers.Add(m);
		}

		for (int i = 0; i < questMarkers.Count; i++) {
			if (i < positions.Count) {
				questMarkers[i].gameObject.SetActive(true);
				questMarkers[i].Indicate(positions[i], player.playerCamera.cam);
			} else {
				questMarkers[i].gameObject.SetActive(false);
			}
		}
	}

	private void UpdateCursor() {
		if (player.id == 1) {
			mousePos += Input.mousePosition - lastMousePos;
			lastMousePos = Input.mousePosition;
		}
		if (player.firstPersonCam.enabled) {
			Vector3 pos = new Vector3(player.firstPersonCam.pixelWidth / 2f, player.firstPersonCam.pixelHeight / 2f, 0);
			pos = player.firstPersonCam.ScreenToWorldPoint(pos);
			pos.z = 0;
			cursor.transform.localPosition = pos;
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			cursor.transform.position = player.playerCamera.cam.ScreenToWorldPoint(mousePos);
			Cursor.lockState = CursorLockMode.None;			
		}
	}

	public void JoystickCursorMove(float dx, float dy) {
		if (player.firstPersonCam.enabled) {
			
		} else {
			if (dx == 0 && dy == 0)
				return;
			// TODO: make this distance configurable in settings
			float mouseDist = 180f;
			Vector3 playerPos = player.playerCamera.cam.WorldToScreenPoint(player.transform.position);
			mousePos = Vector3.Lerp(mousePos, playerPos + new Vector3(dx, dy, 0).normalized * mouseDist, .1f);
		}
	}

	public void UpdateObjectives(List<string> messages) {
		objectiveText.text = string.Join("\n", messages.Select(x => x.Trim('\n')).ToArray()).ToUpper();
	}

	public void UpdateAmmo(string weaponName, int ammo, int clipSize) {
		ammoText.text = (weaponName + "\n" + ammo + "/" + clipSize).ToUpper();
	}

	public void UpdateAmmo(string weaponName) {
		ammoText.text = weaponName.ToUpper();
	}

	public void ShowReloading(string weaponName) {
		ammoText.text = weaponName.ToUpper() + "\nRELOADING...";
	}

	public void UpdateHealth(float health, float healthMax) {
		healthText.text = health > 0 ? new string('*', Mathf.CeilToInt(health)) : "";
	}

	public void HitMarker() {
		CancelInvoke("UnHitMarker");
		cursor.GetComponent<RawImage>().material = GameUI.instance.textRed;
		Invoke("UnHitMarker", .15f);
	}

	private void UnHitMarker() {
		cursor.GetComponent<RawImage>().material = GameUI.instance.textWhite;
	}

	public void ShowDialogue(bool onRight) {
		dialogue.ShowDialogue(player, onRight);
	}

	public void HideDialogue() {
		dialogue.Hide();
	}
}
