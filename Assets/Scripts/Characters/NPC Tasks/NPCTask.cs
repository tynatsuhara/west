using System;

/**
    NPC Planning
    Type of NPC / Behavior
    Civilian - goes about daily schedule, whatever that may be (work, home, sleep, etc)
    Sheriff - Patrols town, will fight back criminals in their town. If a criminal has a high
              enough bounty in the town, they will pursue them in other towns.
    US Marshals - Sent in if a criminal has high bounties in multiple towns. Will hunt them down
                  until they are dead, then head back to their HQ.
    Criminal - Goes from town to town committing crimes. Will ocassionally go back to hideout.

    
    Task Options
    Scheduled time and duration
    Task priority:
        1. Dynamic behavior (only in loaded location, not in simulation)
        2. Quest behavior
        3. Daily schedule

    Solution: TaskSource interface. NPCs store references to all their TaskSources
 */

[System.Serializable]
public abstract class NPCTask {
    public int priority;
    public abstract Task.TaskDestination GetLocation();
}