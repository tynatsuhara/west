using System;

[System.Serializable]
public class NPCKillTask : NPCTask {

    private System.Guid target;

    // constructor to kill players
    public NPCKillTask() {
    }

    public NPCKillTask(System.Guid target) {
        this.target = target;
    }

    public override Task.TaskDestination GetLocation() {
        if (target == System.Guid.Empty) {
            return new Task.TaskDestination(Map.CurrentLocation().guid, SaveGame.currentGame.savedPlayers[0].position.val);
        } else {
            NPC.NPCSaveData c = SaveGame.currentGame.savedCharacters[target];
            return new Task.TaskDestination(c.location, c.position.val);
        }
    }
    
    // Lower bound on time left for a task -- If this is <= 0, the task is done.
    public override float GetTimeLeft() {
        if (target == System.Guid.Empty) {
            return SaveGame.currentGame.savedPlayers[0].health > 0 ? WorldTime.MINUTE : 0;
        } else {
            return SaveGame.currentGame.savedCharacters[target].health > 0 ? WorldTime.MINUTE : 0;
        }
    }
}