using UnityEngine;

[System.Serializable]
public class SerializableVector3 {
	public float x, y, z;
	public Vector3 val {
		get { return new Vector3(x, y, z); }
	}

	public SerializableVector3(Vector3 v) {
		x = v.x;
		y = v.y;
		z = v.z;
	}

	public override bool Equals(System.Object obj) {
		if (obj == null)
			return false;
		if (!(obj is SerializableVector3))
			return false;
		return val == ((SerializableVector3)obj).val;
	}

	public override int GetHashCode() {
		return val.GetHashCode();
	}
}