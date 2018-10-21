using UnityEngine;
using System;
using System.Collections.Generic;

// Intermediary between an EntityTile and the items it spawns
[System.Serializable]
public class EntityTileMetaData : MonoBehaviour {

    private Dictionary<string, System.Object> cache;

    public void SetEntityTileCache(Dictionary<string, System.Object> cache) {
        this.cache = cache;
    }

    // Returns a direct reference to the MetaData object
    // Use the defaultSupplier to create and save an object if it doesn't exist
    // Should be called in Start
    public T GetOrCreate<T>(string key, Func<T> defaultSupplier) {
        if (!cache.ContainsKey(key)) {
            cache[key] = defaultSupplier();
        }
        return (T) cache[key];
    }

    public void Set<T>(string key, T val) {
        cache[key] = val;
    }
}