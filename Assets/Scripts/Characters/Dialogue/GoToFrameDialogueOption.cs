[System.Serializable]
public class GoToFrameDialogueOption : DialogueOption {

    private string frameTag;

    public GoToFrameDialogueOption(string s, string frameTag) : base(s) {
        this.frameTag = frameTag;
    }

    public override void Select(Dialogue parent, DialogueDisplay display) {
        parent.GoToFrame(frameTag);
        display.Refresh();
    }
}