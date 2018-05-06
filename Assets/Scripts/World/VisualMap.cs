using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class VisualMap : MonoBehaviour {

	public static VisualMap instance;
	public GameObject iconPrefab;
	private Dictionary<System.Guid, VisualMapLocation> icons = new Dictionary<System.Guid, VisualMapLocation>();
	private Camera cam;

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

	public void SpawnIcons() {
		Map m = SaveGame.currentGame.map;
		float scale = 1f/Map.WORLD_COORD_SIZE;
		Vector3 offset = new Vector3(transform.localScale.x, 0, transform.localScale.z) * iconPrefab.transform.parent.localScale.x / 2f;	
		foreach (var kv in m.locations.Where(x => x.Value.onMap)) {
			GameObject newIcon = Instantiate(iconPrefab.gameObject, transform.position, iconPrefab.transform.rotation) as GameObject;
			newIcon.transform.SetParent(iconPrefab.transform.parent);
			newIcon.name = kv.Value.name;
			newIcon.transform.localPosition = kv.Value.worldLocation.val * scale;
			newIcon.transform.position -= offset;

			VisualMapLocation vml = newIcon.GetComponent<VisualMapLocation>();
			vml.RefreshText(kv.Value);
			icons.Add(kv.Key, vml);
		}
		Destroy(iconPrefab);
	}

	public void Refresh() {
		if (iconPrefab != null) {  // we haven't spawned the icons yet
			return;
		}
		foreach (var kv in icons) {
			kv.Value.RefreshText(Map.Location(kv.Key));
			kv.Value.RefreshBirdsEyeView(Map.Location(kv.Key).mapRender);
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
