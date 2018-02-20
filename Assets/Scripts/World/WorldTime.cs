using UnityEngine;

[System.Serializable]
public class WorldTime {
    public const float MINUTE = 1.5f;  // how many real life seconds represent an in-game minute?
    public const float HOUR = 60 * MINUTE;
    public const float DAY = 24 * HOUR;
    public const float YEAR = 365 * DAY;

    private int startYear = Random.Range(1875, 1885);
    public WorldTime() {
        worldTime_ = Random.Range(0, 365) * DAY;
    }

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
                // Debug.Log(DateString());
            }
        }
    }

    public static float Future(int minutes = 0, int hours = 0, int days = 0, int years = 0) {
        return SaveGame.currentGame.time.worldTime + MINUTE * minutes + HOUR * hours + DAY * days + YEAR * years;
    }

    public string DateString() {
        return DateStringForTime(worldTime);
    }

    public string DateStringForTime(float time) {
        int year = (int)(time/YEAR) + startYear;
        int day = (int)(time % YEAR/DAY);
        int hour = (int)(time % DAY/HOUR);
        int minute = (int)(time % HOUR/MINUTE);

        var months = new string[]{"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
        var dayAmounts = new int[]{31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
        int daysPrior = 0;
        int month = 0;
        for (;month < 12; month++) {
            if (day < daysPrior + dayAmounts[month]) {
                day -= daysPrior;
                break;
            }
            daysPrior += dayAmounts[month];
        }
        return string.Format("{0} {1}, {2}, {3}:{4} {5}", months[month], day, year, 
                (hour == 0 ? 12 : (hour > 12 ? hour - 12 : hour)), (minute < 10 ? "0" : "") + minute, (hour >= 12 ? "PM" : "AM"));
    }
}