[System.Serializable]
public class WorldTime {
    public const float MINUTE = 1f;  // how many real life seconds represent an in-game minute?
    public const float HOUR = 60 * MINUTE;
    public const float DAY = 24 * HOUR;
    public const float YEAR = 365 * DAY;

    public float worldTime;
}