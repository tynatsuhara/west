[System.Serializable]
public class EndDialogueOption : DialogueOption {
    
    private string reply;
    private bool removeDialogue;
    private string resumeFrameTag;

    // resumeFrameTag -- the frame the dialogue will resume on
    // reply -- the out-of-dialogue response
    public EndDialogueOption(string s, string resumeFrameTag, string reply = "") : base(s) {
        this.reply = reply;
        this.resumeFrameTag = resumeFrameTag;
        this.removeDialogue = false;
    }
    public EndDialogueOption(string s, string reply = "") : base(s) {
        this.reply = reply;
        this.removeDialogue = true;
    }

    public override void Select(Dialogue parent, DialogueDisplay display) {
        if (reply.Length > 0)
            display.NPCReply(reply);
        display.FinishConvo(removeDialogue, resumeFrameTag);
    }
}