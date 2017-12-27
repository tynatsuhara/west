using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InteriorLocation : Location {

    private List<List<Room>> grid;

    public InteriorLocation(Map parent, System.Guid town, List<List<Room>> grid) : base(parent, false) {
        this.grid = grid;
        parent.locations[guid] = this;  // register the guid with the map
        height = grid.Count;
        width = grid.First().Count;
        name = "SOME BUILDING";
        worldLocation = parent.locations[town].worldLocation;
        teleporters.Add(new Teleporter.TeleporterData(town, Vector3.one, "front door"));
        connections = new System.Guid[]{ town };
        this.town = town;
        discovered = true;
    }

	public override GameObject PrefabAt(int x, int y) {
        return grid[y][width-x-1] != null ? LevelBuilder.instance.floorPrefab : null;
    }

	public override bool TileOccupied(int x, int y) {
        // TODO: allow for multiple chars
        return grid[y][width-x-1] != null && grid[y][width-x-1] != null;
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