using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class DialogueMenu : Menu {

    public static readonly int PER_OPTION_SHIFT = 100;

    private Dialogue dialogue;
    private DialogueDisplay display;
    private Dictionary<MenuNode, DialogueOption> options;

    public MenuNode nodesPrefab;
    private MenuNode[] nodes;

    public void Start() {
        playerId = GetComponentInParent<PlayerUI>().player.id;
        nodes = new MenuNode[10];
        nodes[0] = nodesPrefab;
        for (int i = 1; i < nodes.Length; i++) {
            nodes[i] = Instantiate(nodesPrefab, nodesPrefab.transform.parent);
        }
    }

    public void LoadDialogue(Dialogue dialogue, DialogueDisplay display, List<DialogueOption> options) {
        this.dialogue = dialogue;
        this.display = display;
        ConfigureNodes(options);
    }

	public override void Enter(MenuNode node) {
        if (options.Count == 0)
            return;
        options[node].Select(dialogue, display);
    }

    private void ConfigureNodes(List<DialogueOption> options) {
        this.options = new Dictionary<MenuNode, DialogueOption>();
        for (int i = 0; i < options.Count; i++) {
            nodes[i].gameObject.SetActive(true);
            RectTransform rt = nodes[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, (options.Count-i) * PER_OPTION_SHIFT + 10);
            nodes[i].down = nodes[(i + 1) % options.Count];
            nodes[i].up = nodes[(options.Count + i - 1) % options.Count];
            nodes[i].text.text = options[i].text;

            this.options[nodes[i]] = options[i];
        }
        for (int i = options.Count; i < nodes.Length; i++) {
            nodes[i].gameObject.SetActive(false);
        }
        NewSelect(nodes[0]);
    }
}