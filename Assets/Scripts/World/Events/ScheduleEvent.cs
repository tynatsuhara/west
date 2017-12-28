using UnityEngine;

[System.Serializable]
public class ScheduleEvent : WorldEvent {
    private System.Guid character;

    public ScheduleEvent(System.Guid character) {
        this.character = character;
    }

    public override void Execute(bool hasLoadedLocation) {
        float timeUntilNextReschedule = SaveGame.currentGame.savedCharacters[character].Reschedule();
        SaveGame.currentGame.events.CreateEvent(timeUntilNextReschedule, new ScheduleEvent(character));
    }
}