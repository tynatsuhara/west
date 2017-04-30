using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Map {

	public const int WORLD_COORD_SIZE = 100;
	public const int MAX_LOCATION_AMOUNT = 30;
	public const int MIN_LOCATION_AMOUNT = 15;
	public const int MIN_DISTANCE_BETWEEN_LOCATIONS = 5;

	// coordinates increase up and to the right
	public Dictionary<System.Guid, Location> locations;
	public List<System.Guid[]> railroads = new List<System.Guid[]>();	
	public System.Guid currentLocation;

	public Map() {
		locations = new Dictionary<System.Guid, Location>();		
		while (locations.Count < MIN_LOCATION_AMOUNT) {
			locations.Clear();

			// spawn locations
			List<Location> ls = new List<Location>();
			for (int i = 0; i < MAX_LOCATION_AMOUNT; i++) {
				Location l = new Location(this, Random.Range(0, WORLD_COORD_SIZE), Random.Range(0, WORLD_COORD_SIZE));
				for (int j = 0; j < 10 && TooClose(ls, l); j++) {
					l = new Location(this, Random.Range(0, WORLD_COORD_SIZE), Random.Range(0, WORLD_COORD_SIZE));
				}
				if (!TooClose(ls, l)) {
					ls.Add(l);
				}
			}

			// connect locations together
			foreach (Location l in ls) {
				var others = ls.OrderBy(x => (l.worldLocation.val - x.worldLocation.val).magnitude).ToList();
				foreach (Location o in others) {
					if (l == o)
						continue;
					if (l.CanConnectTo(o) && o.CanConnectTo(l)) {
						l.Connect(o);
						o.Connect(l);
					}
					if (l.DoneConnecting())
						break;
				}
			}

			foreach (Location l in ls)
				locations.Add(l.guid, l);

			// Find largest connected map
			List<Location> graph = ls.Select(x => DFS(x))
									.OrderBy(x => x.Count)
									.Reverse().First();
			locations.Clear();
			foreach (Location l in graph) {
				locations.Add(l.guid, l);
			}
		}

		// generate train paths
		// int trainAmount = Random.Range(1, 4);
		// for (int i = 0; i < trainAmount; i++) {
		// 	// Random start location
		// 	Debug.Log("starting to generate train");
		// 	System.Guid start = locations[locations.Keys.ToArray()[Random.Range(0, locations.Count)]].guid;
		// 	List<System.Guid[]> destinations = locations.Where(x => x.Key != start)
		// 											    .Select(x => BestPathFrom(start, x.Key).ToArray())
		// 											    .OrderBy(x => x.Length)
		// 											    .ToList();
		// 	railroads.Add(destinations[Random.Range(destinations.Count / 2, destinations.Count)]);
		// 	Debug.Log("finished generating train");			
		// }

		foreach (System.Guid g in locations.Keys) {
			locations[g].Generate();
			currentLocation = g;			
		}
		Debug.Log("Generated " + locations.Count + " towns");
	}

	private bool TooClose(List<Location> locations, Location newL) {
		foreach (Location l in locations) {
			if (l.DistanceFrom(newL) < MIN_DISTANCE_BETWEEN_LOCATIONS) {
				return true;				
			}
		}
		return false;
	}

	private List<Location> DFS(Location l, List<Location> outGraph = null) {
		if (outGraph == null)
			outGraph = new List<Location>();
		outGraph.Add(l);
		foreach (System.Guid l2 in l.connections) {
			if (l2 == System.Guid.Empty || outGraph.Contains(locations[l2]))
				continue;
			DFS(locations[l2], outGraph);
		}
		return outGraph;
	}

	public List<System.Guid> BestPathFrom(System.Guid start, System.Guid destination) {
		Location src = locations[start];
		Location dst = locations[destination];

		Dictionary<Location, int> dist = new Dictionary<Location, int>();
		foreach (Location l in locations.Values)
			dist.Add(l, int.MaxValue);
		dist[src] = 0;

		Dictionary<Location, Location> prev = new Dictionary<Location, Location>();
		foreach (Location l in locations.Values)
			prev.Add(l, null);
		
		List<Location> q = locations.Values.ToList();

		while (q.Count > 0) {
			Location u = q.OrderBy(x => dist[x]).First();
			if (u == dst) {
				List<Location> path = new List<Location>();
				path.Add(u);
				while (prev[path[0]] != src) {  // backtrack
					path.Insert(0, prev[path[0]]);
				}
				return path.Select(x => x.guid).ToList();
			}
			q.Remove(u);

			foreach (System.Guid vg in u.connections) {
				Location v = locations[vg];
				int alt = dist[u] + (int)(u.worldLocation.val - v.worldLocation.val).magnitude;
				if (alt < dist[v]) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		return null;
	}

	public static Location Location(System.Guid id) {
		return SaveGame.currentGame.map.locations[id];
	}

	public static Location CurrentLocation() {
		return Location(SaveGame.currentGame.map.currentLocation);
	}

	public static Location LocationOfCharacter(System.Guid guid) {
		foreach (var item in SaveGame.currentGame.map.locations) {
			if (item.Value.characters.Contains(guid)) {
				return item.Value;
			}
		}
		return null;
	}
 }