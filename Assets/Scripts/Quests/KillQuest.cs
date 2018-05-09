[System.Serializable]
public class KillQuest : Quest {
    
    private System.Guid character;
    private KillTask killTask;

    public KillQuest(System.Guid character) {
        this.character = character;
        this.killTask = new KillTask(character);
        this.title = "Kill " + SaveGame.currentGame.savedCharacters[character].name;
        
        Dialogue d = new Dialogue("yellow", "!")
                .AddFrame("intro", "I hate my life. Can you help me?",
                    new AcceptQuestDialogueOption("Sure!", this),
                    new GoToFrameDialogueOption("Tell me more", "2")
                ).AddFrame("2", "Can you just shoot me right in the face?",
                    new AcceptQuestDialogueOption("Okie-dokey", this),
                    new EndDialogueOption("Maybe later", "intro", reply: "Thanks, dick!")
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