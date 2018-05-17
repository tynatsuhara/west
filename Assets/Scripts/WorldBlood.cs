using UnityEngine;
using System.Collections;
using System.Linq;

public class WorldBlood : MonoBehaviour {

	public static WorldBlood instance;
	public static float bloodMultiplier = 1f;

	private Texture2D texture;
	private bool needsApply;

	// for pride cheat (rainbow blood)
	private Color32[] options = {
		new Color32(255, 94, 94, 255),
		new Color32(255, 160, 94, 255),
		new Color32(246, 255, 0, 255),
		new Color32(94, 255, 108, 255),
		new Color32(115, 173, 255, 255),
		new Color32(255, 115, 255, 255)
	};

	void Awake() {
		instance = this;
	}

	void LateUpdate() {
		if (needsApply) {
			texture.Apply();
			needsApply = false;
		}
	}

	// width and height (in tiles)
	public void NewBloodTexture(int width, int height) {
		int w = width * LevelBuilder.TILE_SIZE * 10;
		int h = height * LevelBuilder.TILE_SIZE * 10;
		texture = new Texture2D(w, h);
		texture.SetPixels(Enumerable.Range(0, w * h).Select(x => Color.clear).ToArray());		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		Material mat = GetComponent<Renderer>().material;
		mat.mainTexture = texture;
		texture.Apply();
	}
	
	public void BleedFrom(GameObject bleeder, Vector3 worldLocation, bool randomizePosition = true) {
		if (bloodMultiplier == 0)
			return;
		Vector3 circlePos = Random.insideUnitCircle * (randomizePosition ? Random.Range(.3f, .5f) : 0);
		Vector3 pos = worldLocation - new Vector3(circlePos.x, 0f, circlePos.y);
		Color c = BloodColor();
		c.a = 255;
		texture.SetPixel((int)(pos.x * 10), (int)(pos.z * 10), c);
		needsApply = true;
	}

	public Color32 BloodColor() {
		if (Cheats.instance != null && Cheats.instance.IsCheatEnabled("pride")) {
			return options[Random.Range(0, options.Length)];
		}

		byte gb = (byte)Random.Range(0, 30);
		return new Color32((byte)(120 + Random.Range(0, 60)), gb, gb, 0);
	}
}
