using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NPCData : CharacterData {

    public enum NPCType {
		NORMIE
	}

    public System.Guid location;
    public NPCType type;
    public string name;
    public SerializableVector3 rotation = new SerializableVector3(new Vector3(0, Random.Range(0, 360), 0));
    public List<NPCTaskSource> taskSources = new List<NPCTaskSource>();

    public NPCData(NPCType type, System.Guid location, bool female = false, string lastName = "") {
        this.type = type;
        this.location = location;
        this.female = female;
        name = NameGen.CharacterName(female, lastName);
    }

    // try to do a task
    public void Simulate(float startTime, float endTime, bool background) {
        if (health <= 0 || departed) {
            return;
        }
        NPCTask task = GetTask(startTime);
        if (task == null) {
            return;
        }
        float minTimeLeft = task.GetTimeLeft();
        SimulateTask(task, startTime, Mathf.Min(startTime + minTimeLeft, endTime), background);
    }

    public NPCTask GetTask(float time = -1) {
        time = time == -1 ? SaveGame.currentGame.time.worldTime : time;
        List<NPCTask> tasks = taskSources.Select(x => x.GetTask(guid, time)).Where(x => x != null).ToList();
        if (tasks.Count == 0) {
            return null;
        }
        int maxPriority = tasks.Max(x => x.priority);
        return tasks.Where(x => x.priority == maxPriority).First();
    }

    private void SimulateTask(NPCTask task, float startTime, float endTime, bool background) {
        Task.TaskDestination destination = task.GetLocation();

        try {
            if (GoToLocation(startTime, endTime, task.GetLocation())) {
                if (background || destination.location != Map.CurrentLocation().guid) {
                    task.Simulate(this);
                }
            }
        } catch (System.Exception e) {
            Debug.LogFormat("from {0} ({1}, {4}) to {2} ({3}, {5})", 
                    location, 
                    Map.Location(location).name, 
                    task.GetLocation().location, 
                    Map.Location(task.GetLocation().location).name,
                    SaveGame.currentGame.map.locations.ContainsKey(location),
                    SaveGame.currentGame.map.locations.ContainsKey(task.GetLocation().location));
            throw e;
        }
    }

    private float timeInCurrentLocation;
    private float maxSimulatedTime;
    private List<System.Guid> path = new List<System.Guid>();
    private readonly float TIME_IN_LOCATION_BEFORE_TRAVEL = 10 * WorldTime.MINUTE;
    public bool departed {
        get { return timeInCurrentLocation >= TIME_IN_LOCATION_BEFORE_TRAVEL; }
    }
    
    // returns true when they're in range
    private bool GoToLocation(float startTime, float endTime, Task.TaskDestination destination) {
        if (location == destination.location) {
            return true;
        }

        startTime = Mathf.Max(startTime, maxSimulatedTime);
        maxSimulatedTime = startTime;
        if (path.Count == 0 || path.Last() != destination.location) {
            path = SaveGame.currentGame.map.BestPathFrom(location, destination.location);
        }
        if (path.Count == 0) {
            return true;
        }

        timeInCurrentLocation += (endTime - startTime);
        float travelTime = 4 * Map.Location(location).DistanceFrom(Map.Location(path.First()));		

        if (departed) {
            NPCArriveEvent arrival = new NPCArriveEvent(guid, path.First(), location);
            SaveGame.currentGame.events.CreateEvent(SaveGame.currentGame.time.worldTime + travelTime, arrival);
            // string fname = Map.Location(location).name;
            // string lname = Map.Location(path.First()).name;
            // Debug.Log(name + " is traveling from " + fname + " to " + lname + " in " + travelTime + " seconds");
        }

        return false;
    }

    public void TravelToLocation(System.Guid l) {
        location = l;
        timeInCurrentLocation = 0;
        if (path.Count > 0 && path.First() == l) {
            path.RemoveAt(0);
        } else {
            path.Clear();
        }
    }
}