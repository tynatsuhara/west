using UnityEngine;
using UnityEngine.UI;
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
	public Dictionary<System.Guid, TownLocation> towns;	
	public Dictionary<System.Guid, Location> locations;
	public List<System.Guid[]> railroads = new List<System.Guid[]>();	
	public System.Guid currentLocation;

	public IEnumerator MakeMap(Text display) {
		locations = new Dictionary<System.Guid, Location>();	
		towns = new Dictionary<System.Guid, TownLocation>();	
		while (towns.Count < MIN_LOCATION_AMOUNT) {
			towns.Clear();

			// spawn locations
			List<TownLocation> ls = new List<TownLocation>();
			for (int i = 0; i < MAX_LOCATION_AMOUNT; i++) {
				display.text = "PLACING TOWN " + (i+1) + "/" + MAX_LOCATION_AMOUNT;
				yield return new WaitForEndOfFrame();
				TownLocation l = new TownLocation(this, Random.Range(0, WORLD_COORD_SIZE), Random.Range(0, WORLD_COORD_SIZE));
				for (int j = 0; j < 10 && TooClose(ls, l); j++) {
					l = new TownLocation(this, Random.Range(0, WORLD_COORD_SIZE), Random.Range(0, WORLD_COORD_SIZE));
				}
				if (!TooClose(ls, l)) {
					ls.Add(l);
				}
			}

			// connect locations together
			display.text = "BUILDING ROADS";
			yield return new WaitForEndOfFrame();
			foreach (TownLocation l in ls) {
				var others = ls.OrderBy(x => (l.worldLocation.val - x.worldLocation.val).magnitude).ToList().Take(10);
				foreach (TownLocation o in others) {
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

			foreach (TownLocation l in ls)
				towns.Add(l.guid, l);

			// Find largest connected map
			display.text = "PRUNING MAP";
			yield return new WaitForEndOfFrame();
			List<TownLocation> graph = null;
			foreach (TownLocation l in ls) {
				graph = DFS(l);
				if (graph.Count >= MIN_LOCATION_AMOUNT)
					break;
				yield return new WaitForEndOfFrame();				
			}
			
			towns.Clear();
			foreach (TownLocation l in graph) {
				towns.Add(l.guid, l);
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

		foreach (System.Guid g in towns.Keys) {
			locations[g] = towns[g];
		}
		foreach (System.Guid g in towns.Keys) {
			TownLocation town = towns[g];
			town.Generate();
			display.text = "GENERATING TOWN " + town.name.ToUpper();
			yield return new WaitForEndOfFrame();
			currentLocation = g;
		}
		Debug.Log("Generated " + towns.Count + " towns");
	}

	private bool TooClose(List<TownLocation> towns, Location newL) {
		foreach (TownLocation l in towns) {
			if (l.DistanceFrom(newL) < MIN_DISTANCE_BETWEEN_LOCATIONS) {
				return true;				
			}
		}
		return false;
	}

	private List<TownLocation> DFS(TownLocation l, List<TownLocation> outGraph = null) {
		if (outGraph == null)
			outGraph = new List<TownLocation>();
		outGraph.Add(l);
		foreach (System.Guid l2 in l.connections) {
			if (l2 == System.Guid.Empty || outGraph.Contains(towns[l2]))
				continue;
			DFS(towns[l2], outGraph);
		}
		return outGraph;
	}

	public List<System.Guid> BestPathFrom(System.Guid start, System.Guid destination) {
		TownLocation src = towns[start];
		TownLocation dst = towns[destination];

		Dictionary<TownLocation, int> dist = new Dictionary<TownLocation, int>();
		foreach (TownLocation l in towns.Values)
			dist.Add(l, int.MaxValue);
		dist[src] = 0;

		Dictionary<Location, Location> prev = new Dictionary<Location, Location>();
		foreach (Location l in towns.Values)
			prev.Add(l, null);
		
		List<TownLocation> q = towns.Values.ToList();

		while (q.Count > 0) {
			TownLocation u = q.OrderBy(x => dist[x]).First();
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
				TownLocation v = towns[vg];
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

	public static TownLocation CurrentTown() {
		return (TownLocation)(CurrentLocation() is TownLocation ? CurrentLocation() : Location(CurrentLocation().town));
	}

	public static Location LocationOfCharacter(System.Guid guid) {
		foreach (var item in SaveGame.currentGame.map.locations) {
			if (item.Value.characters.Contains(guid)) {
				return item.Value;
			}
		}
		return null;
	}

	public void Save() {
		foreach (Location l in locations.Values) {
			l.Save();
		}
	}
 }