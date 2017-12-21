using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InteriorLocation : Location {

    private List<string> grid;

    public InteriorLocation(Map parent, List<string> grid) : base(parent) {
        this.grid = grid;
        height = grid.Count;
        width = grid[0].Length;
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