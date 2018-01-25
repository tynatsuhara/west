using System;
using UnityEngine;

[System.Serializable]
public class NPCNoOpTask : NPCTask {

    private System.Guid location;
    private SerializableVector3 pos;

    public NPCNoOpTask(System.Guid location, Vector3 position) {
        this.location = location;
        pos = new SerializableVector3(position);
    }

    public override Task.TaskDestination GetLocation() {
        return new Task.TaskDestination(location, pos.val, "");
    }
    
    // Lower bound on time left for a task -- If this is <= 0, the task is done.
    public override float GetTimeLeft() {
        return 1f;
    }
}