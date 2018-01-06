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
        Character.CharacterSaveData c = GetCharacter();
        return new Task.TaskDestination(Map.LocationOfCharacter(c.guid).guid, c.position.val);
    }
    
    // Lower bound on time left for a task -- If this is <= 0, the task is done.
    public override float GetTimeLeft() {
        return GetCharacter().health > 0 ? WorldTime.MINUTE : 0;
    }

    private Character.CharacterSaveData GetCharacter() {
        if (target == System.Guid.Empty) {
            return SaveGame.currentGame.savedPlayers[0];
        }
        return  SaveGame.currentGame.savedCharacters[target];
    }
}