using UnityEngine;

namespace World {
    [System.Serializable]
    public abstract class TileElement {
        public bool occupied;

        // rotate clockwise
        public virtual void Rotate() {
        }

        public virtual void Spawn(LevelBuilder lb, Location location, int x, int y) {
        }
    }
}