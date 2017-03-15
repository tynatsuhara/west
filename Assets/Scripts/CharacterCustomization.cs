﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Abandon hope all ye who enter here
 */

public class CharacterCustomization : MonoBehaviour {

	public Color32 skinColor;
	public Color32 hairColor;

	public Accessory[] hair;
	public Accessory[] accessory;
	public string[] outfitNames;

	// Each character component
	public PicaVoxel.Volume head;
	public PicaVoxel.Volume body;
	public PicaVoxel.Volume legs;
	public PicaVoxel.Volume arms;
	public PicaVoxel.Volume[] gunz;

	private List<PicaVoxel.Volume> spawnedAccessories;
	public void ColorCharacter(Outfits.Outfit outfit, bool randomize = false, Accessory[] accessories = null) {
		if (spawnedAccessories != null) {
			foreach (PicaVoxel.Volume vol in spawnedAccessories) {
				if (vol == null)
					continue;
				Destroy(vol.gameObject);
			}
		}

		if (randomize) {
			Color32[] skinColors = CharacterCustomizationMenu.instance.skinColors;
			Color32[] hairColors = CharacterCustomizationMenu.instance.skinColors;
			hairColor = hairColors[Random.Range(0, hairColors.Length)];
			skinColor = skinColors[Random.Range(0, skinColors.Length)];
			accessories = new Accessory[] {
				hair[Random.Range(0, hair.Length)], 
				accessory[Random.Range(0, accessory.Length)] 
			};
		}

		List<Accessory> accPrefabs = new List<Accessory>(outfit.accessories);
		if (accessories != null)
			accPrefabs.AddRange(accessories);
		spawnedAccessories = SpawnAccessories(accPrefabs);

		var palettes = Parse(outfit, accPrefabs.ToArray());

		List<PicaVoxel.Volume> volumez = new List<PicaVoxel.Volume>(new PicaVoxel.Volume[] { body, head, legs, arms });
		volumez.AddRange(gunz);
		volumez.AddRange(spawnedAccessories);

		for (int i = 0; i < volumez.Count; i++) {
			PicaVoxel.Volume volume = volumez[i];
			Dictionary<byte, Color32> palette = palettes[i];
			if (volume == null)
				continue;

			foreach (PicaVoxel.Frame frame in volume.Frames) {
				for (int x = 0; x < frame.XSize; x++) {
					for (int y = 0; y < frame.YSize; y++) {
						for (int z = 0; z < frame.ZSize; z++) {
							PicaVoxel.Voxel? voxq = frame.GetVoxelAtArrayPosition(x, y, z);
							PicaVoxel.Voxel vox = (PicaVoxel.Voxel)voxq;
							if (voxq == null || vox.State != PicaVoxel.VoxelState.Active)
								continue;

							if (palette != null && palette.ContainsKey(vox.Value)) {
								Color32 c = palette[vox.Value];
								// DISCOLORATION FACTOR (maybe disable this randomness for later optimization)
								int r = 8;
								vox.Color = new Color32(JiggleByte(c.r, r), JiggleByte(c.g, r), JiggleByte(c.b, r), (byte)0);
							} else if (vox.Value == 255) {
								// guts
								vox.Color = WorldBlood.instance.BloodColor();
							} else if (volume == head || volume == body || volume == legs ||
								(volume == arms && vox.Value <= 4) || (gunz.Contains(volume) && vox.Value <= 4)) {
								vox.Color = skinColor;
							}
							if ((vox.Value == 37 || vox.Value == 40) && 
									(!palette.ContainsKey(vox.Value) || palette[vox.Value].Equals(skinColor))) {
								vox.Color = Outfits.HexParse("1F1F1F00");
							}
							frame.SetVoxelAtArrayPosition(new PicaVoxel.PicaVoxelPoint(new Vector3(x, y, z)), vox);
						}
					}
				}
				frame.UpdateChunks(true);
			}
		}
	}

	private List<PicaVoxel.Volume> SpawnAccessories(List<Accessory> accs) {
		List<PicaVoxel.Volume> res = new List<PicaVoxel.Volume>();
		foreach (Accessory a in accs) {
			if (a == null)
				continue;
			GameObject go = Instantiate(a.gameObject) as GameObject;
			go.transform.parent = transform.root;
			go.transform.localPosition = a.positionOffset;
			go.transform.localEulerAngles = Vector3.up * 180;
			if (a.headParent)
				go.transform.parent = GetComponentInParent<Character>().head.transform;
			else if (a.bodyParent)
				go.transform.parent = GetComponentInParent<Character>().body.transform;
			res.Add(go.GetComponentInParent<PicaVoxel.Volume>());
		}
		return res;
	}

