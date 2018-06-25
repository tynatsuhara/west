using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class NPCInvestigateTask : NPCTask {

    private System.Guid location;
    private SerializableVector3 position;
    private float endTime;
    private SerializableVector3 lookDirection;
    private float nextLookDirectionTime;    

    public NPCInvestigateTask(Vector3 position, float timeToSpend = -1) {
        if (timeToSpend == -1) {
            timeToSpend = UnityEngine.Random.Range(8, 15) * WorldTime.MINUTE;
        }
        this.location = Map.CurrentLocation().guid;
        this.position = new SerializableVector3(position);
        endTime = SaveGame.currentGame.time.worldTime + timeToSpend;
    }

    public override Task.TaskDestination GetLocation() {
        return new Task.TaskDestination(location, position.val, "");
    }
    
    // Lower bound on time left for a task -- If this is <= 0, the task is done.
    public override float GetTimeLeft() {
        return endTime - SaveGame.currentGame.time.worldTime;
    }

    public override void Execute(NPC self) {
        self.DrawWeapon();
		self.GoToPosition(position.val, 2);
        float dist = (self.transform.position - position.val).magnitude;
        if (dist < 3) {  // look around
            if (lookDirection == null || SaveGame.currentGame.time.worldTime >= nextLookDirectionTime) {
                nextLookDirectionTime = SaveGame.currentGame.time.worldTime + UnityEngine.Random.Range(.5f, 2f) * WorldTime.MINUTE;
                lookDirection = new SerializableVector3(self.transform.position + UnityEngine.Random.insideUnitSphere);
            }
            self.LookAt(lookDirection.val);
        } else if (dist < 10) {
            self.LookAt(position.val);
        }
    }

    public override void Simulate(NPCData sim) {
        // TODO: how do we sim this?
    }
}