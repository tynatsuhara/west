using UnityEngine;
using System.Collections;

public class Tumbleweed : MonoBehaviour {
    private Vector3 wind;
    private Vector3 nextWind;
    private float forcePercentage;
    public float windForce;
    public float windTorque;
    public float density;
    public SphereCollider sc;
    public Color32[] colors;

    public void Start() {
        float radius = sc.radius = Random.Range(.3f, .6f);
        int d = (int) (radius * density);
        PicaVoxel.Volume vol = GetComponent<PicaVoxel.Volume>();
        for (int i = 0; i < d; i++) {
            Vector3 pos = transform.position + Random.insideUnitSphere * radius;
            PicaVoxel.Voxel voxel = new PicaVoxel.Voxel();
            voxel.Color = colors[Random.Range(0, colors.Length)];
            voxel.State = PicaVoxel.VoxelState.Active;
            vol.SetVoxelAtWorldPosition(pos, voxel);
        }

        wind = NewDirection();
        StartCoroutine(UpdateWind());
    }

    public void Update() {
        wind = Vector3.Lerp(wind, nextWind, .2f);
    }

    public void FixedUpdate() {
        GetComponent<Rigidbody>().AddForce(wind * windForce * forcePercentage, ForceMode.Force);
        GetComponent<Rigidbody>().AddTorque(wind * windTorque * forcePercentage, ForceMode.Force);
    }

    private IEnumerator UpdateWind() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(5, 10));
            nextWind = NewDirection();
            forcePercentage = Random.Range(.1f, 1f);
        }
    }

    private Vector3 NewDirection() {
        Vector3 w = Random.insideUnitSphere;
        w.y = 0f;
        return w.normalized;
    }
}