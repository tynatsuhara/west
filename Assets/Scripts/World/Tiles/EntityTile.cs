using System.Collections;
using UnityEngine;

// TODO: Verify saving cactus works, maybe move the saving logic to here

namespace World {
    [System.Serializable]
    public class EntityTile : TileElement {

        private float xSpawnOffset;
        private float ySpawnOffset;

        public EntityTile(float xSpawnOffset, float ySpawnOffset) {
            this.xSpawnOffset = xSpawnOffset;
            this.ySpawnOffset = ySpawnOffset;
        }
    
        public override void Rotate() {
            float temp = xSpawnOffset;
            xSpawnOffset = ySpawnOffset;
            ySpawnOffset = temp;

            // rotate prefab?
        }

        public override void Spawn(LevelBuilder lb, Location location, int x, int y) {
            GameObject tile = GameObject.Instantiate(
                lb.floorPrefab, 
                new Vector3(
                    x * LevelBuilder.TILE_SIZE + xSpawnOffset, 
                    -.2f,                                               // TODO: variable z-height?
                    y * LevelBuilder.TILE_SIZE + ySpawnOffset
                ), 
                Quaternion.identity
            ) as GameObject;
        }
    }
}