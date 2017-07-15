using UnityEngine;

[System.Serializable]
public class KillTask : Task {
    private System.Guid character;

    public override bool complete {
		get { return !SaveGame.currentGame.savedCharacters[character].isAlive; }
	}
    public override string message {
        get { return "Kill " + SaveGame.currentGame.savedCharacters[character].name; }
    }

    public KillTask(System.Guid character) {
        this.character = character;
    }

	public override TaskDestination[] GetLocations() {
        Location l = Map.LocationOfCharacter(character);
		return new TaskDestination[] {
            new TaskDestination(Map.LocationOfCharacter(character).guid, 
                                SaveGame.currentGame.savedCharacters[character].position.val) 
        };
	}
}