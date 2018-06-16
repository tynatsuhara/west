using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualMapLocation : MonoBehaviour {

	public Color defaultColor;
	public Color currentLocationColor;
	public Color questDestinationColor;
	private bool questMarked;
	public MeshRenderer birdsEyeRender;

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

	public void RefreshBirdsEyeView(Location l) {
		// if (l.mapRender == null)
		// 	return;
		// RenderTexture ogRender = LevelBuilder.instance.townRenderCam.targetTexture;
		// Texture2D t = new Texture2D(LevelBuilder.instance.townRenderCam.pixelWidth, LevelBuilder.instance.townRenderCam.pixelHeight);
		// t.LoadRawTextureData(l.mapRender);
		// t.Apply();
		// birdsEyeRender.material.mainTexture = t;
		// Debug.Log("loaded tex data for " + name);
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
