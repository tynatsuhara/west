using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CharacterData {
    public System.Guid guid = System.Guid.NewGuid();
    public List<List<byte[]>> voxelBlobs;
    public SerializableVector3 position;
    public Inventory inv = new Inventory();
    public float health = float.NaN;
    public byte weaponId = 1;   // default main: ak47
    public byte sidearmId = 1;  // default sidearm: revolver
    public byte equippedWeapon = 0;  // start wielding primary
    public System.Object[] gunSaves;		
    public bool isWeaponDrawn;
    public string outfit = "default";
    public byte skinColor;
    public byte hairColor;
    public byte hairStyle;
    public byte accessory;
    public bool female;
    public bool ridingHorse;
    public System.Guid mountGuid;
    public List<System.Guid> groups = new List<System.Guid>();
    public List<System.Guid> enemyGroups = new List<System.Guid>();
    public System.Guid killedBy;

    public bool isAlive {
        get { return float.IsNaN(health) || health > 0; }
    }
}