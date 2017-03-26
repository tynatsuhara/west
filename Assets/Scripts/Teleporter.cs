using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public System.Guid toId;

	void OnTriggerEnter(Collider other) {
		if (other.GetComponentInParent<PlayerControls>()) {
			SaveGame.Save();
		}
	}

	public TeleporterData SaveData() {
		TeleporterData td = new TeleporterData();
		td.toId = toId;
		td.position = new SerializableVector3(transform.position);
		return td;
	}

	public void LoadSaveData(TeleporterData td) {
		toId = td.toId;
	}

	public class TeleporterData {
		public System.Guid toId;
		public SerializableVector3 position;
	}
}