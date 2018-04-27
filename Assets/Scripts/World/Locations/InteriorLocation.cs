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
            Grid<List<World.TileElement>> tiles,            
            Grid<Room> grid,
            string name, 
            List<Teleporter.TeleporterData> teleporters
    ) : base(parent, false) {
        this.tiles = tiles;
        this.grid = grid;
        this.town = town;   
        worldLocation = parent.locations[town].worldLocation;        
        this.name = name;
        this.teleporters.AddRange(teleporters);
        discovered = true;
    }

    public override string ToString() {
        return grid.ToString((room, x, y) => {
            if (room == null) {
                return ' ';
            }
            World.FloorTile sq = room.TileElementsAt(x, y).Where(el => el is World.FloorTile).First() as World.FloorTile;
            return (sq.wallTop || sq.wallBottom || sq.wallLeft || sq.wallRight) ? '#' : '/';
        });
    }
}