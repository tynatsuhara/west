using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benchmarking : MonoBehaviour {

	void Start () {
		float start = -Time.time;

		RoomBuilder room = new RoomBuilder(
			"aabaa",
			"afffa",
			"abaaa"
		)
		.Attach("b", new RoomBuilder(
			"aaaaaaaaa",
			"bfffffffb",
			"abbbaaaaa"
		))
		.Finish();

		Debug.Log("time elapsed = " + (start + Time.time));
	}
}
