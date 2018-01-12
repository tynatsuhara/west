using System.Linq;
using UnityEngine;

[System.Serializable]
public class NPCArriveEvent : WorldEvent {

    private System.Guid character;
    private System.Guid location;
    private System.Guid from;

    public NPCArriveEvent(System.Guid character, System.Guid location, System.Guid from) {
        this.character = character;
        this.location = location;
        this.from = from;
    }

    public void Execute(bool hasLoadedLocation) {
        NPCData c = SaveGame.currentGame.savedCharacters[character];
        c.TravelToLocation(location);
        if (hasLoadedLocation && Map.CurrentLocation().guid == location) {
            // Debug.Log("(1) executing arrive event for " + c.name);
            c.position = Map.CurrentLocation().teleporters.Where(x => x.toId == from).First().position;
            LevelBuilder.instance.SpawnNPC(c.guid);
        } else {
            // Debug.Log("(2) executing arrive event for " + c.name);
            c.position = new SerializableVector3(Map.Location(location).RandomUnoccupiedTile());
        }
    }
}