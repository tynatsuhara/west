using UnityEngine;
using System.Linq;

[System.Serializable]
public class KillTask : Task {
    private System.Guid character;

    public override bool complete {
		get { 
            if (SaveGame.currentGame.savedCharacters[character].location == Map.CurrentLocation().guid) {
                NPC npc = GameManager.spawnedNPCs.Where(x => x.guid == character).FirstOrDefault();
                if (npc != null) {
                    return !npc.isAlive;
                }
            }
            return !SaveGame.currentGame.savedCharacters[character].isAlive;
        }
	}

    public override string message {
        get { return "Kill " + SaveGame.currentGame.savedCharacters[character].name; }
    }

    public override TaskDestination[] GetLocations() {
		return new TaskDestination[]{
            new TaskDestination(character, Quest.QUEST_KILL_ICON)
        };
	}

    public KillTask(System.Guid character) {
        this.character = character;
    }
}