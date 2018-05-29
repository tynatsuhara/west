using UnityEngine;

public class NPCFactory {

    public NPCData MakeNormie() {
        NPCData npc = new NPCData(NPCData.NPCType.NORMIE, UnityEngine.Random.Range(0, 2) == 0);

        // Schedule schedule = new Schedule()
        //     .AddBlock(WorldTime.HOUR * 8, new NPCNoOpTask(home.guid, home.RandomUnoccupiedTile()))
        //     .AddBlock(WorldTime.HOUR * 10, new NPCNoOpTask(work.guid, work.RandomUnoccupiedTile()))
        //     .AddBlock(WorldTime.HOUR * 6, new NPCNoOpTask(home.guid, home.RandomUnoccupiedTile()));
        // npc.taskSources.Add(schedule);

        return npc;
    }

    public NPCData MakeGoon(string group) {
        NPCData npc = new NPCData(NPCData.NPCType.GOON, Random.Range(0, 2) == 0);
        npc.outfit = "default";
        npc.groups.Add(group);
        return npc;
    }

    public NPCData MakeGangLeader(string group) {
        NPCData npc = new NPCData(NPCData.NPCType.GANG_LEADER, Random.Range(0, 2) == 0);
        npc.outfit = "default";
        npc.groups.Add(group);
        return npc;
    }

    public NPCData MakeSheriff() {
        NPCData npc = new NPCData(NPCData.NPCType.SHERIFF, Random.Range(0, 2) == 0);
        npc.outfit = "cop1";
        npc.groups.Add(Group.LAW_ENFORCEMENT);
        return npc;
    }

    public NPCData MakeMarshal() {
        NPCData npc = new NPCData(NPCData.NPCType.US_MARSHAL, Random.Range(0, 2) == 0);
        npc.outfit = "cop1";
        npc.groups.Add(Group.LAW_ENFORCEMENT);
        return npc;
    }
}