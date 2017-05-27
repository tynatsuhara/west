using UnityEngine;

public class Tumbleweed : MonoBehaviour {
    public static Vector3 wind;

    public void Awake() {
        if (wind == Vector3.zero) {
            wind = Random.insideUnitSphere;
            wind.y = 0f;
            wind = wind.normalized;
        }
    }
}