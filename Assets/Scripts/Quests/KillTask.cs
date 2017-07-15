using UnityEngine;
using System.Linq;

[System.Serializable]
public class KillTask : Task {
    private System.Guid character;

    public override bool complete {
		get { 
            return Map.LocationOfCharacter(character) == Map.CurrentLocation() 
                ? !GameManager.spawnedNPCs.Where(x => x.guid == character).First().isAlive
                : !SaveGame.currentGame.savedCharacters[character].isAlive;
        }
	}
    public override string message {
        get { return "Kill " + SaveGame.currentGame.savedCharacters[character].name; }
    }

    public KillTask(System.Guid character) {
        this.character = character;
    }
}