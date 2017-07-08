using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class EventQueue {
    private SortedList<float, WorldEvent> events = new SortedList<float, WorldEvent>();

    public IEnumerator Tick() {
        while (true) {
            while (events.Count > 0 && events.Keys[0] <= SaveGame.currentGame.time.worldTime) {
                events[0].Execute();
                events.RemoveAt(0);
            }
            yield return new WaitForSeconds(WorldTime.MINUTE);
        }
    }
    
    public void CreateEvent(float time, WorldEvent e) {
        events.Add(time, e);
    }
}