using System;

[System.Serializable]
public class DebugTaskSource : NPCTaskSource {
    private NPCTask task;

    public NPCTask GetTask(System.Guid character, float time) {
        return task;
    }

    public void SetTask(NPCTask task) {
        this.task = task;
    }
}