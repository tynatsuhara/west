using UnityEngine;
using System.Linq;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public System.Guid toId;
	public string destination;
	private bool ignore = true;
	private TeleporterData otherSide;

	void Awake() {
		StartCoroutine(Delay());
	}

	void OnTriggerEnter(Collider other) {
		if (ignore)
			return;
		if (other.GetComponentInParent<PlayerControls>()) {
			ignore = true;
			other.transform.root.position = otherSide.position.val;
			GameManager.instance.LoadLocation(toId);
		}
	}

	public void LoadSaveData(TeleporterData td) {
		toId = td.toId;
		transform.position = td.position.val;
		destination = SaveGame.currentGame.map.locations[toId].name;
		otherSide = SaveGame.currentGame.map.locations[toId].teleporters
				.Where(x => x.toId == SaveGame.currentGame.map.currentLocation).First();
	}

	private IEnumerator Delay() {
		ignore = true;
		yield return new WaitForSeconds(1f);
		ignore = false;
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