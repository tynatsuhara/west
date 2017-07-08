using UnityEngine;

[System.Serializable]
public class CharacterSpeechEvent : WorldEvent {
    private System.Guid character;
    private string message;

    public CharacterSpeechEvent(System.Guid character, string message) {
        this.character = character;
        this.message = message;
    }

    public override void Execute() {
        if (Map.CurrentLocation() == Map.LocationOfCharacter(character)) {
            Character c = GameManager.instance.GetCharacter(character);
            if (c != null && c.isAlive) {
                c.speech.Say(message);
            }
        }
    }
}