using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class DialogueMenu : Menu {

    private Dialogue dialogue;
    private DialogueDisplay display;
    private Dictionary<MenuNode, DialogueOption> options;

    public MenuNode[] nodes;

    public void Start() {
        playerId = GetComponentInParent<PlayerUI>().player.id;
    }

    public void LoadDialogue(Dialogue dialogue, DialogueDisplay display, List<DialogueOption> options) {
        this.dialogue = dialogue;
        this.display = display;
        ConfigureNodes(options);
    }

	public override void Enter(MenuNode node) {
        options[node].Select(dialogue, display);
    }

    private void ConfigureNodes(List<DialogueOption> options) {
        this.options = new Dictionary<MenuNode, DialogueOption>();
        for (int i = 0; i < options.Count; i++) {
            nodes[i].gameObject.SetActive(true);
            nodes[i].down = nodes[(i + 1) % options.Count];
            nodes[i].up = nodes[(options.Count + i - 1) % options.Count];
            nodes[i].text.text = options[i].text;

            this.options[nodes[i]] = options[i];
        }
        for (int i = options.Count; i < nodes.Length; i++) {
            nodes[i].gameObject.SetActive(false);
        }
    }
}