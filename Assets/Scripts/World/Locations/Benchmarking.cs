using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Benchmarking : MonoBehaviour {

	void Start () {
		Stopwatch timer = Stopwatch.StartNew();	

		for (int i = 0; i < 100; i++) {
			Room room = new Room('a', '/',
				"///////////#",
				"///////////#",
				"///////////#",
				"############"
			);

			new InteriorBuilder().Attach("#", room);
			.Attach("#", "-", new Room(
				"//###//",
				"#//////",
				"#//////",
				"///////"
			))
			.Attach("#", "-", new Room(
				"////#////",
				"#////////",
				"//////##/"
			))
			.Replace("#", "=")
			.Build(null, System.Guid.Empty);

			UnityEngine.Debug.Log(room);
		}

		timer.Stop();
		UnityEngine.Debug.Log("time elapsed = " + timer.ElapsedMilliseconds + "ms");
	}
}
