using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class VisualMap : MonoBehaviour {

	public Text txt;

	public void Draw() {
		Map m = SaveGame.currentGame.map;
		float scale = 1f/Map.WORLD_COORD_SIZE;
		Vector3 offset = new Vector3(transform.localScale.x, 0, transform.localScale.z) * txt.transform.parent.localScale.x / 2f;	
		foreach (var kv in m.locations.Where(x => x.Value.onMap)) {
			GameObject newtxt = Instantiate(txt.gameObject, transform.position, txt.transform.rotation) as GameObject;
			newtxt.transform.SetParent(txt.transform.parent);
			newtxt.name = kv.Value.name;
			newtxt.GetComponent<Text>().text = (kv.Value.icon.Length > 0 ? kv.Value.icon + "\n" : "") + kv.Value.name;
			newtxt.transform.localPosition = kv.Value.worldLocation.val * scale;
			newtxt.transform.position -= offset;
		}
		Destroy(txt.gameObject);
	}
}
