using System.Collections.Generic;

[System.Serializable]
/*
  Continually cycles through the schedule 
  (so, to make it work cleanly, the blocks' durations should sum to a multiple of 24 hours)
 */
public class Schedule : NPCTaskSource {

    private List<Block> blocks = new List<Block>();
    private float scheduleDuration;

    public NPCTask GetTask(System.Guid character, float time) {
        time = time % scheduleDuration;
        float passed = 0;
        foreach (Block b in blocks) {
            passed += b.duration;
            if (passed > time) {
                return b.task;
            }
        }
        return null;
    }

    public Schedule Clear() {
        blocks.Clear();
        scheduleDuration = 0;
        return this;
    }

    // starts from 00:00 (midnight)
    // if task is null, don't do anything for the duration of that block 
    // (TODO: task should never actually be null)
    public Schedule AddBlock(float duration, NPCTask task=null) {
        Block b = new Block();
        b.duration = duration;
        b.task = task;
        blocks.Add(b);
        scheduleDuration += duration;

        return this;
    }

    [System.Serializable]
    private class Block {
        public float duration;
        public NPCTask task;
    }
}