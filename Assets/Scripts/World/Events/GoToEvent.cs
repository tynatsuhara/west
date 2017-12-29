using UnityEngine;

[System.Serializable]
public class GoToEvent : WorldEvent {
    private System.Guid character;
    public System.Guid location;
    public SerializableVector3 spot;

    public GoToEvent(System.Guid character, System.Guid location, Vector3 spot) {
        this.character = character;
        this.location = location;
        this.spot = new SerializableVector3(spot);
    }

    public override void Execute(bool hasLoadedLocation) {
        if (!hasLoadedLocation) {
            return;
        }
        if (Map.LocationOfCharacter(character) == Map.CurrentLocation()) {
            GameManager.instance.GetNPC(character).GoTo(this);
        }
    }
}