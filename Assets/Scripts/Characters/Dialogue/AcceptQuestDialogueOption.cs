[System.Serializable]
public class AcceptQuestDialogueOption : DialogueOption {

    private Quest q;

    public AcceptQuestDialogueOption(Quest q) : base("ACCEPT") {
        this.q = q;
    }

    public override void Select(Dialogue parent) {
        SaveGame.currentGame.quests.AddQuest(q);
    }
}