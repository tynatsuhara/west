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
        Character c = GameManager.instance.GetCharacter(self);
        if (c == null)
            return null;

        while (stimuli.Count > 0) {
            NPCTask task = stimuli[0];
            // TODO: getTimeLeft may be fucked            
            if (task.GetTimeLeft() > 0) {
                return task;
            }
            stimuli.RemoveAt(0);
        }
        c.HideWeapon();
        c.LoseLookTarget();
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
    GUN_DRAWN,
    JUST_CHILLIN
}