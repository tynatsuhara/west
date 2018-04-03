using System.Collections.Generic;
using UnityEngine;

namespace World {
    [System.Serializable]
    public class Headstone : TileElement {
        public string message;
        public int frame;
        public List<byte[]> bytes;
        public Headstone(string message) {
            this.message = message;
            frame = -1;
        }

        public override void Spawn(LevelBuilder lb, Location location, int x, int y) {
            GameObject h = GameObject.Instantiate(lb.headstonePrefab, location.TileVectorPosition(x, y), Quaternion.identity);
			h.GetComponent<WorldGameObject.Headstone>().LoadSaveData(this);
        }
    }
}