using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Map {

	public const int WORLD_COORD_SIZE = 100;
	public const int MIN_LOCATION_AMOUNT = 20;
	public const int MIN_DISTANCE_BETWEEN_LOCATIONS = 5;

	// coordinates increase up and to the right
	public Dictionary<System.Guid, Location> locations = new Dictionary<System.Guid, Location>();
	public List<System.Guid[]> railroads = new List<System.Guid[]>();	
	public System.Guid currentLocation;

	public IEnumerator MakeMap(Text display) {
		locations = new Dictionary<System.Guid, Location>();	
		TownFactory tf = new TownFactory(this);

		while (locations.Count < MIN_LOCATION_AMOUNT) {
			SaveGame.currentGame.groups = DefaultGroups();

			locations.Clear();
			List<TownLocation> ls = new List<TownLocation>();
			HashSet<string> gangs = new HashSet<string>();
			
			display.text = "PLACING GANG HIDEOUTS";
			List<TownLocation> spawnedTowns = TrySpawnTowns(ls, Random.Range(5, 10), () => {
				string gangName;
				do {
					gangName = NameGen.GangName(NameGen.CharacterFirstName(), NameGen.CharacterLastName());
				} while (gangs.Contains(gangName));
				gangs.Add(gangName);
				return tf.NewGangTown(gangName);
			});
			spawnedTowns.ForEach(t => {
				t.discovered = true;
				SaveGame.currentGame.groups.Add(t.controllingGroup, new Group(t.controllingGroup));
			});
			Debug.Log("spawned " + spawnedTowns.Count + " gang towns: " + spawnedTowns.Select(x => x.name).Aggregate((a, b) => a + ", " + b));

			display.text = "PLACING TOWNS";
			TrySpawnTowns(ls, Random.Range(10, 20), () => tf.NewLargeTown());
			TrySpawnTowns(ls, Random.Range(10, 20), () => tf.NewSmallTown());

			// connect locations together
			// TODO: connect each thing to the closest town WHICH IS ALREADY PART OF THE GRAPH
			//       this will allow us to skip pruning and guarantee the amount of towns
			display.text = "BUILDING ROADS";
			yield return 0;
			var distances = ls.SelectMany(l1 => ls.Select(l2 => new {l1, l2, (l1.worldLocation.val - l2.worldLocation.val).magnitude}))
					.OrderBy(x => x.magnitude)
					.ToList();

			// TODO: don't make all connections (infinite map)
			foreach (var tuple in distances) {
				if (tuple.l1 != tuple.l2 && tuple.l1.CanConnectTo(tuple.l2) && tuple.l2.CanConnectTo(tuple.l1)) {
					tuple.l1.Connect(tuple.l2);
					tuple.l2.Connect(tuple.l1);
				}
			}

			foreach (TownLocation l in ls)
				locations.Add(l.guid, l);

			// Find largest connected map
			display.text = "PRUNING MAP";
			yield return 0;
			HashSet<Location> graph = null;
			foreach (TownLocation l in locations.Values) {
				graph = DFS(l);
				if (graph.Count >= MIN_LOCATION_AMOUNT)
					break;
				yield return 0;				
			}

			locations.Clear();
			foreach (TownLocation l in graph) {
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

		List<System.Guid> towns = locations.Keys.ToList();
		foreach (System.Guid g in towns) {
			TownLocation town = (TownLocation) locations[g];
			town.Generate();
			display.text = "GENERATING TOWN " + town.name.ToUpper();
			yield return 0;
			currentLocation = g;
		}
		display.text = "LOADING " + ((TownLocation) locations[currentLocation]).name.ToUpper();
		Debug.Log("Generated " + towns.Count + " towns, " + locations.Count + " locations total");
	}

	private Dictionary<string, Group> DefaultGroups() {
		Dictionary<string, Group> result = new Dictionary<string, Group>();
		result.Add(Group.LAW_ENFORCEMENT, new Group(Group.LAW_ENFORCEMENT));
		result.Add(Group.PLAYERS, new Group(Group.PLAYERS));
		return result;
	}

	// towns are added to ls if successfully spawned
	// successfully spawned towns are returned
	private List<TownLocation> TrySpawnTowns(List<TownLocation> ls, int amount, System.Func<TownLocation> townSupplier) {
		List<TownLocation> successful = new List<TownLocation>();
		for (int i = 0; i < amount; i++) {
			TownLocation l = townSupplier();
			for (int j = 0; j < 10 && TooClose(ls, l); j++) {
				l = townSupplier();
			}
			if (!TooClose(ls, l)) {
				ls.Add(l);
				successful.Add(l);
			}
		}
		return successful;
	}

	private bool TooClose(List<TownLocation> towns, Location newL) {
		foreach (TownLocation l in towns) {
			if (l.DistanceFrom(newL) < MIN_DISTANCE_BETWEEN_LOCATIONS) {
				return true;				
			}
		}
		return false;
	}

	private HashSet<Location> DFS(Location l, HashSet<Location> outGraph = null) {
		if (outGraph == null)
			outGraph = new HashSet<Location>();
		outGraph.Add(l);
		foreach (System.Guid l2 in l.connections) {
			if (l2 == System.Guid.Empty || outGraph.Contains(locations[l2]))
				continue;
			DFS(locations[l2], outGraph);
		}
		return outGraph;
	}

	// Returns the list (excluding start), or null if there is no path
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
			Location u = q.OrderBy(x => dist.ContainsKey(x) ? dist[x] : 0).First();
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

	public static TownLocation CurrentTown() {
		return (TownLocation)(CurrentLocation() is TownLocation ? CurrentLocation() : Location(CurrentLocation().town));
	}
 }