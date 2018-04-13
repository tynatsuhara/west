using UnityEngine;

// Represents an interior floor tile

namespace World {
    [System.Serializable]
    public class FloorTile : GroundTile {
        public bool wallRight, wallBottom, wallLeft, wallTop;

        // TODO: get rid of ch???
        public FloorTile(char ch) : base(GroundType.FLOOR) {
            this.ch = ch;
        }

        public override void Rotate() {
            bool oldRight = wallRight;
            wallRight = wallTop;
            wallTop = wallLeft;
            wallLeft = wallBottom;
            wallBottom = oldRight;
        }

        public void RemoveWallTop(char newFloor) {
            ch = newFloor;
            wallTop = false;
        }
        public void RemoveWallBottom(char newFloor) {
            ch = newFloor;
            wallBottom = false;
        }

        public override void Spawn(LevelBuilder lb, Location location, int x, int y) {
            GameObject tile = GameObject.Instantiate(lb.floorPrefab, new Vector3(x * LevelBuilder.TILE_SIZE, -.2f, 
                    y * LevelBuilder.TILE_SIZE), Quaternion.identity) as GameObject;
            tile.GetComponent<Floor>().kickUpDirt = false;  // TODO: change the floor and biome color shit
            lb.floorTiles[x,y] = tile.GetComponent<PicaVoxel.Volume>();
        }
    }
}