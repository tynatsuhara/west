using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World {
    [System.Serializable]
    public class EntityTile : TileElement {

        private LevelBuilder.PrefabKey prefabKey;
        private float xSpawnOffset;
        private float ySpawnOffset;
        private int rotation;
        private Dictionary<string, System.Object> metaDataCache = new Dictionary<string, System.Object>();

        public EntityTile(LevelBuilder.PrefabKey prefabKey, float xSpawnOffset = 0, float ySpawnOffset = 0, int rotation = 0) {
            this.prefabKey = prefabKey;
            this.xSpawnOffset = xSpawnOffset;
            this.ySpawnOffset = ySpawnOffset;
            this.rotation = rotation;
        }
    
        public override void Rotate() {
            rotation = (rotation + 90) % 360;
        }

        public override void Spawn(LevelBuilder lb, Location location, int x, int y) {
            GameObject tile = GameObject.Instantiate(
                lb.GetPrefab(prefabKey), 
                new Vector3(
                    x * LevelBuilder.TILE_SIZE + xSpawnOffset, 
                    0f,                                               // TODO: should we change this? also, variable z-height?
                    y * LevelBuilder.TILE_SIZE + ySpawnOffset
                ), 
                Quaternion.identity
            ) as GameObject;

            // rotate around the middle of the bottom left tile
            Vector3 axis = new Vector3(x + .5f, 0, y + .5f) * LevelBuilder.TILE_SIZE;
            tile.transform.RotateAround(axis, Vector3.up, rotation);

            foreach (var metaData in tile.GetComponentsInChildren<EntityTileMetaData>()) {
                metaData.SetEntityTileCache(metaDataCache);
            }
        }
    }
}