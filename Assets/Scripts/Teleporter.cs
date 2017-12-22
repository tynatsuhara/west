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
			GameUI.instance.topCenterText.Say("TRAVEL TO " + destination + "?", permanent: true);
		}
	}
	void OnTriggerExit(Collider other) {
		if (ignore)
			return;
		Player pc = other.GetComponentInParent<Player>();		
		if (pc != null && collidingWith.Contains(pc)) {
			collidingWith.Remove(pc);
			GameUI.instance.topCenterText.Clear();
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
			Debug.Log("Teleporting to " + otherSide.position.val);
		} else {
			// TODO: save character to the other location
		}
		if (character.ridingHorse) {
			Horse.HorseSaveData hsd = character.mount.SaveData();
			Map.CurrentLocation().horses.Remove(hsd.guid);
			Map.Location(toId).horses.Add(hsd.guid);
		}
	}

	// Mark if the player should go this way for a quest
	public void MarkQuest(bool hasQuest) {
		GetComponentInChildren<TextObject>().Say(destination, color: hasQuest ? "red" : "white", permanent: true);
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