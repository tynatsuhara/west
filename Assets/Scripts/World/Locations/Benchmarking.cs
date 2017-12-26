using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Benchmarking : MonoBehaviour {

	void Start () {
		Stopwatch timer = Stopwatch.StartNew();	

		for (int i = 0; i < 100; i++) {
			Room room1 = new Room('a', '/',
				"      #////#",
				"      #////#",
				"//####/////#",
				"///////#####"
			);

			Room room2 = new Room('b', '/',
				"///////////#",
				"///////////#",
				"///////////#",
				"############"
			);

			UnityEngine.Debug.Log(new InteriorBuilder(room1).Attach("##", room2));
		}

		timer.Stop();
		UnityEngine.Debug.Log("time elapsed = " + timer.ElapsedMilliseconds + "ms");
	}
}
