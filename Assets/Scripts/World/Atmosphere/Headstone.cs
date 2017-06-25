using UnityEngine;

public class Headstone : MonoBehaviour {
    private HeadstoneSaveData data;

    public HeadstoneSaveData SaveData() {
        return data;
    }

    public void LoadSaveData(HeadstoneSaveData data) {
        if (data.frame == -1) {
            int max = GetComponent<PicaVoxel.Volume>().Frames.Count;
            data.frame = Random.Range(0, max);
        }
        GetComponent<PicaVoxel.Volume>().SetFrame(data.frame);
        this.data = data;
    }

    public class HeadstoneSaveData {
        public int tile;
        public string message;
        public int frame;
        public HeadstoneSaveData(int tile, string message) {
            this.tile = tile;
            this.message = message;
            frame = -1;
        }
    }
}