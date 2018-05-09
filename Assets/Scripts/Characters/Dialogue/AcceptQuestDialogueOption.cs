[System.Serializable]
public class AcceptQuestDialogueOption : DialogueOption {

    private Quest q;

    public AcceptQuestDialogueOption(string s, Quest q) : base(s) {
        this.q = q;
    }

    public override void Select(Dialogue parent, DialogueDisplay display) {
        SaveGame.currentGame.quests.AddQuest(q);
        display.FinishConvo(true);
    }
}