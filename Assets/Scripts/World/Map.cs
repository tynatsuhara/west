using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Map {

	/*
		Map generation strategy:
		Generate concentric rings of towns for an ever-expanding map
	 */
	private const int MIN_DISTANCE_BETWEEN_LOCATIONS = 5;
	private const int MAX_CONNECTING_DISTANCE = 15;
	private const int TOWN_PLACEMENT_ATTEMPTS = 10;

	private const int RING_SIZE = 5;  // the thiccness of each ring in the map
	private const int RING_BUFFER = 2;  // how many rings out to spawn
	private const float TOWNS_PER_SQUARE_UNIT = 1f / 25;  // TODO what is this val?

	private int furthestRing = 1; // the furthest ring of towns which has been spawned (innermost ring is 1)

	// coordinates increase up and to the right
	public Dictionary<System.Guid, Location> locations = new Dictionary<System.Guid, Location>();
	private List<TownLocation> towns = new List<TownLocation>();

	public System.Guid currentLocation;

	// Infinite map (start with some towns pre-generated)
	public IEnumerator MakeMap(Text display) {
		TownLocation startTown = new TownFactory(this).NewLargeTown();
		startTown.PlaceAt(0, 0);
		towns.Add(startTown);
		locations.Add(startTown.guid, startTown);
		currentLocation = startTown.guid;
		Generate(startTown);
		yield return new WaitForEndOfFrame();
	}

	private int TownRing(TownLocation town) {
		return (int) (town.worldLocation.val.magnitude / RING_SIZE) + 1;
	}

	private void PlaceTowns(int ring) {
		if (ring <= furthestRing) {
			return;
		}
		int townAmount = (int) ((Mathf.Pow(ring * RING_SIZE, 2) - Mathf.Pow((ring-1) * RING_SIZE, 2)) * Mathf.PI * TOWNS_PER_SQUARE_UNIT);
		Debug.Log("ring " + ring + ": attempting to spawn " + townAmount + " towns");
		int successful = Enumerable.Range(0, townAmount)
								   .Where(x => TryPlaceNewTown(ring))
								   .Count();
		Debug.Log("successfully spawned " + successful + " towns");
		furthestRing = ring;
	}

	private bool TryPlaceNewTown(int ring) {
		TownLocation n = NewTown();
		int i = 0;
		do {
			Vector2 pos = (Vector2) Random.insideUnitCircle.normalized * Random.Range(1f * ring-1, 1f * ring) * RING_SIZE;
			n.PlaceAt(pos.x, pos.y);
			i++;
		} while (towns.Any(x => x.DistanceFrom(n) < MIN_DISTANCE_BETWEEN_LOCATIONS) && i < TOWN_PLACEMENT_ATTEMPTS);
		if (i == TOWN_PLACEMENT_ATTEMPTS) {
			return false;
		}
		towns.Add(n);
		locations.Add(n.guid, n);
		return true;
	}

	private void Generate(TownLocation town) {
		if (town.generated) {
			return;
		}
		Debug.Log("generating " + town.name);

		int ring = TownRing(town);
		for (int i = ring; i < ring + RING_BUFFER; i++) {  // expand the map if necessary
			PlaceTowns(i + 1);
		}

		List<TownLocation> adj = towns.Where(x => !x.generated && x.CanConnectTo(town) && town.DistanceFrom(x) < MAX_CONNECTING_DISTANCE)
								  	  .OrderBy(x => x.connections.Count)
								      .ToList();
		Debug.Log("found " + adj.Count + " adjacent towns to " + town.name);

		for (int i = 0; i < Mathf.Min(adj.Count, town.availableConnections); i++) {
			town.Connect(adj[i]);
			adj[i].Connect(town);
		}

		town.Generate();
		Debug.Log("generated " + town.name);
	}

	// Visiting a town ensures that all adjacent connected towns are generated
	public void Visit(System.Guid id) {
		foreach (System.Guid c in locations[id].connections) {
			Location l = locations[c];
			if (l is TownLocation) {
				Generate(l as TownLocation);
			}
		}
		Debug.Log("visited " + locations[id].name);
	}

	private TownLocation NewTown() {
		TownFactory tf = new TownFactory(this);
		int val = Random.Range(0, 10);
		if (val < 2) {
			string gangName;
			do {
				gangName = NameGen.GangName(NameGen.CharacterFirstName(), NameGen.CharacterLastName());
				Debug.Log("new gang name " + gangName);				
			} while (SaveGame.currentGame.groups.ContainsKey(gangName));
			SaveGame.currentGame.groups.Add(gangName, new Group(gangName));			
			return tf.NewGangTown(gangName);
		} else if (val < 6) {
			return tf.NewLargeTown();
		} else {
			return tf.NewSmallTown();
		}
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