using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*
    This class is where actual decisions are made based on stimuli
 */
[System.Serializable]
public abstract class DynamicBehavior : NPCTaskSource {

    protected System.Guid self;
    protected SortedList<Stimulus, NPCTask> stimuli = new SortedList<Stimulus, NPCTask>(new DuplicateKeyComparer<Stimulus>());

    public DynamicBehavior(System.Guid self) {
        this.self = self;
    }

    public virtual void React(Stimulus s, Vector3 location, Character alerter) {
        GameManager.instance.GetCharacter(self).speech.Say("I am reacting to a " + s + " event");
    }

    public NPCTask GetTask(System.Guid character, float time) {
        while (stimuli.Count > 0) {
            NPCTask task = stimuli.First().Value;
            if (task.GetTimeLeft() > 0) {
                return task;
            }
            stimuli.RemoveAt(0);
        }
        return null;
    }
}

// Ranked in decreasing order of severeness
public enum Stimulus {
    MURDER,
    ATTACKED,
    DEAD_BODY,
    VIOLENCE,
    HORSE_THEFT,
    SHOOTING,
    GUN_DRAWN
}