using UnityEngine;
using System.Collections;

public class Tumbleweed : MonoBehaviour {
    private Vector3 wind;
    private Vector3 nextWind;
    public float windForce;
    public SphereCollider sc;
    public Color32[] colors;

    public void Start() {
        float radius = sc.radius = Random.Range(.3f, .6f);
        int density = (int) (radius * 75);
        for (int i = 0; i < density; i++) {
            // todo: put voxels randomly in sphere shape
        }

        wind = NewDirection();
        StartCoroutine(UpdateWind());
    }

    public void Update() {
        wind = Vector3.Lerp(wind, nextWind, .2f);
    }

    public void FixedUpdate() {
        GetComponent<Rigidbody>().AddForce(wind * windForce, ForceMode.Force);
    }

    private IEnumerator UpdateWind() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(5, 10));
            nextWind = NewDirection();
        }
    }

    private Vector3 NewDirection() {
        Vector3 w = Random.insideUnitSphere;
        w.y = 0f;
        return w.normalized;
    }
}