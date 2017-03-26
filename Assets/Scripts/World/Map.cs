using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Map {

	public const int WORLD_COORD_SIZE = 100;
	public const int LOCATION_AMOUNT = 30;
	public const int MIN_DISTANCE_BETWEEN_LOCATIONS = 5;

	// coordinates increase up and to the right
	public Dictionary<System.Guid, Location> locations;
	public Location currentLocation;

	public Map() {

		// spawn locations
		List<Location> ls = new List<Location>();
		for (int i = 0; i < LOCATION_AMOUNT; i++) {
			Location l = new Location(Random.Range(0, WORLD_COORD_SIZE), Random.Range(0, WORLD_COORD_SIZE));
			ls.Add(l);
		}

		// remove overlapping locations
		for (int i = ls.Count - 1; i > 0; i--) {
			for (int j = i - 1; j >= 0; j--) {
				if (ls[i].DistanceFrom(ls[j]) < MIN_DISTANCE_BETWEEN_LOCATIONS) {
					ls.RemoveAt(i);
					break;
				}
			}
		}

		// connect locations together
		foreach (Location l in ls) {
			var others = ls.OrderBy(x => (l.worldLocation.val - x.worldLocation.val).magnitude).ToList();
			foreach (Location o in ls) {
				if (l.CanConnectTo(o) && o.CanConnectTo(l))
					l.Connect(o);
				if (l.DoneConnecting())
					break;
			}
		}

		locations = new Dictionary<System.Guid, Location>();
		foreach (Location l in ls)
			locations.Add(l.guid, l);

		// BFS to find connected map
		List<Location> graph = new List<Location>();
		BFS(ls[0], graph);
		locations.Clear();
		foreach (Location l in graph)
			locations.Add(l.guid, l);

		Debug.Log("generated " + ls.Count + " towns");
	}

	private void BFS(Location l, List<Location> outGraph) {
		outGraph.Add(l);
		foreach (System.Guid l2 in l.connections) {
			if (outGraph.Contains(locations[l2]))
				continue;
			BFS(locations[l2], outGraph);
		}
	}
 }