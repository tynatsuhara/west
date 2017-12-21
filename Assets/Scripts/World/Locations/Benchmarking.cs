using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Benchmarking : MonoBehaviour {

	void Start () {
		Stopwatch timer = Stopwatch.StartNew();	

		for (int i = 0; i < 100; i++) {
			InteriorLocation room = new RoomBuilder(
				"=============",
				"=///////////#",
				"=///////////#",
				"=///////////#",
				"=###########="
			)
			.AddOverlapRule('#', '=')
			.Attach("#", "-", new RoomBuilder(
				"==###==",
				"#/////=",
				"#/////=",
				"======="
			))
			.Attach("#", "-", new RoomBuilder(
				"====#====",
				"#///////=",
				"======##="
			))
			.Replace("#", "=")
			.Build(null);

			UnityEngine.Debug.Log(room);
		}

		timer.Stop();
		UnityEngine.Debug.Log("time elapsed = " + timer.ElapsedMilliseconds + "ms");
	}
}
