using UnityEngine;
using System.Collections;

public class Cactus : MonoBehaviour {
    
    public GameObject[] arms;
    private CactusSaveData data;    

    public CactusSaveData SaveData() {
        return data;
    }

    public void LoadSaveData(CactusSaveData csd) {
        data = csd;
        for (int i = 0; i < arms.Length; i++) {
            if (data.arms.Get(i)) {
                arms[i].transform.Translate(new Vector3(0, data.offsets[i % 4], 0));
            } else {
                Destroy(arms[i]);
            }
        }
        transform.Translate(new Vector3(0, data.overallOffset, 0));
        transform.RotateAround(transform.position, transform.up, data.rotation);
    }
    
    [System.Serializable]
    public class CactusSaveData {
        public BitArray arms;  // 0-3 small, 4-7 big
        public float[] offsets;  // 4 arm vertical offsets
        public float overallOffset;  // vertical offset
        public int rotation;

        public CactusSaveData() {
            for (int i = 0; i < 4; i++) {  // 4 possible arms
                if (Random.Range(0, 2) == 1) {  // put an arm on this side
                    arms.Set(i + (Random.Range(0, 2) == 1 ? 4 : 0), true);  // randomly choose between big and small
                }
            }
            offsets = new float[4];
            rotation = Random.Range(0, 360);
        }
    }
}