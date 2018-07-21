using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Benchmarking : MonoBehaviour {

	void Start() {
		Stopwatch timer = Stopwatch.StartNew();	

		AsciiData ascii = new AsciiData();
		UnityEngine.Debug.Log(ascii.Get("room1").ToString(c => c));
		UnityEngine.Debug.Log(ascii.Get("room2").ToString(c => c));

		timer.Stop();
		UnityEngine.Debug.Log("time elapsed = " + timer.ElapsedMilliseconds + "ms");
	}
}
