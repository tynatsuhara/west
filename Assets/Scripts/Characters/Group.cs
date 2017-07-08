using System.Collections.Generic;

[System.Serializable]
public class Group {

    public const string CRIMINALS = "Criminals";

    public System.Guid guid = System.Guid.NewGuid();
    public string name;
    List<System.Guid> leaders = new List<System.Guid>();
    List<System.Guid> members = new List<System.Guid>();

    public Group(string name) {
        this.name = name;
    }

    public void Add(Character c, bool leader) {
        c.groups.Add(guid);
        var list = leader ? leaders : members;
        if (!list.Contains(c.guid))
            list.Add(c.guid);
    }
}