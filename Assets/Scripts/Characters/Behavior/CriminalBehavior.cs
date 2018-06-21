using UnityEngine;
using System.Linq;

[System.Serializable]
public class CriminalBehavior : DynamicBehavior {

    public CriminalBehavior(System.Guid self) : base(self) {}

    public override void React(Stimulus s, Vector3 location) {   
        base.React(s, location);
    } 

    public override NPCTask GetTask(System.Guid character, float time) {
        while (stimuli.Count > 0) {
            var stim = stimuli.First();
            switch (stim.Key) {
                case Stimulus.GUN_DRAWN:
                case Stimulus.SHOOTING:
                case Stimulus.HORSE_THEFT:
                case Stimulus.VIOLENCE:
                case Stimulus.DEAD_BODY:
                case Stimulus.ATTACKED:
                case Stimulus.MURDER:
                    // TODO: how do we not create a new task each time?
                    // TODO: check groups, don't just kill with reckless abandon
                    NPCTask task = new NPCKillTask(GameManager.players[0].guid);
                    if (task.GetTimeLeft() <= 0) {
                        break;
                    }
                    return task;
                default:
                    Debug.Log("unrecognized stimulus " + stim.Key);
                    break;
            }
            NextStimulus();            
        }
        return null;
    }
}