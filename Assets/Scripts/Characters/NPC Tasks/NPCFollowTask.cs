using System;

[System.Serializable]
public class NPCFollowTask : NPCTask {

    private System.Guid target;
    public float distance;

    // constructor to follow players
    public NPCFollowTask(float distance) {
        this.distance = distance;
    }

    public NPCFollowTask(float distance, System.Guid target) {
        this.target = target;
        this.distance = distance;
    }

    public override Task.TaskDestination GetLocation() {
        if (target == System.Guid.Empty) {
            return new Task.TaskDestination(Map.CurrentLocation().guid, SaveGame.currentGame.savedPlayers[0].position.val);
        } else {
            NPCData c = SaveGame.currentGame.savedCharacters[target];
            return new Task.TaskDestination(c.location, c.position.val);
        }
    }
    
    // Lower bound on time left for a task -- If this is <= 0, the task is done.
    public override float GetTimeLeft() {
        return WorldTime.MINUTE;
    }
}