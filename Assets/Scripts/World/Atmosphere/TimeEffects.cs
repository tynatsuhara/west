using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEffects : MonoBehaviour {

	public Color dayColor;         // 6am-8pm
	public Color transitionColor;  // 5-6am and 8-9pm
	public Color nightColor;       // 9pm-6am

	void Start () {
		RenderSettings.ambientLight = getColorForTime();		
		StartCoroutine(CheckTime());
	}

	private IEnumerator CheckTime() {
		while (true) {
			yield return new WaitForSeconds(WorldTime.MINUTE);			
			Color nextColor = getColorForTime();
			RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, nextColor, .1f);
		}
	}

	private Color getColorForTime() {
		float hour = (SaveGame.currentGame.time.worldTime % WorldTime.DAY / WorldTime.HOUR);		
		if (hour >= 6 && hour < 20) {
			return dayColor;
		} else if ((hour >= 5 && hour < 6) || (hour >= 20 && hour < 21)) {
			return transitionColor;
		} else {
			return nightColor;
		}
	}
}
