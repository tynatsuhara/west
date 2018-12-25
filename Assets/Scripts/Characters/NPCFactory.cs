using UnityEngine;

public class NPCFactory {

    public NPCData MakeNormie() {
        NPCData npc = new NPCData(NPCData.NPCType.NORMIE, UnityEngine.Random.Range(0, 2) == 0);

        // TODO: allocate schedule dynamically when the game starts?
        // Schedule schedule = new Schedule()
        //     .AddBlock(WorldTime.HOUR * 8, new NPCNoOpTask(home.guid, home.RandomUnoccupiedTile()))
        //     .AddBlock(WorldTime.HOUR * 10, new NPCNoOpTask(work.guid, work.RandomUnoccupiedTile()))
        //     .AddBlock(WorldTime.HOUR * 6, new NPCNoOpTask(home.guid, home.RandomUnoccupiedTile()));
        // npc.taskSources.Add(schedule);
        npc.taskSources[NPCData.DYNAMIC_AI] = new NormieBehavior(npc.guid);
        return npc;
    }

    public NPCData MakeGoon(Group gang) {
        NPCData npc = new NPCData(NPCData.NPCType.GOON, Random.Range(0, 2) == 0);
        npc.outfit = "default";
        gang.Add(npc, false);
        npc.taskSources[NPCData.DYNAMIC_AI] = new CriminalBehavior(npc.guid);
        return npc;
    }

    public NPCData MakeGangLeader(Group gang) {
        NPCData npc = new NPCData(NPCData.NPCType.GANG_LEADER, Random.Range(0, 2) == 0);
        npc.outfit = "default";
        gang.Add(npc, true);
        npc.taskSources[NPCData.DYNAMIC_AI] = new CriminalBehavior(npc.guid);
        return npc;
    }

    public NPCData MakeSheriff() {
        NPCData npc = new NPCData(NPCData.NPCType.SHERIFF, Random.Range(0, 2) == 0);
        npc.outfit = "cop1";
        npc.groups.Add(Group.LAW_ENFORCEMENT);
        npc.taskSources[NPCData.DYNAMIC_AI] = new CopBehavior(npc.guid);
        return npc;
    }

    public NPCData MakeMarshal() {
        NPCData npc = new NPCData(NPCData.NPCType.US_MARSHAL, Random.Range(0, 2) == 0);
        npc.outfit = "cop1";
        npc.groups.Add(Group.LAW_ENFORCEMENT);
        npc.taskSources[NPCData.DYNAMIC_AI] = new CopBehavior(npc.guid);
        return npc;
    }
}