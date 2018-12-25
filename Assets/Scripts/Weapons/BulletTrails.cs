using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrails : MonoBehaviour {

	public static BulletTrails instance;

	public float duration;
	public LineRenderer lineRendererSample;

	// Parallel renderer and expiration time lists
	public List<LineRenderer> rendered = new List<LineRenderer>();
	public List<float> expirations = new List<float>();

	void Awake() {
		instance = this;
	}

	void LateUpdate() {
		while (expirations.Count > 0 && expirations[0] <= Time.time) {
			Destroy(rendered[0].gameObject);
			rendered.RemoveAt(0);
			expirations.RemoveAt(0);
		}
	}

	public void RenderShot(Vector3 a, Vector3 b) {
		// todo set up render
		GameObject go = Instantiate(lineRendererSample.gameObject);
		go.transform.parent = transform;
		LineRenderer render = go.GetComponent<LineRenderer>();
		render.SetPosition(0, a);
		render.SetPosition(1, b);
		render.enabled = true;

		rendered.Add(render);
		expirations.Add(Time.time + duration);
	}

	public void ClearAll() {
		foreach (LineRenderer render in rendered) {
			Destroy(render.gameObject);
		}
		rendered.Clear();
		expirations.Clear();
	}
}
