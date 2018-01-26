using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	public Player player;

	public Text invText;
	public Text ammoText;
	public Text healthText;
	public Text questMarker;

	public Transform cursor;
	private Vector3 lastMousePos;
	public Vector3 mousePos;

	private List<Dictionary<string, int>> displayedInventories;

	void Start() {
		displayedInventories = new List<Dictionary<string, int>>();
		lastMousePos = mousePos = player.id == 1 
				? Input.mousePosition
				: player.playerCamera.cam.WorldToScreenPoint(player.transform.position);
	}

	void Update() {
		UpdateCursor();
		Cursor.visible = false;

		UpdateQuestMarkers();
	}

	public float markerDistFromEdge;
	private void UpdateQuestMarkers() {
		// combine all into List<{transform: Transform}>
		var positions = GameManager.spawnedNPCs
				// get all marked npc positions
				.Where(x => x.questMarker.activeSelf)
				.Select(x => new {transform = x.questMarker.transform})
				// get all non-pc marked destinations
				.Union(LevelBuilder.instance.markedDestinations.Values.Select(x => new {transform = x.transform}))
				// get all marked teleporters
				.Union(LevelBuilder.instance.teleporters.Where(x => x.HasQuest()).Select(x => new {transform = x.transform}))
				.Select(x => player.playerCamera.cam.WorldToViewportPoint(x.transform.position))
				.Where(p => p.x < 0 || p.x > 1 || p.y < 0 || p.y > 1)
				.ToList();
		if (positions.Count > 0) {  // temp before object pooling + multiple points
			Vector3 p = positions.First();
			Vector3 worldPoint = player.playerCamera.cam.ViewportToWorldPoint(ViewportIntersectPoint(p));
			Vector3 worldCenter = player.playerCamera.cam.ViewportToWorldPoint(new Vector3(.5f, .5f));
			// move it slightly towards the center
			worldPoint += (worldCenter - worldPoint).normalized * markerDistFromEdge * player.playerCamera.cam.orthographicSize;
			questMarker.transform.position = worldPoint;

			Vector3 screenPoint = player.playerCamera.cam.ViewportToScreenPoint(p);
			Vector3 screenCenter = player.playerCamera.cam.ViewportToScreenPoint(new Vector2(.5f, .5f));
			float angle = Mathf.Atan2(screenPoint.y - screenCenter.y, screenPoint.x - screenCenter.x) * Mathf.Rad2Deg;
			questMarker.transform.localEulerAngles = new Vector3(0, 0, angle);
		}
		questMarker.enabled = positions.Count > 0;
	}

	private Vector3 ViewportIntersectPoint(Vector3 outsidePoint) {
		Vector3 midpoint = new Vector2(.5f, .5f);
		List<Vector3?> pts = new List<Vector3?>();
		if (outsidePoint.y > .5f) {
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.up, Vector2.one));
		} else {
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.zero, Vector2.right));
		}
		if (outsidePoint.x < .5f) {
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.zero, Vector2.up));
		} else {
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.right, Vector2.one));
		}
		return pts.Where(x => x.HasValue).OrderBy(x => (midpoint - x.Value).magnitude).First().Value;
	}

	// taken from https://gamedev.stackexchange.com/questions/111100/intersection-of-a-line-and-a-rectangle
	private Vector3? LineIntersectPoint(Vector3 ps1, Vector3 pe1, Vector3 ps2, Vector3 pe2) {
		// Get A,B of first line - points : ps1 to pe1
		float A1 = pe1.y-ps1.y;
		float B1 = ps1.x-pe1.x;
		// Get A,B of second line - points : ps2 to pe2
		float A2 = pe2.y-ps2.y;
		float B2 = ps2.x-pe2.x;

		// Get delta and check if the lines are parallel
		float delta = A1*B2 - A2*B1;
		if(delta == 0) return null;

		// Get C of first and second lines
		float C2 = A2*ps2.x+B2*ps2.y;
		float C1 = A1*ps1.x+B1*ps1.y;
		//invert delta to make division cheaper
		float invdelta = 1/delta;
		// now return the Vector2 intersection point
		return new Vector3((B2*C1 - B1*C2)*invdelta, (A1*C2 - A2*C1)*invdelta);
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

	public void UpdateInventory(Dictionary<string, int> dict) {
		if (!displayedInventories.Contains(dict))
			displayedInventories.Add(dict);

		Dictionary<string, int> mergedInventories = new Dictionary<string, int>();
		foreach (Dictionary<string, int> d in displayedInventories) {
			foreach (string s in d.Keys) {
				if (!mergedInventories.ContainsKey(s))
					mergedInventories.Add(s, 0);
				mergedInventories[s] += d[s];
			}
		}
		string result = "";
		foreach (string s in mergedInventories.Keys) {
			result += s + (mergedInventories[s] > 1 ? " × " + mergedInventories[s] + "\n" : "\n");
		}
		invText.text = result.ToUpper();
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
}
