[System.Serializable]
public class EndDialogueOption : DialogueOption {
    
    public EndDialogueOption() : base("DONE") {
    }

    public override void Select(Dialogue parent, DialogueDisplay display) {
        display.FinishConvo();
    }
}