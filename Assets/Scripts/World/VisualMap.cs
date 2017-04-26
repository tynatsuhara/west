using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VisualMap : MonoBehaviour {

	private static float scale = .1f;
	public Vector3 offset;
	public Text txt;

	void Start () {
		Map m = SaveGame.currentGame.map;
		foreach (var kv in m.locations) {
			Vector3 pos = kv.Value.worldLocation.val;
			GameObject go = Instantiate(txt, transform.position, txt.transform.rotation) as GameObject;
			go.GetComponent<Text>().text = (kv.Value.icon.Length > 0 ? kv.Value.icon.Length + "\n" : "") + kv.Value.name;
		}
	}
	
	void Update () {
	
	}
}
