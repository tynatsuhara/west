using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Benchmarking : MonoBehaviour {

	void Start() {
		Stopwatch timer = Stopwatch.StartNew();	

		for (int i = 0; i < 1; i++) {
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

			InteriorBuilder ib = new InteriorBuilder(room1).Attach("##", room2);
			
			UnityEngine.Debug.Log(ib);
		}

		timer.Stop();
		UnityEngine.Debug.Log("time elapsed = " + timer.ElapsedMilliseconds + "ms");
	}
}
