using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class VisualMap : MonoBehaviour {

	public static VisualMap instance;
	public Text txt;
	private Dictionary<System.Guid, GameObject> txtObjects = new Dictionary<System.Guid, GameObject>();

	public Color defaultColor;
	public Color currentLocationColor;
	public Color questDestinationColor;

	public void Awake() {
		instance = this;
	}

	public void Draw() {
		Map m = SaveGame.currentGame.map;
		float scale = 1f/Map.WORLD_COORD_SIZE;
		Vector3 offset = new Vector3(transform.localScale.x, 0, transform.localScale.z) * txt.transform.parent.localScale.x / 2f;	
		foreach (var kv in m.locations.Where(x => x.Value.onMap)) {
			GameObject newtxt = Instantiate(txt.gameObject, transform.position, txt.transform.rotation) as GameObject;
			newtxt.transform.SetParent(txt.transform.parent);
			newtxt.name = kv.Value.name;
			newtxt.GetComponent<Text>().text = (kv.Value.icon.Length > 0 ? kv.Value.icon + "\n" : "") + kv.Value.name;
			// newtxt.GetComponent<Text>().color = currentLocationColor;
			newtxt.transform.localPosition = kv.Value.worldLocation.val * scale;
			newtxt.transform.position -= offset;
			txtObjects.Add(kv.Key, newtxt);
		}
		Destroy(txt.gameObject);
	}

	private List<System.Guid> marked = new List<System.Guid>();
	public void MarkQuestDestinations(List<Task.TaskDestination> destinations) {
		destinations.ForEach(x => marked.Remove(x.location));
		foreach (System.Guid id in marked) {
			txtObjects[id].GetComponent<Text>().color = defaultColor;
			Debug.Log("reset color for " + id);			
		}
		foreach (var kv in destinations) {
			txtObjects[kv.location].GetComponent<Text>().color = questDestinationColor;
			Debug.Log("marked quest for " + kv.location);			
		}
		txtObjects[Map.CurrentLocation().guid].GetComponent<Text>().color = currentLocationColor;
		marked = destinations.Select(x => x.location).ToList();
		marked.Add(Map.CurrentLocation().guid);
	}
}
