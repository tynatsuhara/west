using UnityEngine;

namespace World {
    [System.Serializable]
    public class FloorTile : TileElement {
        public bool wallRight, wallBottom, wallLeft, wallTop;

        // TODO: get rid of ch???
        public FloorTile(char ch) {
            this.ch = ch;
        }

        public override void Rotate() {
            bool oldRight = wallRight;
            wallRight = wallTop;
            wallTop = wallLeft;
            wallLeft = wallBottom;
            wallBottom = oldRight;
        }

        public void RemoveWalls(char newFloor) {
            ch = newFloor;
            wallRight = wallLeft = wallBottom = wallTop = false;
        }

        public override void Spawn(LevelBuilder lb, Location location, int x, int y) {
            GameObject tile = GameObject.Instantiate(lb.floorPrefab, new Vector3(x * LevelBuilder.TILE_SIZE, -.2f, 
                    y * LevelBuilder.TILE_SIZE), Quaternion.identity) as GameObject;
            lb.floorTiles[x,y] = tile.GetComponent<PicaVoxel.Volume>();
        }        
    }    
}