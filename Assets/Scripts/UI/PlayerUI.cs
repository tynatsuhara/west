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

	public Text interactMenu;
	public Interactable highlightedInteractable;

	public Transform cursor;
	private Vector3 lastMousePos;
	public Vector3 mousePos;

	public DialogueDisplay dialogueDisplay;

	void Start() {
		lastMousePos = mousePos = player.id == 1 
				? new Vector3(player.playerCamera.cam.pixelWidth/2f, player.playerCamera.cam.pixelHeight/2f)
				: player.playerCamera.cam.WorldToScreenPoint(player.transform.position);
	}

	void Update() {
		Cursor.visible = false;

		UpdateQuestMarkers();

		bool esc = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 9");
		if (esc && dialogueDisplay.IsShowing()) {
			HideDialogue();
		}
	}

	void LateUpdate() {
		MouseCursorMove();
		UpdateCursor();
	}

	private List<OffScreenMarker> questMarkers = new List<OffScreenMarker>();
	// points to offscreen markers
	private void UpdateQuestMarkers() {
		// combine all into List<Vector3 position>, account for x/y offsets that get fucky between objects
		List<Vector3> positions = GameManager.spawnedNPCs
				// get all marked npc positions
				.Where(x => x.characterIndicator.ShowOffScreenMarker())
				.Select(x => x.characterIndicator.transform.position + x.characterIndicator.transform.up * .4f)
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

	public float stickyCursorDistance = 5f;
	public float stickyCursorLerpRate = .3f;
	public Vector3 stickyCursorNPCOffset;

	private void UpdateCursor() {
		if (player.firstPersonCam.enabled) {
			Vector3 pos = new Vector3(player.firstPersonCam.pixelWidth / 2f, player.firstPersonCam.pixelHeight / 2f, 0);
			pos = player.firstPersonCam.ScreenToWorldPoint(pos);
			pos.z = 0;
			cursor.transform.localPosition = pos;
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Vector3? targetPos = GameManager.spawnedNPCs
					.Select(npc => player.playerCamera.cam.WorldToScreenPoint(npc.transform.position + stickyCursorNPCOffset))
					.Where(npcPos => Vector3.Distance(mousePos, npcPos) < stickyCursorDistance)
					.OrderBy(npcPos => Vector3.Distance(mousePos, npcPos))
					.Cast<Vector3?>()
					.FirstOrDefault();
			if (targetPos != null) {
				// Debug.Log("mouse dist " + targetPos.Value);
				mousePos = Vector3.Lerp(mousePos, targetPos.Value, stickyCursorLerpRate);
			}
			cursor.transform.position = player.playerCamera.cam.ScreenToWorldPoint(mousePos);
			Cursor.lockState = CursorLockMode.None;

			// If they are interacting and move their mouse away, don't quit the interaction
			// (best example case is when dragging)
			if (player.IsInteractButtonDown() && interactMenu.text.Length > 0) {
				return;
			}

			// Get the hovered-over interactable and show menu
			RaycastHit[] interactables = Physics.RaycastAll(player.playerCamera.cam.ScreenPointToRay(mousePos))
					.Where(x => x.collider.GetComponentInParent<Interactable>() != null)
					.OrderBy(x => x.distance)
					.ToArray();
			interactMenu.text = "";
			highlightedInteractable = null;
			foreach (RaycastHit hit in interactables) {
				Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
				InteractAction[] actions = interactable.GetActions(player);
				// TODO: Support displaying multiple interaction types
				if (actions.Length > 0) {
					interactMenu.text = hit.transform.name + "\n[E] " + actions.First().action;
					interactMenu.material = actions.First().enabled ? GameUI.instance.textWhite : GameUI.instance.textGrey;
					highlightedInteractable = interactable;
					break;
				}
			}
		}
	}

	public void MouseCursorMove() {
		if (player.id == 1) {  // only player 1 can use mouse, otherwise JoystickCursorMove is called
			mousePos += Input.mousePosition - lastMousePos;
			lastMousePos = Input.mousePosition;
		}
	}

	// TODO: How should joystick cursor actually behave? ATM it is locked 
	// within range of the player and pulls back to the center if they release
	public void JoystickCursorMove(float dx, float dy) {
		if (!player.firstPersonCam.enabled && dx != 0 && dy != 0) {
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

	public void ShowDialogue(Dialogue d, NPC npc) {
		foreach (OffScreenSlide slide in GetComponentsInChildren<OffScreenSlide>()) {
			if (slide.gameObject != dialogueDisplay.gameObject) {
				slide.MoveOffScreen();
			}
		}
		dialogueDisplay.StartDialogue(d, player, npc);
	}

	public void HideDialogue() {
		foreach (OffScreenSlide slide in GetComponentsInChildren<OffScreenSlide>()) {
			if (slide.gameObject != dialogueDisplay.gameObject) {
				slide.MoveOnScreen();
			}
		}
		dialogueDisplay.Hide();
	}

	public bool IsDialogueShowing() {
		return dialogueDisplay.IsShowing();
	}
}
