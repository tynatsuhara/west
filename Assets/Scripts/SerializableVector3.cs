using UnityEngine;

[System.Serializable]
public class SerializableVector3 {
	private float x, y, z;
	public Vector3 val {
		get { return new Vector3(x, y, z); }
	}

	public SerializableVector3(Vector3 v) {
		x = v.x;
		y = v.y;
		z = v.z;
	}
}