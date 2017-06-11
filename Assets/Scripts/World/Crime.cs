using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Crime {
    private Dictionary<System.Guid, List<SingleCrime>> committed = new Dictionary<System.Guid, List<SingleCrime>>();

    public void Commit(System.Guid location, string crime, int bounty) {
        if (!committed.ContainsKey(location)) {
            committed[location] = new List<SingleCrime>();
        }
        committed[location].Add(new SingleCrime(crime, bounty));
        Debug.Log("Committed crime of " + crime.ToUpper() + ", bounty += " + bounty);
    }

    public int Bounty(System.Guid location) {
        return !committed.ContainsKey(location) ? 0 : committed[location].Select(x => x.bounty).Aggregate((x, y) => x + y);
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