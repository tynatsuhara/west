using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class EventQueue {
    private SortedList<float, WorldEvent> events = new SortedList<float, WorldEvent>(new DuplicateKeyComparer<float>());

    public IEnumerator Tick() {
        while (true) {
            CheckQueue(true);
            yield return new WaitForSeconds(.2f);
        }
    }
    public void CheckQueue(bool hasLoadedLocation) {
        while (events.Count > 0 && events.Keys[0] <= SaveGame.currentGame.time.worldTime) {
            events.Values[0].Execute(hasLoadedLocation);
            events.RemoveAt(0);
        }
    }
    
    public void CreateEvent(float time, WorldEvent e) {
        events.Add(time, e);
    }


    [System.Serializable]
    private class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : System.IComparable {
        public int Compare(TKey x, TKey y) {
            int result = x.CompareTo(y);
            return result == 0 ? 1 : result;
        }
    }
}