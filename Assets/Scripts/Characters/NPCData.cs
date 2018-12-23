using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NPCData : CharacterData {

    public bool showSimDebug;
    public enum NPCType {
		NORMIE,
        GOON,
        GANG_LEADER,
        SHERIFF,
        US_MARSHAL
	}

    public System.Guid location;
    public NPCType type;
    public SerializableVector3 rotation = new SerializableVector3(new Vector3(0, Random.Range(0, 360), 0));
    public SortedList<int, Dialogue> dialogues = new SortedList<int, Dialogue>(new DuplicateKeyComparer<int>());

    // task sources
    public const string SCHEDULE = "schedule";
    public const string DYNAMIC_AI = "dynamic_ai";
    public const string DEBUG_TASKS = "debug_tasks";
    public Dictionary<string, NPCTaskSource> taskSources = new Dictionary<string, NPCTaskSource>();

    public NPCData(NPCType type, bool female = false) {
        this.type = type;
        this.female = female;
        name = NameGen.CharacterFirstName(female) + " " + NameGen.CharacterLastName();
        this.taskSources.Add(DEBUG_TASKS, new DebugTaskSource());
    }

	// background - true if the player is in a loaded location (aka exclude simulating that location)
    public void Simulate(float startTime, float endTime, bool background) {
        if (health <= 0 || departed) {
            return;
        }
        NPCTask task = GetTask(startTime);
        if (task == null) {
            return;
        }
        if (showSimDebug) {
            Debug.Log("simulating " + name);
        }
        float minTimeLeft = task.GetTimeLeft();
        SimulateTask(task, startTime, Mathf.Min(startTime + minTimeLeft, endTime), background);
    }

    public NPCTask GetTask(float time = -1) {
        time = time == -1 ? SaveGame.currentGame.time.worldTime : time;
        List<NPCTask> tasks = taskSources.Values.Select(x => x.GetTask(guid, time))
                                                .Where(x => x != null && x.GetTimeLeft() > 0)
                                                .ToList();
        if (tasks.Count == 0) {
            return null;
        }
        int maxPriority = tasks.Max(x => x.priority);
        NPCTask task = tasks.Where(x => x.priority == maxPriority).First();
        if (showSimDebug) {
            Debug.Log(task.GetLocation().position);
        }
        return task;
    }

    private void SimulateTask(NPCTask task, float startTime, float endTime, bool background) {
        Task.TaskDestination destination = task.GetLocation();

        try {
            if (GoToLocation(startTime, endTime, destination)) {
                if (!background || destination.location != Map.CurrentLocation().guid) {
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

    public float timeSimulatedInLocation;
    private float maxSimulatedTime;
    public List<System.Guid> path = new List<System.Guid>();
    private float timeBeforeTravel = -1;
    public bool departed;
    
    public List<System.Guid> SetPathFor(System.Guid destination) {
        if (path.Count == 0 || path.Last() != destination) {
            path = SaveGame.currentGame.map.BestPathFrom(location, destination);
            timeBeforeTravel = -1;
        }
        if (path.Count > 0) {

        }
        return path;
    }

    // returns true when they're in range
    private bool GoToLocation(float startTime, float endTime, Task.TaskDestination destination) {
        if (location == destination.location) {
            return true;
        }

        startTime = Mathf.Max(startTime, maxSimulatedTime);
        maxSimulatedTime = startTime;
        SetPathFor(destination.location);

        if (path.Count == 0) {
            return true;
        }

        timeSimulatedInLocation += (endTime - startTime);

        if (timeBeforeTravel < 0) {
            Vector3 teleporterPos = Map.Location(location).teleporters
                    .Where(x => x.toId == path.First())
                    .OrderBy(x => (x.position.val-position.val).magnitude)
                    .First().position.val;
            timeBeforeTravel = (position.val - teleporterPos).magnitude / 10f;
        }

        if (showSimDebug) {
            Debug.Log(name + " timeSimulatedInLocation = " + timeSimulatedInLocation);
        }
        if (timeSimulatedInLocation >= timeBeforeTravel) {
            departed = true;
            InitiateTravel(path.First());
        }

        return false;
    }

    public void InitiateTravel(System.Guid destination) {
        float travelTime = 4.1f * Map.Location(location).DistanceFrom(Map.Location(destination));
        NPCArriveEvent arrival = new NPCArriveEvent(guid, destination, location);
        float arrivalTime = SaveGame.currentGame.time.worldTime + travelTime;
        SaveGame.currentGame.events.CreateEvent(arrivalTime, arrival);
        if (showSimDebug) {
            string lname = Map.Location(path.First()).name;
            string arrivalTimeString = SaveGame.currentGame.time.DateStringForTime(arrivalTime);
            Debug.Log(name + " will arrive in " + lname + " at " + arrivalTimeString);
        }
    }

    // you should probably call InitiateTravel
    public void TravelToLocation(System.Guid l) {
        location = l;
        timeSimulatedInLocation = 0;
        departed = false;
        if (path.Count > 0 && path.First() == l) {
            path.RemoveAt(0);
        } else {
            path.Clear();
        }
    }
}