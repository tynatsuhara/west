using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class NPCKillTask : NPCTask {

    private System.Guid target;
    private bool isTargetPlayer;

    public NPCKillTask(System.Guid target) {
        this.target = target;
        isTargetPlayer = GameManager.players.Select(x => x.guid).Contains(target);
    }

    public override Task.TaskDestination GetLocation() {
        if (isTargetPlayer) {
            return new Task.TaskDestination(Map.CurrentLocation().guid, SaveGame.currentGame.savedPlayers[0].position.val, "");
        } else {
            NPCData c = SaveGame.currentGame.savedCharacters[target];
            return new Task.TaskDestination(c.location, c.position.val, "");
        }
    }
    
    // Lower bound on time left for a task -- If this is <= 0, the task is done.
    public override float GetTimeLeft() {
        if (isTargetPlayer) {
            return SaveGame.currentGame.savedPlayers[0].health > 0 ? WorldTime.MINUTE : 0;
        } else {
            return SaveGame.currentGame.savedCharacters[target].health > 0 ? WorldTime.MINUTE : 0;
        }
    }

    public override void Execute(NPC self) {
        self.DrawWeapon();
        float range = self.CurrentWeapon().range * .75f;
        Character targetChar = GameManager.instance.GetCharacter(target);
        if (self.CanSee(targetChar.gameObject, viewDist: range)) {
            self.LookAt(targetChar.transform);
            if (self.CanSee(targetChar.gameObject, fov: 10f, viewDist: range)) {
                self.Shoot();
            }
        }
		self.GoToPosition(targetChar.transform.position, range);
    }

    public override void Simulate(NPCData sim) {
        /* TODO: how do we determine odds?
            Cumulative threat level of this NPC + friends vs cumulative threat level of enemies
        */
    }
}