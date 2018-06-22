using UnityEngine;
using System.Collections.Generic;

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

    public abstract NPCTask GetTask(System.Guid character, float time);

    public virtual void React(Stimulus s, Vector3 location, Character alerter) {
        GameManager.instance.GetCharacter(self).speech.Say("I am reacting to a " + s + " event");
    }

    protected void NextStimulus() {
        stimuli.RemoveAt(0);
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