	// Randomizes a byte and clamps it between 0 and 255
	private byte JiggleByte(byte b, int jiggleFactor) {
		return (byte)Mathf.Clamp(b + Random.Range(0, jiggleFactor + 1), 0, 255);
	}

	private Dictionary<byte, Color32>[] Parse(Outfits.Outfit outfit, Accessory[] accessories) {

		// Old obsolete trash. Mostly, you can just ignore this. Clean up when you're feeling less lazy ;)
		Color32[] colors = {
			outfit.colors[0],
			outfit.colors[1],
			outfit.colors[2],
			outfit.colors[3],
			outfit.colors[4],
			outfit.colors[5],
			skinColor,
			hairColor
		};

		List<string> outfit_ = new List<string>(outfit.pattern);
		// arms and guns use the same palette	
		foreach (PicaVoxel.Volume g in gunz)
			outfit_.Add(outfit.pattern[3]);

		if (accessories != null) {
			outfit_.AddRange(accessories.Where(i => i != null).Select(i => i.colorString));
			foreach (Accessory a in accessories) {
				if (a == null)
					continue;
				// apply accessory coloring to the head and body
				if (a.bodyString != null && a.bodyString.Length > 0) {
					outfit_[0] += ';' + a.bodyString;
				}
				if (a.headString != null && a.headString.Length > 0) {
					outfit_[1] += ';' + a.headString;
				}
			}
		}

		Dictionary<byte, Color32>[] res = new Dictionary<byte, Color32>[outfit_.Count];

		for (int j = 0; j < outfit_.Count; j++) {
			var palette = outfit_[j];
			Dictionary<Color32, byte[]> dict = new Dictionary<Color32, byte[]>();
			List<string> strings = palette.Split(';').Where(x => x.Length > 0).ToList();
			foreach (string s in strings) {
				string[] ranges = s.Trim().Split(' ');

				Color32 color = ranges[0].Length > 1 ? Outfits.HexParse(ranges[0]) : colors[int.Parse(ranges[0])];

				if (!dict.ContainsKey(color))
					dict.Add(color, new byte[0]);
				for (int i = 1; i < ranges.Length; i++) {
					if (ranges[i].Contains("-")) {
						string[] ab = ranges[i].Split('-');
						byte a = (byte)int.Parse(ab[0]);
						byte b = (byte)int.Parse(ab[1]);
						dict[color] = Merge(dict[color], Range(a, b));
					} else {
						byte a = (byte)int.Parse(ranges[i]);
						dict[color] = Merge(dict[color], new byte[]{ a });
					}
				}
			}
			res[j] = ByteKeyMap(dict);
		}

		return res;
	}

	// Takes a dict from ints to byte[] and returns a dict
	// mapping bytes to ints
	private static Dictionary<byte, Color32> ByteKeyMap(Dictionary<Color32, byte[]> dict) {
		Dictionary<byte, Color32> res = new Dictionary<byte, Color32>();
		foreach (Color32 color in dict.Keys) {
			foreach (byte b in dict[color]) {
				if (!res.ContainsKey(b))
					res.Add(b, color);
				res[b] = color;
			}
		}
		return res;
	}

	// Returns a byte array from start to end (both inclusive)
	private static byte[] Range(int start, int end) {
		byte startByte = (byte)start;
		byte endByte = (byte)end;
		byte[] res = new byte[end - start + 1];
		for (byte i = startByte; i <= endByte; i++) {
			res[i - start] = i;
		}
		return res;
	}

	private static byte[] Merge(params byte[][] lists) {
		int len = 0;
		foreach (byte[] b in lists) {
			len += b.Length;
		}
		byte[] res = new byte[len];
		int index = 0;
		foreach (byte[] bytes in lists) {
			foreach (byte b in bytes) {
				res[index++] = b;
			}
		}
		return res;
	}

	// Private helper method for testing
	private static void DebugBytes(byte[] bytes) {
		if (bytes.Length == 0) {
			Debug.Log("[]");
		} else if (bytes.Length == 1) {
			Debug.Log("[" + (int)bytes[0] + "]");
		} else {
			string s = "[" + (int)bytes[0];
			for (int i = 1; i < bytes.Length; i++) {
				s += "," + (int)bytes[i];
			}
			Debug.Log(s + "]");
		}
	}
}
