using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InteriorLocation : Location {

    private List<string> grid;
    new public string name = "Interior!";

    public InteriorLocation(Map parent, System.Guid outside, List<string> grid) : base(parent, false) {
        this.grid = grid;
        parent.locations[guid] = this;  // register the guid with the map
        height = grid.Count;
        width = grid[0].Length;
        teleporters.Add(new Teleporter.TeleporterData(outside, Vector3.zero, "front door"));
    }

	public override GameObject PrefabAt(int x, int y) {
        return LevelBuilder.instance.floorPrefab;
    }

	public override bool TileOccupied(int x, int y) {
        return grid[x][y] != ' ';
    }

    public override string ToString() {
        return string.Join("\n", grid.ToArray());
    }
}