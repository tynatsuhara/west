using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InteriorLocation : Location {

    private Grid<Room> grid;

    public InteriorLocation(Map parent, System.Guid town, Grid<Room> grid, string name) : base(parent, false) {
        this.grid = grid;
        this.town = town;   
        worldLocation = parent.locations[town].worldLocation;        
        height = grid.height;
        width = grid.width;
        this.name = name;
        teleporters.Add(new Teleporter.TeleporterData(town, Vector3.one, "front door"));
        discovered = true;
    }

    public void PlaceAt(System.Guid town) {
        connections.Add(town);     
    }

	public override GameObject PrefabAt(int x, int y) {
        return grid.Get(x, y) != null ? LevelBuilder.instance.floorPrefab : null;
    }

	public override bool TileOccupied(int x, int y) {
        return grid.Get(x, y) != null;
    }

    public Room.Square SquareAt(int x, int y) {
        Room rm = grid.Get(x, y);
        return rm == null ? null : rm.SquareAt(x, y);
    }

    public override string ToString() {
        return grid.ToString((room, x, y) => room == null ? ' ' : room.CharAt(x, y));
    }
}