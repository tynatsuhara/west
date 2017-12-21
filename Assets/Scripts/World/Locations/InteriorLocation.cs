using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class InteriorLocation : Location {

    public InteriorLocation(Map parent) : base(parent) {
    }
    
    public override void Generate() {

    }

	public override GameObject TileAt(int x, int y) {
        return LevelBuilder.instance.floorPrefab;
    }

	public override bool TileOccupied(int val) {

    }
}