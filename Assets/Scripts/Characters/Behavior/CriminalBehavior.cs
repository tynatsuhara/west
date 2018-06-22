using UnityEngine;
using System.Linq;

[System.Serializable]
public class CriminalBehavior : DynamicBehavior {

    public CriminalBehavior(System.Guid self) : base(self) {}

    public override void React(Stimulus s, Vector3 location, Character alerter) {   
        // base.React(s, location, alerter);
        switch (s) {
            case Stimulus.GUN_DRAWN:
            case Stimulus.SHOOTING:
            case Stimulus.HORSE_THEFT:
            case Stimulus.VIOLENCE:
            case Stimulus.DEAD_BODY:
            case Stimulus.ATTACKED:
            case Stimulus.MURDER:
                // TODO: check groups, don't just kill with reckless abandon
                NPCTask task = new NPCKillTask(alerter.guid);
                // TODO: getTimeLeft may be fucked
                if (task.GetTimeLeft() > 0) {
                    stimuli.Add(s, task);
                }
                break;
            default:
                break;
        }
    }
}