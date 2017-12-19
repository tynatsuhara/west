using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Benchmarking : MonoBehaviour {

	void Start () {
		Stopwatch timer = Stopwatch.StartNew();

		for (int i = 0; i < 1; i++) {
			RoomBuilder room = new RoomBuilder(
				"aaaaaaaaaaaaa",
				"afffffffffffa",
				"afffffffffffa",
				"afffffffffffa",
				"aaabbbaaaaaaa"
			)
			.Attach("b", new RoomBuilder(
				"aaaaaaaaa",
				"bfffffffb",
				"abbbaaaaa"
			))
			.Finish();
		}

		timer.Stop();
		UnityEngine.Debug.Log("time elapsed = " + timer.ElapsedMilliseconds);
	}
}
