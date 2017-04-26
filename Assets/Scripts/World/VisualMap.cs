using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VisualMap : MonoBehaviour {

	public Vector3 offset;
	public Text txt;

	void Start () {
		Map m = SaveGame.currentGame.map;
		foreach (var kv in m.locations) {
			GameObject newtxt = Instantiate(txt.gameObject, transform.position, txt.transform.rotation) as GameObject;
			newtxt.transform.SetParent(txt.transform.parent);
			newtxt.GetComponent<Text>().text = (kv.Value.icon.Length > 0 ? kv.Value.icon + "\n" : "") + kv.Value.name;
			// newtxt.transform.localPosition = new Vector3(kv.Value.worldLocation.val.x * scale - transform.localScale.x / 2f, 
														//  0, kv.Value.worldLocation.val.y * scale - transform.localScale.z / 2f);
			float scale = 1f/Map.WORLD_COORD_SIZE;
			newtxt.transform.localPosition = kv.Value.worldLocation.val * scale;
			newtxt.transform.position -= new Vector3(transform.localScale.x/2f, 0, transform.localScale.z/2f);
		}
		Destroy(txt);
	}
	
	void Update () {
	
	}
}
