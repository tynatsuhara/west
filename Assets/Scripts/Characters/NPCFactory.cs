using System;
using UnityEngine;

public class NPCFactory {
    public NPCData MakeNormie(System.Guid work, System.Guid home) {
        NPCData npc = new NPCData(NPCData.NPCType.NORMIE, UnityEngine.Random.Range(0, 2) == 0);

        Schedule schedule = new Schedule()
            .AddBlock(WorldTime.MINUTE * 10, new NPCNoOpTask(work, Vector3.one))
            .AddBlock(WorldTime.MINUTE * 10, new NPCNoOpTask(home, Vector3.one));
        npc.taskSources.Add(schedule);

        return npc;
    }
}