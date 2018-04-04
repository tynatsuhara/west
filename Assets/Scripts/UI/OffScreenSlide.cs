using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * For future debugging: using an overlay canvas might be fucky if something moves fully offscreen
 */

public class OffScreenSlide : MonoBehaviour {

	public Vector2 offScreenPos;
	public float onLerp;
	public float offLerp;
	public bool startOffScreen;

	private float lerpRate;
	private Vector2 onScreenPos;
	private Vector2 destinationPos;
	private RectTransform rect;

	public void Start() {
		rect = GetComponent<RectTransform>();
		onScreenPos = rect.anchoredPosition;
		rect.anchoredPosition = startOffScreen ? offScreenPos : onScreenPos;
		destinationPos = rect.anchoredPosition;
	}

	public void Update() {
		Vector3 pos = Vector3.Slerp(rect.anchoredPosition, destinationPos, 50 * lerpRate * Time.unscaledDeltaTime);
		pos.x = rect.anchoredPosition.x;
		rect.anchoredPosition = pos;
	}
	
	public void MoveOffScreen() {
		destinationPos = offScreenPos;
		lerpRate = offLerp;
	}

	public void MoveOnScreen() {
		destinationPos = onScreenPos;
		lerpRate = onLerp;
	}
}
