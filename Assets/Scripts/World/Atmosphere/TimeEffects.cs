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
		float hour = (SaveGame.currentGame.time.worldTime % WorldTime.DAY / WorldTime.HOUR);
		float timeSoFar = (SaveGame.currentGame.time.worldTime % WorldTime.HOUR);
		float transitionTime = 45 * WorldTime.MINUTE;
		float percentTransitioned = Mathf.Clamp01(timeSoFar/transitionTime);
		if (hour >= 6 && hour < 20) {
			return Color.Lerp(sunriseColor, dayColor, percentTransitioned);   // day	
		} else if (hour >= 5 && hour < 6) {
			return Color.Lerp(nightColor, sunriseColor, percentTransitioned); // sunrise		
		} else if (hour >= 20 && hour < 21) {
			return Color.Lerp(dayColor, sunsetColor, percentTransitioned);    // sunset
		} else {
			return Color.Lerp(sunsetColor, nightColor, percentTransitioned);  // night			
		}
	}
}
