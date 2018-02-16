using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour {

	// character display
	public Vector3 rotation;
	public float leftRightRotation;
	public float sidePadding;
	public float bottomPadding;
	public float scale;
	public float playerDepth;

	// background
	public RectTransform tint;
	public float tintPadding;
	public float tintShift;

	private GameObject displayedCharacter;

	public void Start() {
		Hide();
	}

	public void Update() {
		tint.sizeDelta = new Vector2(Screen.width - tintPadding, tint.rect.height);
		tint.transform.localPosition = new Vector3(tintShift, tint.localPosition.y, tint.localPosition.z);
	}

	public void Hide() {
		tint.gameObject.SetActive(false);
		if (displayedCharacter != null)
			Destroy(displayedCharacter);
	}

	public void ShowDialogue(Player p, bool onRight) {
		if (displayedCharacter != null)
			Destroy(displayedCharacter);

		GameObject clone = displayedCharacter = Instantiate(p.gameObject, p.transform.position, p.transform.rotation);
		Destroy(clone.GetComponent<Character>());
		Destroy(clone.GetComponent<Rigidbody>());
		Destroy(clone.GetComponentInChildren<AudioListener>());
		clone.transform.SetParent(p.playerUI.transform);

		// position clone
		RectTransform rect = clone.AddComponent(typeof(RectTransform)) as RectTransform;
		rect.anchorMin = rect.anchorMax = rect.pivot = onRight ? Vector2.right : Vector2.zero;
		rect.anchoredPosition3D = new Vector3((onRight ? -1 : 1) * sidePadding, bottomPadding, playerDepth);
		rect.localScale = Vector3.one * scale;
		rect.localEulerAngles = rotation;
		rect.transform.RotateAround(rect.transform.position, rect.transform.up, (onRight ? 1 : -1) * leftRightRotation);

		tint.gameObject.SetActive(true);
	}
}
