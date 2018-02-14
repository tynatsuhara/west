using UnityEngine;

[System.Serializable]
public class SerializableVector3 {
	public float x, y, z;
	public Vector3 val {
		get { return new Vector3(x, y, z); }
		set { 
			x = value.x;
			y = value.y;
			z = value.z;
		}
	}

	public SerializableVector3(Vector3 v) : this(v.x, v.y, v.z) {
	}

	public SerializableVector3(float x, float y, float z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public override bool Equals(System.Object obj) {
		if (obj == null)
			return false;
		if (obj is Vector3)
			return val == (Vector3)obj;
		if (obj is SerializableVector3)
			return val == ((SerializableVector3)obj).val;
		return false;
	}

	public override int GetHashCode() {
		return val.GetHashCode();
	}
}