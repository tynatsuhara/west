using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InteriorLocation : Location {

    private List<List<Room>> grid;

    public InteriorLocation(Map parent, System.Guid town, List<List<Room>> grid, string name) : base(parent, false) {
        this.grid = grid;
        this.town = town;   
        worldLocation = parent.locations[town].worldLocation;        
        height = grid.Count;
        width = grid.First().Count;
        this.name = name;
        teleporters.Add(new Teleporter.TeleporterData(town, Vector3.one, "front door"));
        discovered = true;
    }

    public void PlaceAt(System.Guid town) {
        connections.Add(town);     
    }

	public override GameObject PrefabAt(int x, int y) {
        return grid[y][width-x-1] != null ? LevelBuilder.instance.floorPrefab : null;
    }

	public override bool TileOccupied(int x, int y) {
        // TODO: allow for multiple chars
        return grid[y][width-x-1] != null;
    }

    public Room.Square SquareAt(int row, int col) {
        if (row < 0 || row >= height || col < 0 || col >= width || grid[row][col] == null) {
            return null;
        }
        return grid[row][col].SquareAt(row, col);
    }

    public override string ToString() {
        string result = "";
        for (int r = 0; r < height; r++) {
            for (int c = 0; c < width; c++) {
                result += grid[r][c] != null ? grid[r][c].CharAt(r, c) : ' ';
            }
            result += '\n';
        }
        return result.Substring(0, result.Length - 1);
    }
}