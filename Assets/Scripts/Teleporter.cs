using UnityEngine;
using System.Linq;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public System.Guid toId;
	public string destination;
	private TeleporterData otherSide;
	private bool ignore = true;

	void Awake() {
		StartCoroutine(Delay());
	}

	void OnTriggerEnter(Collider other) {
		if (ignore)
			return;
		Character character = other.GetComponentInParent<Character>();		
		PlayerControls pc = other.GetComponentInParent<PlayerControls>();
		if (character || pc) {
			other.transform.root.position = otherSide.position.val;
			if (character.ridingHorse) {
				Horse.HorseSaveData hsd = character.mount.SaveData();
				Map.CurrentLocation().horses.Remove(hsd.guid);
				Map.Location(toId).horses.Add(hsd.guid);
			}
			if (pc) {
				GameManager.instance.LoadLocation(toId);
			} else {
				// TODO: save character to the other location
			}
		}
	}

	public void LoadSaveData(TeleporterData td) {
		toId = td.toId;
		transform.position = td.position.val;
		destination = SaveGame.currentGame.map.locations[toId].name;
		otherSide = SaveGame.currentGame.map.locations[toId].teleporters
				.Where(x => x.toId == SaveGame.currentGame.map.currentLocation && x.tag == td.tag).First();
		GetComponentInChildren<TextObject>().Say(destination, permanent: true);			
	}

	private IEnumerator Delay() {
		yield return new WaitForSeconds(1f);
		GetComponent<SphereCollider>().enabled = true;
		yield return new WaitForSeconds(.2f);
		ignore = false;
	}

	[System.Serializable]
	public class TeleporterData {
		public System.Guid toId;
		public SerializableVector3 position;
		public string tag;

		public TeleporterData(System.Guid toId, Vector3 position, string tag = "") {
			this.toId = toId;
			this.position = new SerializableVector3(position);
			this.tag = tag;
		}
	}
}