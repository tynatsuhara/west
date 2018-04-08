
[System.Serializable]
public class KillQuest : Quest {
    
    private System.Guid character;
    private KillTask killTask;

    public KillQuest(System.Guid character) {
        this.character = character;
        this.killTask = new KillTask(character);
        this.title = "Kill " + SaveGame.currentGame.savedCharacters[character].name;

        Dialogue d = new Dialogue("yellow", "!")
                .AddFrame("intro", false, "Can you help me?",
                    new AcceptQuestDialogueOption(this)
                );

        SaveGame.currentGame.savedCharacters[character].dialogues.Add((int) Dialogue.Priority.OFFERING_QUEST, d);
    }

	public override Task UpdateQuest() {
        if (SaveGame.currentGame.savedCharacters[character].isAlive) {
            return killTask;
        }
        return null;
    }
}