using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InteriorLocation : Location {

    private Grid<Room> grid;

    public override string greeting {
		get { return name; }
	}

    public InteriorLocation(
            Map parent, 
            System.Guid town, 
            Grid<Room> grid, 
            string name, 
            List<Teleporter.TeleporterData> teleporters
    ) : base(parent, false) {
        this.grid = grid;
        this.town = town;   
        worldLocation = parent.locations[town].worldLocation;        
        height = grid.height;
        width = grid.width;
        this.name = name;
        this.teleporters.AddRange(teleporters);
        discovered = true;
    }

    public void PlaceAt(System.Guid town) {
        connections.Add(town);     
    }

	public override GameObject PrefabAt(int x, int y) {
        return grid.Get(x, y) != null ? LevelBuilder.instance.floorPrefab : null;
    }

	public override bool TileOccupied(int x, int y) {
        Room r = grid.Get(x, y);
        if (r == null) {
            return true;
        }
        return r.SquareAt(x, y).occupied;
    }

    public Room.Square SquareAt(int x, int y) {
        Room rm = grid.Get(x, y);
        return rm == null ? null : rm.SquareAt(x, y);
    }

    public override string ToString() {
        return grid.ToString((room, x, y) => {
            if (room == null) {
                return ' ';
            }
            Room.Square sq = room.SquareAt(x, y);
            return (sq.wallTop || sq.wallBottom || sq.wallLeft || sq.wallRight) ? '#' : '/';
        });
    }
}