using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Location {
	public System.Guid guid = System.Guid.NewGuid();
	public string name = NameGen.TownName();
	public SerializableVector3 worldLocation;
	public System.Guid[] connections = new System.Guid[Random.Range(1, 6)];
	public System.Guid[] horses;
	public List<Teleporter.TeleporterData> teleporters = new List<Teleporter.TeleporterData>();

	public Location(float x, float y) {
		this.worldLocation = new SerializableVector3(new Vector3(x, y, 0));
		Debug.Log(name);
		
		// TEMP spawning
		horses = new System.Guid[Random.Range(1, 5)];
		for (int i = 0; i < horses.Length; i++) {
			Horse.HorseSaveData hsd = new Horse.HorseSaveData(LevelBuilder.instance.horsePrefab);
			SaveGame.currentGame.horses.Add(hsd.guid, hsd);
			horses[i] = hsd.guid;
		}
	}

	public bool DoneConnecting() {
		return !connections.Contains(System.Guid.Empty);
	}

	public bool CanConnectTo(Location l) {
		return connections.Contains(System.Guid.Empty) && l.connections.Contains(System.Guid.Empty);
	}

	public void Connect(Location l) {
		int index = System.Array.IndexOf(connections, System.Guid.Empty);
		int indexL = System.Array.IndexOf(l.connections, System.Guid.Empty);
		connections[index] = l.guid;
		l.connections[indexL] = guid;

		teleporters.Add(new Teleporter.TeleporterData(l.guid, new Vector3(index, 1, 1)));
	}

	public float DistanceFrom(Location l) {
		return (l.worldLocation.val - worldLocation.val).magnitude;
	}
}