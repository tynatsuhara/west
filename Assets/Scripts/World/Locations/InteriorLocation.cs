using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InteriorLocation : Location {

    private List<string> grid;

    public InteriorLocation(Map parent, System.Guid town, List<string> grid) : base(parent, false) {
        this.grid = grid;
        parent.locations[guid] = this;  // register the guid with the map
        height = grid.Count;
        width = grid[0].Length;
        name = "Interior!";
        worldLocation = parent.locations[town].worldLocation;
        teleporters.Add(new Teleporter.TeleporterData(town, Vector3.one, "front door"));
        connections = new System.Guid[]{ town };
        this.town = town;
    }

	public override GameObject PrefabAt(int x, int y) {
        return grid[y][x] != ' ' ? LevelBuilder.instance.floorPrefab : null;
    }

	public override bool TileOccupied(int x, int y) {
        // TODO: allow for multiple chars
        return grid[y][x] != ' ' && grid[y][x] != '/';
    }

    public override string ToString() {
        return string.Join("\n", grid.ToArray());
    }
}