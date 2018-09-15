using UnityEngine;
using System.Collections.Generic;

// NEW TODO: Clean this all up

namespace WorldGameObject {
    public class Headstone : MonoBehaviour {
        private World.Headstone data;

        // TODO: when implementing headstones, make it save bytes after damage
        // public HeadstoneSaveData SaveData() {
        //     data.bytes = GetComponent<PicaVoxel.Volume>().GetBytes();
        //     return data;
        // }

        public void LoadSaveData(World.Headstone data) {
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
    }
}