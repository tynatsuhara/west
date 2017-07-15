using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEffects : MonoBehaviour {

	public Color sunriseColor;   // 5-6am
	public Color dayColor;       // 6am-8pm
	public Color sunsetColor;    // 8-9pm
	public Color nightColor;     // 9pm-6am

	void Update() {
		RenderSettings.ambientLight = getColorForTime();
	}

	private Color getColorForTime() {
		float hour = (SaveGame.currentGame.time.worldTime % WorldTime.DAY) / WorldTime.HOUR;
		float timeSoFar = SaveGame.currentGame.time.worldTime % WorldTime.HOUR;
		float transitionTime = 45 * WorldTime.MINUTE;
		// Debug.Log(timeSoFar + "/" + transitionTime + " = " + percentTransitioned);
		if (hour >= 5 && hour < 6) {
			float percentTransitioned = Mathf.Clamp01((timeSoFar + (hour - 5) * WorldTime.HOUR)/transitionTime);
			return Color.Lerp(nightColor, sunriseColor, percentTransitioned); // sunrise		
		} else if (hour >= 6 && hour < 20) {
			float percentTransitioned = Mathf.Clamp01((timeSoFar + (hour - 6) * WorldTime.HOUR)/transitionTime);
			return Color.Lerp(sunriseColor, dayColor, percentTransitioned);   // day	
		} else if (hour >= 20 && hour < 21) {
			float percentTransitioned = Mathf.Clamp01((timeSoFar + (hour - 20) * WorldTime.HOUR)/transitionTime);			
			return Color.Lerp(dayColor, sunsetColor, percentTransitioned);    // sunset
		} else {
			float percentTransitioned = Mathf.Clamp01((timeSoFar + (24 + hour - 21) % 24 * WorldTime.HOUR)/transitionTime);			
			return Color.Lerp(sunsetColor, nightColor, percentTransitioned);  // night			
		}
	}
}
