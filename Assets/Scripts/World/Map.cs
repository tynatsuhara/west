using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Map {

	public const int WORLD_COORD_SIZE = 100;
	public const int LOCATION_AMOUNT = 30;
	public const int MIN_DISTANCE_BETWEEN_LOCATIONS = 7;

	// Chunk coordinates increase up and to the right
	public Dictionary<System.Guid, Location> locations;

	public Map() {
		locations = new Dictionary<System.Guid, Location>();
		
		List<Location> ls = new List<Location>();
		for (int i = 0; i < LOCATION_AMOUNT; i++) {
			Location l = new Location(Random.Range(0, WORLD_COORD_SIZE), Random.Range(0, WORLD_COORD_SIZE));
			ls.Add(l);
		}

		for (int i = ls.Count - 1; i > 0; i++) {
			for (int j = i - 1; j >= 0; j++) {
				if (ls[i].DistanceFrom(ls[j]) < MIN_DISTANCE_BETWEEN_LOCATIONS) {
					ls.RemoveAt(i);
					break;
				}
			}
		}
	}


	[System.Serializable]
	public class Location {
		public System.Guid guid;
		private SerializableVector3 worldLocation;

		public Location(float x, float y) {
			this.guid = System.Guid.NewGuid();
			this.worldLocation = new SerializableVector3(new Vector3(x, y, 0));
		}

		public float DistanceFrom(Location l) {
			return (l.worldLocation.val - worldLocation.val).magnitude;
		}
	}
 }