using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour {

	public System.Guid toId;
	public string destination;
	public TeleporterData otherSide;
	public float radius {
		get { return GetComponent<SphereCollider>().radius; }
	}

	private bool ignore = true;
	private HashSet<Character> collidingWith = new HashSet<Character>();

	void Awake() {
		StartCoroutine(Delay());
	}

	void OnTriggerEnter(Collider other) {
		if (ignore)
			return;
		Player pc = other.GetComponentInParent<Player>();		
		if (pc != null) {
			collidingWith.Add(pc);
			UpdateText();
		}
	}
	void OnTriggerExit(Collider other) {
		if (ignore)
			return;
		Player pc = other.GetComponentInParent<Player>();		
		if (pc != null && collidingWith.Contains(pc)) {
			collidingWith.Remove(pc);
			UpdateText();
		}
	}

	public bool CollidingWith(Character c) {
		return collidingWith.Contains(c);
	}

	public void Teleport(Character character) {
		if (character is Player) {
			GameManager.instance.loadReposition = otherSide.position.val;
			float dist = Map.CurrentLocation().DistanceFrom(Map.Location(toId));
			// loading location autosaves before transport, so make sure to do any save modification (eg moving horses) after location load
			GameManager.instance.LoadLocation(toId, 4 /* minutes per distance unit */ * dist * character.moveSpeed / character.CalculateSpeed());
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

	private void UpdateText() {
		string str = "";
		if (collidingWith.Count > 0) {
			str += destination;
		} else if (hasQuest) {
			str = "*";
		} else {
			str = "";
		}
		GetComponentInChildren<TextObject>().Say(str, color: hasQuest ? "red" : "white", permanent: true);
	}

	// Mark if the player should go this way for a quest
	private bool hasQuest;
	public void MarkQuest(bool hasQuest) {
		this.hasQuest = hasQuest;
		UpdateText();
	}

	public void LoadSaveData(TeleporterData td, System.Guid currentLoc) {
		toId = td.toId;
		destination = Map.Location(toId).name;
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
		public bool connectRoad;

		public TeleporterData(System.Guid toId, Vector3 position, string tag = "", bool connectRoad = true) {
			this.toId = toId;
			this.position = new SerializableVector3(position);
			this.tag = tag;
			this.connectRoad = connectRoad;
		}
	}
}