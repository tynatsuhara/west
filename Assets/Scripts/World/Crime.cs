using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Crime {
    public enum WantedStatus {
        NOT_WANTED,
        ALIVE,  // not used right now, maybe later
        DEAD_OR_ALIVE
    }

    // maps character guid -> location guid -> bounty
    private Dictionary<System.Guid, Dictionary<System.Guid, List<SingleCrime>>> committed = new Dictionary<System.Guid, Dictionary<System.Guid, List<SingleCrime>>>();

    public void Commit(System.Guid character, System.Guid location, string crime, int bounty) {
        if (!committed.ContainsKey(character)) {
            committed.Add(character, new Dictionary<System.Guid, List<SingleCrime>>());
        }
        Dictionary<System.Guid, List<SingleCrime>> characterCrimes = committed[character];
        if (!characterCrimes.ContainsKey(location)) {
            characterCrimes[location] = new List<SingleCrime>();
        }
        characterCrimes[location].Add(new SingleCrime(crime, bounty));
        Debug.Log("Committed crime of " + crime.ToUpper() + ", bounty += " + bounty);
    }

    public int Bounty(System.Guid character) {
        if (!committed.ContainsKey(character)) {
            return 0;
        }
        int result = 0;
        foreach (System.Guid location in committed[character].Keys) {
            result += committed[character][location].Select(x => x.bounty).Aggregate((x, y) => x + y);
        }
        return result;
    }

    public int Bounty(System.Guid character, System.Guid location) {
        if (!committed.ContainsKey(character) || !committed[character].ContainsKey(location))
            return 0;
        return committed[character][location].Select(x => x.bounty).Aggregate((x, y) => x + y);
    }

    public WantedStatus GetWantedStatus(System.Guid character) {
        if (!committed.ContainsKey(character) || committed[character].Count == 0) {
            return WantedStatus.NOT_WANTED;
        } else {
            return WantedStatus.DEAD_OR_ALIVE;
        }
    }

    [System.Serializable] 
    private class SingleCrime {
        public string crime;
        public int bounty;

        public SingleCrime(string crime, int bounty) {
            this.crime = crime;
            this.bounty = bounty;
        }
    }
}