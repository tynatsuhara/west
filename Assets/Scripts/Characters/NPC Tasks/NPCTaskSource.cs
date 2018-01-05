public interface NPCTaskSource {
    // returns the current task for this source
    NPCTask GetTask(System.Guid character, float time);
}