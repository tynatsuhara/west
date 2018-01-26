
[System.Serializable]
public class KillQuest : Quest {
    
    private System.Guid character;
    private KillTask killTask;

    public KillQuest(System.Guid character) {
        this.character = character;
        this.killTask = new KillTask(character);
        this.title = "Kill " + SaveGame.currentGame.savedCharacters[character].name;
    }

	public override Task UpdateQuest() {
        if (SaveGame.currentGame.savedCharacters[character].isAlive) {
            return killTask;
        }
        return null;
    }
}