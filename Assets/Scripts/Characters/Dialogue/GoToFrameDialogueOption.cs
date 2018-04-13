[System.Serializable]
public class GoToFrameDialogueOption : DialogueOption {

    private string frameTag;

    public GoToFrameDialogueOption(string frameTag) : base("NEXT") {
        this.frameTag = frameTag;
    }

    public override void Select(Dialogue parent, DialogueDisplay display) {
        parent.GoToFrame(frameTag);
        display.Refresh();
    }
}