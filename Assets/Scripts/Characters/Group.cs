using System.Collections.Generic;

[System.Serializable]
public class Group {

    public const string CRIMINALS = "Criminals";

    public System.Guid guid = System.Guid.NewGuid();
    public string name;
    HashSet<System.Guid> leaders = new HashSet<System.Guid>();
    HashSet<System.Guid> members = new HashSet<System.Guid>();

    public Group(string name) {
        this.name = name;
    }

    public void Add(Character c, bool leader) {
        c.groups.Add(guid);
        (leader ? leaders : members).Add(c.guid);
    }
}