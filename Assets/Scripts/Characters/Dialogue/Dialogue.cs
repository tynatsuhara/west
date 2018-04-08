using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Dialogue {
    public enum Priority {
		CURRENT_QUEST = 10,
		OFFERING_QUEST = 20,
		HAS_SHOP = 30
	}

    public string color;
    public string icon;

    private Dictionary<string, DialogueFrame> frames = new Dictionary<string, DialogueFrame>();
    private string currentFrameTag;

    public Dialogue(string color, string icon) {
        this.color = color;
        this.icon = icon;
    }

    public Dialogue AddFrame(string tag, bool player, string text, params DialogueOption[] options) {
        if (currentFrameTag == null) {
            currentFrameTag = tag;
        }
        frames[tag] = (new DialogueFrame(tag, player, text, options.ToList()));
        return this;
    }

    public void GoToFrame(string frameTag) {
        currentFrameTag = frameTag;
    }

    public DialogueFrame GetCurrentFrame() {
        return frames[currentFrameTag];
    }

    [System.Serializable]
    public class DialogueFrame {
        public string tag;
        public bool player;
        public string text;
        public List<DialogueOption> options;

        public DialogueFrame(string tag, bool player, string text, List<DialogueOption> options) {
            this.tag = tag;
            this.player = player;
            this.text = text;
            this.options = options;
        }
    }
}