using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEffects : MonoBehaviour {

	void OnCollisionEnter(Collision collision) {
		Jumper j = collision.transform.root.GetComponent<Jumper>();
		if (j == null)
			return;
		Floor f = LevelBuilder.instance.FloorAt(j.transform.position);
		if (f == null)
			return;
		f.KickUpDirtLanding(j.transform.position, j.landingDirtRadius);
	}
}
