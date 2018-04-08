[System.Serializable]
public abstract class DialogueOption {

    public string name = "DEFAULT OPTION";

    public DialogueOption(string name) {
        this.name = name.ToUpper();
    }

    public abstract void Select(Dialogue parent);
}