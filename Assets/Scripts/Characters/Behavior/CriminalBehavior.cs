using UnityEngine;
using System.Linq;

[System.Serializable]
public class CriminalBehavior : DynamicBehavior {

    public CriminalBehavior(System.Guid self) : base(self) {}

    public override void React(Stimulus s, Vector3 location, Character alerter) {   
        // base.React(s, location, alerter);
        Character c = GameManager.instance.GetCharacter(self);
        switch (s) {
            case Stimulus.SHOOTING:
            case Stimulus.DEAD_BODY:
                stimuli.Add(s, new NPCInvestigateTask(location));
                break;
            case Stimulus.GUN_DRAWN:
            case Stimulus.HORSE_THEFT:
            case Stimulus.VIOLENCE:
            case Stimulus.ATTACKED:
            case Stimulus.MURDER:
                if (!c.IsFriendsWith(alerter)) {
                    stimuli.Add(s, new NPCKillTask(alerter.guid));
                }
                break;
            case Stimulus.JUST_CHILLIN:
                if (c.IsEnemiesWith(alerter)) {
                    stimuli.Add(s, new NPCKillTask(alerter.guid));
                }
                break;
            default:
                break;
        }
    }
}