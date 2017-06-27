using UnityEngine;
using System.Collections.Generic;

public class Headstone : MonoBehaviour {
    private HeadstoneSaveData data;

    public HeadstoneSaveData SaveData() {
        data.bytes = GetComponent<PicaVoxel.Volume>().GetBytes();
        return data;
    }

    public void LoadSaveData(HeadstoneSaveData data) {
        if (data.frame == -1) {
            int max = GetComponent<PicaVoxel.Volume>().Frames.Count;
            data.frame = Random.Range(0, max);
        }
        GetComponent<PicaVoxel.Volume>().SetFrame(data.frame);
        if (data.bytes != null) {
            GetComponent<PicaVoxel.Volume>().SetBytes(data.bytes);
        }
        this.data = data;
    }

    public class HeadstoneSaveData {
        public int tile;
        public string message;
        public int frame;
        public List<byte[]> bytes;
        public HeadstoneSaveData(int tile, string message) {
            this.tile = tile;
            this.message = message;
            frame = -1;
        }
    }
}