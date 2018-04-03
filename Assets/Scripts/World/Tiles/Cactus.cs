using System.Collections;
using UnityEngine;

namespace World {
    [System.Serializable]
    public class Cactus : TileElement {
        public BitArray arms = new BitArray(8);
        public float[] offsets;  // 4 arm vertical offsets
        public float overallOffset;  // vertical offset
        public int rotation;
        public bool flower;

        public Cactus() {
            offsets = new float[4];            
            for (int i = 0; i < 4; i++) {  // 4 possible arms
                if (Random.Range(0, 3) == 1) {  // put an arm on this side
                    arms.Set(i + (Random.Range(0, 2) == 1 ? 4 : 0), true);  // randomly choose between big and small
                }
                offsets[i] = Random.Range(2, 6) * .1f;
            }
            overallOffset = Random.Range(3, 11) * .1f;
            rotation = Random.Range(0, 360);
            flower = Random.Range(0, 3) == 0;
        }

        public void Spawn(LevelBuilder lb, Location location, int x, int y) {
            GameObject c = GameObject.Instantiate(lb.cactusPrefab, location.TileVectorPosition(x, y), Quaternion.identity);
			c.GetComponent<WorldGameObject.Cactus>().LoadSaveData(this);
        }
    }
}