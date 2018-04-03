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
        c.position = Map.Location(location).teleporters.Where(x => x.toId == from).First().position;            
        if (hasLoadedLocation && Map.CurrentLocation().guid == location) {
            LevelBuilder.instance.SpawnNPC(c.guid);
            Debug.Log("spawning NPC");
        }
    }
}