using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualMapLocation : MonoBehaviour {

	public Color defaultColor;
	public Color currentLocationColor;
	public Color questDestinationColor;
	private bool questMarked;

	public void RefreshText(Location l) {
		string str;
		if (l.discovered) {
			str = (l.icon.Length > 0 ? l.icon + "\n" : "") + l.name;
		} else if (questMarked) {
			str = "?";
		} else {
			str = "";
		}
		GetComponent<Text>().text = str;
	}

	public void MarkQuest() {
		GetComponent<Text>().color = questDestinationColor;
		questMarked = true;
	}

	public void MarkCurrentLocation() {
		GetComponent<Text>().color = currentLocationColor;
	}

	public void Unmark() {
		GetComponent<Text>().color = defaultColor;	
		questMarked = false;	
	}
}
