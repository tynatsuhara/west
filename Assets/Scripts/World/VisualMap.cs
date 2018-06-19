using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class VisualMap : MonoBehaviour {

	public static VisualMap instance;
	public VisualMapLocation iconPrefab;
	private Dictionary<System.Guid, VisualMapLocation> icons = new Dictionary<System.Guid, VisualMapLocation>();
	private Camera cam;

	public float scale = 1f / 30;	

	public bool active {
		get { return cam.enabled; }
		set {
			cam.enabled = value;
		}
	}

	public void Awake() {
		instance = this;
		cam = GetComponentInChildren<Camera>();
	}

	// spawns icons for any locations not already on the map
	public void SpawnIcons() {
		Map m = SaveGame.currentGame.map;
		foreach (var kv in m.locations.Where(x => x.Value.onMap && !icons.ContainsKey(x.Key))) {
			GameObject newIcon = Instantiate(iconPrefab.gameObject, 
											 iconPrefab.transform.position, 
											 iconPrefab.transform.rotation, 
											 iconPrefab.transform.parent);
			newIcon.name = kv.Value.name;
			icons.Add(kv.Key, newIcon.GetComponent<VisualMapLocation>());
		}
	}

	public void Refresh() {
		SpawnIcons();
		Vector3 centerOffset = Map.CurrentLocation().worldLocation.val;
		foreach (var kv in icons) {
			Location l = Map.Location(kv.Key);

			kv.Value.transform.localPosition = iconPrefab.transform.localPosition + (l.worldLocation.val - centerOffset) * scale;
			kv.Value.RefreshText(l);
			kv.Value.RefreshBirdsEyeView(l);
		}
	}

	private List<System.Guid> marked = new List<System.Guid>();
	public void MarkQuestDestinations(List<Task.TaskDestination> destinations) {
		destinations = destinations.Where(x => Map.Location(x.location).onMap).ToList();
		destinations.ForEach(x => marked.Remove(x.location));

		foreach (System.Guid id in marked) {
			icons[id].Unmark();
		}
		marked = destinations.Select(x => x.location).ToList();

		foreach (var kv in destinations) {
			icons[kv.location].MarkQuest();
		}

		System.Guid toMark = Map.CurrentTown().guid;
		icons[toMark].MarkCurrentLocation();
		marked.Add(toMark);
	}
}
