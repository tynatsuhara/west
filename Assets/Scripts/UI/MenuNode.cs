using UnityEngine;
using UnityEngine.UI;

public class MenuNode : MonoBehaviour {

	[HideInInspector]
	public MenuNode up;
	public MenuNode down;
	[HideInInspector]
	public MenuNode left;
	public MenuNode right;

	public bool carousel;
	public Material defaultMaterial;
	public Material selectedMaterial;
	public Text text;

	public void Start() {
		ColorAll();
		if (down != null)
			down.up = this;
		if (right != null)
			right.left = this;
	}

	private bool selected;

	public void Select() {
		SetSelected(true);
	}

	public void Deselect() {
		SetSelected(false);
	}

	public void SetText(string text) {
		this.text.text = text;
	}

	private void SetSelected(bool newSelectedState) {
		if (selected == newSelectedState)
			return;
		selected = newSelectedState;
		ColorAll();
	}

	private void ColorAll() {
		Text[] t = GetComponentsInChildren<Text>();		
		foreach (Text txt in t)
			txt.material = selected ? selectedMaterial : defaultMaterial;
	}
}
