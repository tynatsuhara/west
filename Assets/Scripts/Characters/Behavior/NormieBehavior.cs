using UnityEngine;
using System.Linq;

[System.Serializable]
public class NormieBehavior : DynamicBehavior {

    public NormieBehavior(System.Guid self) : base(self) {}

    public override void React(Stimulus s, Vector3 location, Character alerter) {   
        // base.React(s, location, alerter);
        Character c = GameManager.instance.GetCharacter(self);
        switch (s) {
            case Stimulus.SHOOTING:
            case Stimulus.DEAD_BODY:
            case Stimulus.GUN_DRAWN:
            case Stimulus.HORSE_THEFT:
            case Stimulus.VIOLENCE:
            case Stimulus.ATTACKED:
            case Stimulus.MURDER:
            case Stimulus.JUST_CHILLIN:
            default:
                break;
        }
    }
}