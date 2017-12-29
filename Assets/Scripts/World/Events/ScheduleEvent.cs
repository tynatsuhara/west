using UnityEngine;

[System.Serializable]
public class ScheduleEvent : WorldEvent {
    private System.Guid character;

    public ScheduleEvent(System.Guid character) {
        this.character = character;
    }

    public override void Execute(bool hasLoadedLocation) {
        SaveGame.currentGame.savedCharacters[character].Reschedule();
    }
}