using System.Collections.Generic;

[System.Serializable]
public class Group {

    public const string LAW_ENFORCEMENT = "Law Enforcement";
    public const string PLAYERS = "Players";

    public string name;
    private Dictionary<System.Guid, bool> members = new Dictionary<System.Guid, bool>();  // maps char -> leader?

    // how do we feel about other groups?
    private Dictionary<string, ReputationWithGroup> groupReputations = new Dictionary<string, ReputationWithGroup>();

    public Group(string name) {
        this.name = name;
    }

    public void Add(Character c, bool leader) {
        c.groups.Add(name);
        members[c.guid] = leader;
    }

    public ReputationWithGroup GetReputationWith(string groupName) {
        if (!groupReputations.ContainsKey(groupName)) {
            groupReputations.Add(groupName, new ReputationWithGroup());
        }
        return groupReputations[groupName];
    }

    public void SetReputationWith(string groupName, ReputationWithGroup reputation) {
        groupReputations[groupName] = reputation;
    }

    /* REPUTATION-AFFECTING METHODS */

    public void KillMember(List<string> killerGroups, System.Guid member) {
        // weight the death of a leader higher than that of a standard member
        ImpactReputations(killerGroups, members[member] ? -5f : .5f);
    }

    private void ImpactReputations(List<string> groups, float value) {
        foreach (string g in groups) {
            GetReputationWith(g).AffectReputation(value);
        }
    }
}