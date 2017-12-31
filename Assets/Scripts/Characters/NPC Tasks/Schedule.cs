using System.Collections.Generic;

public class Schedule {
    private List<Task> tasks = new List<Task>();
    private List<int> timeForTask = new List<Task>();

    // durations should add to 24 hours
    public void AddTask(Task task, int duration) {
        tasks.Add(task);
        timeForTask.Add(duration);
    }

    public Task CurrentTask() {
        float time = SaveGame.currentGame.time.worldTime % WorldTime.DAY;
        return null;
    }
}