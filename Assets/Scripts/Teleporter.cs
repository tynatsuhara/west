using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour {

	public System.Guid toId;
	public string destination;
	public TeleporterData otherSide;  // TODO: make this optional (one-way teleporters)
	public bool permitHorses;
	public float radius {
		get { return GetComponent<SphereCollider>().radius; }
	}

	private bool ignore = false;
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
			// ignore = true;
		} else {
			// TODO: save character to the other location
		}
		if (character.ridingHorse) {
			Horse.HorseSaveData hsd = character.mount.SaveData();
			Map.CurrentLocation().horses.Remove(hsd.guid);
			Map.Location(toId).horses.Add(hsd.guid);
		}
	}

	private bool mouseOver;
	void OnMouseEnter() {
        mouseOver = true;
		UpdateText();
    }

    void OnMouseExit() {
        mouseOver = false;
		UpdateText();
    }

	private void UpdateText() {
		string str = "";
		if (mouseOver || collidingWith.Count > 0) {
			str += destination;
		} else if (hasQuest) {
			str = Quest.QUEST_DEFAULT_ICON;
		} else {
			str = "";
		}
		// todo: show this separately for each character
		GetComponentInChildren<TextObject>().Say(str, color: hasQuest ? Quest.QUEST_MARKER_COLOR : "white", permanent: true);
	}

	// Mark if the player should go this way for a quest
	private bool hasQuest;
	public void MarkQuest(bool hasQuest) {
		this.hasQuest = hasQuest;
		UpdateText();
	}
	public bool HasQuest() {
		return hasQuest;
	}

	public void LoadSaveData(TeleporterData td, System.Guid currentLoc) {
		toId = td.toId;
		destination = Map.Location(toId).name;
		otherSide = SaveGame.currentGame.map.locations[toId].teleporters
				.Where(x => x.toId == currentLoc /* && x.tag == td.tag */).First();
		permitHorses = td.permitHorses;
	}

	private IEnumerator Delay() {
		// yield return new WaitForSeconds(1f);
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
		public bool permitHorses;

		public TeleporterData(System.Guid toId, Vector3 position, string tag = "", bool connectRoad = true, bool permitHorses = true) {
			this.toId = toId;
			this.position = new SerializableVector3(position);
			this.tag = tag;
			this.connectRoad = connectRoad;
			this.permitHorses = permitHorses;
		}
	}
}