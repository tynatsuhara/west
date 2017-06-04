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
			if (pc) {
				GameManager.instance.loadReposition = otherSide.position.val;
				float dist = Map.CurrentLocation().DistanceFrom(Map.Location(toId));
				GameManager.instance.LoadLocation(toId, 4 /* minutes per distance unit */ * dist * pc.moveSpeed / pc.CalculateSpeed());
				ignore = true;
			} else {
				// TODO: save character to the other location
			}
			if (character.ridingHorse) {
				Horse.HorseSaveData hsd = character.mount.SaveData();
				Map.CurrentLocation().horses.Remove(hsd.guid);
				Map.Location(toId).horses.Add(hsd.guid);
			}
		}
	}

	// Mark if the player should go this way for a quest
	public void MarkQuest(bool hasQuest) {
		GetComponentInChildren<TextObject>().Say(destination, color: hasQuest ? "green" : "white", permanent: true);
	}

	public void LoadSaveData(TeleporterData td, System.Guid currentLoc) {
		toId = td.toId;
		transform.position = td.position.val;
		destination = SaveGame.currentGame.map.locations[toId].name;
		otherSide = SaveGame.currentGame.map.locations[toId].teleporters
				.Where(x => x.toId == currentLoc && x.tag == td.tag).First();
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