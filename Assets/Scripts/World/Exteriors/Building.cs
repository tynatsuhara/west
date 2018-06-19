using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: probably rewrite all of this when exterior generation is a thing

[System.Serializable]
public class Building {

    public enum BuildingType {
        HOME,
        SALOON
    }

    public readonly BuildingType type;
    public Dictionary<NPCData.NPCType, int> npcHousingSlots = new Dictionary<NPCData.NPCType, int>();
    public List<NPCData> npcs = new List<NPCData>();

    private string name;

    public Building(BuildingType type, string name) {
        this.type = type;
        this.name = name;
    }

    public Building AddHousingSlot(NPCData.NPCType type, int count) {
        npcHousingSlots.Add(type, count);
        return this;
    }

    public Building AddNPCs(System.Func<NPCData> supplier, int count) {
        for (int i = 0; i < count; i++) {
            npcs.Add(supplier());
        }
        return this;
    }

    public Building AddNPC(NPCData npc) {
        npcs.Add(npc);
        return this;
    }

    // all this shit is disgusting

    // TODO: these TDs should store relative positions to the center of the building?
    public List<Teleporter.TeleporterData> doors = new List<Teleporter.TeleporterData>();
    public System.Guid guid;

    // predefined for each building type (later set with builder class)
    public int prefabIndex = 0;
    private int width_ = 4;
    private int height_ = 3;

    // set in TownLocation.cs
    public int bottomLeftTile;

    public void GenerateInterior(Map map, TownLocation town) {
        InteriorFactory factory = new InteriorFactory(map, town);
        InteriorLocation interior = factory.InteriorFor(type);
        interior.name = name;
        
        // set the interior
        guid = interior.guid;
        map.locations[guid] = interior;

        // link the town to the interior
        map.locations[guid].connections.Add(town.guid);
        map.locations[town.guid].connections.Add(guid);

        // add teleporters
        doors.Add(new Teleporter.TeleporterData(interior.guid, Vector3.one, "front door", permitHorses: false));
    }


    // !!!! Everything below here is trash and will be rewritten later !!!! //

    private int doorOffsetX_ = 1;
    private int doorOffsetY_ = 0;

    public int angle {
        get { return rot * 90; }
    }
    public int width {
        get {
            if (rot % 2 == 1) {
                return height_;
            }
            return width_;
        }
    }
    public int height {
        get {
            if (rot % 2 == 1) {
                return width_;
            }
            return height_;
        }
    }
    public int doorOffsetX {  // where to build the road to
        get {
            if (rot == 0) {
                return doorOffsetX_;
            } else if (rot == 1) {
                return doorOffsetY_;
            } else if (rot == 2) {
                return -doorOffsetX_ + width_ - 1;
            } else {
                return -doorOffsetY_ + height_ - 1;
            }
        }
    }
    public int doorOffsetY {
        get {
            if (rot == 0) {
                return doorOffsetY_;
            } else if (rot == 1) {
                return width_ - doorOffsetX_ - 1;
            } else if (rot == 2) {
                return height_ - doorOffsetY_ - 1;
            } else {
                return doorOffsetX_;
            }
        }
    }

    private int rot = 0;
    public int rotation {
        get { return rot; }
        set {
            if (value < 0 || value > 3) {
                throw new UnityException("invalid rotation value " + value);
            }
            while (rot != value) {
                Rotate();
            }
        }
    }

    // rotates 90 degrees clockwise
    public void Rotate(int times = 1) {
        for (int i = 0; i < times; i++) {
            rot = (rot + 1) % 4;
            int temp = width;
            width_ = height;
            height_ = temp;

            foreach (Teleporter.TeleporterData td in doors) {
                // rotate 90 degrees
                td.position = new SerializableVector3(Quaternion.AngleAxis(90, Vector3.up) * td.position.val);
                // Debug.Log(td.position.val);
            }
        }
    }
}