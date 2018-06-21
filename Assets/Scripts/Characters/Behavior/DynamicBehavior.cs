using UnityEngine;
using System.Collections.Generic;

/*
    This class is where actual decisions are made based on stimuli
 */
[System.Serializable]
public abstract class DynamicBehavior : NPCTaskSource {

    private System.Guid self;
    protected SortedList<Stimulus, SerializableVector3> stimuli = new SortedList<Stimulus, SerializableVector3>(new DuplicateKeyComparer<Stimulus>());

    public DynamicBehavior(System.Guid self) {
        this.self = self;
    }

    public abstract NPCTask GetTask(System.Guid character, float time);

    public virtual void React(Stimulus s, Vector3 location) {
        GameManager.instance.GetCharacter(self).speech.Say("I am reacting to a " + s + " event");
        stimuli.Add(s, new SerializableVector3(location));
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