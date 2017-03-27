using UnityEngine;
using System.Linq;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public System.Guid toId;

	void Awake() {
		StartCoroutine(Delay());
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponentInParent<PlayerControls>()) {
			TeleporterData otherSide = SaveGame.currentGame.map.locations[toId].teleporters
				.Where(x => x.toId == SaveGame.currentGame.map.currentLocation.guid).First();
			other.transform.root.position = otherSide.position.val;
			GameManager.instance.LoadLocation(toId);
		}
	}

	public void LoadSaveData(TeleporterData td) {
		toId = td.toId;
		transform.position = td.position.val;
	}

	private IEnumerator Delay() {
		GetComponent<Collider>().enabled = false;
		yield return new WaitForSeconds(1f);
		GetComponent<Collider>().enabled = true;
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