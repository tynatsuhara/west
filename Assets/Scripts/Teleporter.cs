using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public System.Guid toId;

	void OnTriggerEnter(Collider other) {
		if (other.GetComponentInParent<PlayerControls>()) {
			Debug.Log("teleported to " + toId);
		}
	}

	public void LoadSaveData(TeleporterData td) {
		toId = td.toId;
		transform.position = td.position.val;
	}

	[System.Serializable]
	public class TeleporterData {
		public System.Guid toId;
		public SerializableVector3 position;

		public TeleporterData(System.Guid toId, Vector3 position) {
			this.toId = toId;
			this.position = new SerializableVector3(position);
		}
	}
}