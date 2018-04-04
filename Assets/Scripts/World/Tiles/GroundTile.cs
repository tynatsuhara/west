using UnityEngine;

// Represents an exterior ground tile
// If occupied, that means there's a building on it

namespace World {
    [System.Serializable]
    public class GroundTile : TileElement {

        public enum GroundType {
            GRASS,
            TRAIL
        }

        public GroundType type;

        public override void Spawn(LevelBuilder lb, Location location, int x, int y) {
            GameObject prefab = type == GroundType.GRASS ? lb.floorPrefab : lb.trailPrefab;

            GameObject tile = GameObject.Instantiate(prefab, new Vector3(x * LevelBuilder.TILE_SIZE, -.2f, 
                    y * LevelBuilder.TILE_SIZE), Quaternion.identity) as GameObject;
            lb.floorTiles[x,y] = tile.GetComponent<PicaVoxel.Volume>();
        }        
    }    
}