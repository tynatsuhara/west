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

        string name = SaveGame.currentGame.savedCharacters[character].name;
        string fname = Map.Location(from).name;
        string lname = Map.Location(location).name;
        Debug.Log(name + " is traveling from " + fname + " to " + lname);
    }

    public void Execute(bool hasLoadedLocation) {
        NPC.NPCSaveData c = SaveGame.currentGame.savedCharacters[character];
        c.location = location;
        if (hasLoadedLocation && Map.CurrentLocation().guid == location) {
            c.position = Map.Location(location).teleporters.Where(x => x.toId == from).First().position;
            LevelBuilder.instance.SpawnNPC(c.guid);
        } else {
            c.position = new SerializableVector3(Map.Location(location).RandomUnoccupiedTile());
        }
    }
}