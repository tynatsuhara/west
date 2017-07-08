using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cactus : MonoBehaviour, Damageable {
    
    public GameObject bigArm;
    public GameObject lilArm;
    public GameObject flower;
    private GameObject[] arms = new GameObject[8];
    private CactusSaveData data;

    public bool Damage(Vector3 location, Vector3 angle, float damage, Character attacker = null, DamageType type = DamageType.BULLET) {
        List<GameObject> onArms = arms.Where(a => a != null && a.transform.parent == transform).ToList();
        if (onArms.Count == 0)
            return true;
        GameObject which = onArms[Random.Range(0, onArms.Count)];
        int index = arms.ToList().IndexOf(which);
        which.transform.parent = null;
        which.GetComponent<Rigidbody>().isKinematic = false;
        which.GetComponent<Rigidbody>().AddForce(angle * 5f, ForceMode.Impulse);
        which.transform.RotateAround(which.transform.position, Random.insideUnitSphere, Random.Range(10, 50));
        data.arms.Set(index, false);
        return true;
    }

    public CactusSaveData SaveData() {
        return data;
    }

    public void LoadSaveData(CactusSaveData csd) {
        data = csd;
        for (int i = 0; i < 8; i++) {
            if (data.arms.Get(i)) {
                GameObject arm = i < 4 ? bigArm : lilArm;
                arms[i] = Instantiate(arm, arm.transform.position, arm.transform.rotation);            
                arms[i].transform.Translate(new Vector3(0, data.offsets[i%4], 0));
                arms[i].transform.RotateAround(transform.position, transform.up, i * 90);     
                arms[i].transform.parent = transform;                           
            }
        }
        transform.Translate(new Vector3(0, data.overallOffset, 0));
        transform.RotateAround(transform.position, transform.up, data.rotation);
        if (!data.flower) {
            Destroy(flower);
        }
        Destroy(bigArm);
        Destroy(lilArm);
    }
    
    [System.Serializable]
    public class CactusSaveData {
        public BitArray arms = new BitArray(8);
        public float[] offsets;  // 4 arm vertical offsets
        public float overallOffset;  // vertical offset
        public int rotation;
        public bool flower;
        public int tile;

        public CactusSaveData(int tile) {
            this.tile = tile;
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
    }
}