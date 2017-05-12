using UnityEngine;

[System.Serializable]
public class WorldTime {
    public const float MINUTE = 1.5f;  // how many real life seconds represent an in-game minute?
    public const float HOUR = 60 * MINUTE;
    public const float DAY = 24 * HOUR;
    public const float YEAR = 365 * DAY;

    private float worldTime_;    
    public float worldTime {
        get {
            return worldTime_;
        }
        set {
            int beforeTime = (int)(worldTime_/MINUTE);
            worldTime_ = value;
            int afterTime = (int)(worldTime_/MINUTE);
            if (afterTime > beforeTime) {
                Debug.Log(afterTime);
            }
        }
    }
}