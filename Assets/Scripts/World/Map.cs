using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Map {

	public const int WORLD_COORD_SIZE = 100;
	public const int LOCATION_AMOUNT = 30;
	public const int MIN_DISTANCE_BETWEEN_LOCATIONS = 5;

	// Chunk coordinates increase up and to the right
	public Dictionary<System.Guid, Location> locations;

	public Map() {
		locations = new Dictionary<System.Guid, Location>();
		
		List<Location> ls = new List<Location>();
		for (int i = 0; i < LOCATION_AMOUNT; i++) {
			Location l = new Location(Random.Range(0, WORLD_COORD_SIZE), Random.Range(0, WORLD_COORD_SIZE));
			ls.Add(l);
		}

		for (int i = ls.Count - 1; i > 0; i--) {
			for (int j = i - 1; j >= 0; j--) {
				if (ls[i].DistanceFrom(ls[j]) < MIN_DISTANCE_BETWEEN_LOCATIONS) {
					ls.RemoveAt(i);
					break;
				}
			}
		}

		Debug.Log("generated " + ls.Count + " towns");
	}


	[System.Serializable]
	public class Location {
		public System.Guid guid;
		private SerializableVector3 worldLocation;
		private System.Guid[] connections;

		public Location(float x, float y) {
			this.guid = System.Guid.NewGuid();
			this.worldLocation = new SerializableVector3(new Vector3(x, y, 0));
			connections = new System.Guid[Random.Range(1, 6)];
		}

		public bool CanConnectTo(Location l) {
			return connections.Contains(System.Guid.Empty) && l.connections.Contains(System.Guid.Empty);
		}

		public void Connect(Location l) {
			int index = System.Array.IndexOf(connections, System.Guid.Empty);
			int indexL = System.Array.IndexOf(l.connections, System.Guid.Empty);
			connections[index] = l.guid;
			l.connections[indexL] = guid;
		}

		public float DistanceFrom(Location l) {
			return (l.worldLocation.val - worldLocation.val).magnitude;
		}
	}
 }