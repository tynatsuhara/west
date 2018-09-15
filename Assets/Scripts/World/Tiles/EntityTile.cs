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
            float temp = xSpawnOffset;
            xSpawnOffset = ySpawnOffset;
            ySpawnOffset = temp;

            // TODO: rotate prefab?
            rotation += 90;
        }

        public override void Spawn(LevelBuilder lb, Location location, int x, int y) {
            GameObject tile = GameObject.Instantiate(
                lb.GetPrefab(prefabKey), 
                new Vector3(
                    x * LevelBuilder.TILE_SIZE + xSpawnOffset, 
                    -.2f,                                               // TODO: variable z-height?
                    y * LevelBuilder.TILE_SIZE + ySpawnOffset
                ), 
                Quaternion.Euler(Vector3.up * rotation)
            ) as GameObject;

            foreach (var metaData in tile.GetComponentsInChildren<EntityTileMetaData>()) {
                metaData.SetEntityTileCache(metaDataCache);
            }
        }
    }
}