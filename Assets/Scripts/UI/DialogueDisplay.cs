using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour {

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
	
	private bool onScreen;
	private GameObject displayedCharacter;
	private OffScreenSlide slide;

	public void Start() {
		slide = GetComponent<OffScreenSlide>();
	}

	public void Hide() {
		// tint.gameObject.SetActive(false);
		onScreen = false;
		slide.MoveOffScreen();		
	}

	public void ShowDialogue(Player p, bool onRight) {
		onScreen = true;
		if (displayedCharacter != null)
			Destroy(displayedCharacter);

		GameObject clone = displayedCharacter = Instantiate(p.gameObject, p.transform.position, p.transform.rotation);
		Destroy(clone.GetComponent<Character>());
		Destroy(clone.GetComponent<Rigidbody>());
		Destroy(clone.GetComponentInChildren<AudioListener>());
		clone.transform.SetParent(transform);

		// position clone
		RectTransform rect = clone.AddComponent(typeof(RectTransform)) as RectTransform;
		rect.anchorMin = rect.anchorMax = rect.pivot = onRight ? Vector2.right : Vector2.zero;
		rect.anchoredPosition3D = new Vector3((onRight ? -1 : 1) * sidePadding, bottomPadding, playerDepth);
		rect.localScale = Vector3.one * scale;
		rect.localEulerAngles = rotation;
		rect.transform.RotateAround(rect.transform.position, rect.transform.up, (onRight ? 1 : -1) * leftRightRotation);

		// tint.gameObject.SetActive(true);
		slide.MoveOnScreen();
	}

	public bool IsShowing() {
		return onScreen;
		// return tint.gameObject.activeSelf;
	}
}
