[System.Serializable]
public abstract class DialogueOption {

    public string text = "DEFAULT OPTION";

    public DialogueOption(string text) {
        this.text = text.ToUpper();
    }

    public abstract void Select(Dialogue parent, DialogueDisplay display);
}