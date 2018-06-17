using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TownFactory {

    private Map map;
    private BuildingFactory bf;
	public readonly string[] ICONS = new string[]{"{", "}", "[", "]", "> <", "*", "@", ">", "<"};    

    public TownFactory(Map map) {
        this.map = map;
        this.bf = new BuildingFactory(new NPCFactory());
    }

    public TownLocation NewLargeTown() {
        return new TownLocation(
            NameGen.TownName(),
            map, 
            icon: ICONS[Random.Range(0, ICONS.Length)],
            availableConnections: Random.Range(2, 6),
            buildingsToAttempt: new List<Building>[] {
                b(Random.Range(5, 10), () => bf.NewHome()), 
                b(Random.Range(0, 3), () => bf.NewSaloon()),
                b(Random.Range(1, 2), () => bf.NewSheriffsOffice())
            }
        );
    }

    public TownLocation NewSmallTown() {
        return new TownLocation(
            NameGen.TownName(),
            map, 
            icon: "s",
            availableConnections: Random.Range(1, 3),
            buildingsToAttempt: new List<Building>[] {
                b(Random.Range(2, 7), () => bf.NewHome()), 
                b(Random.Range(0, 1), () => bf.NewSaloon()),
                b(Random.Range(0, 1), () => bf.NewSheriffsOffice())
            }
        );
    }

    public TownLocation NewGangTown(string gang) {
        return new TownLocation(
            gang + " Hideout",
            map, 
            icon: "H",
            controllingGroup: gang,
            availableConnections: Random.Range(1, 3),
            buildingsToAttempt: new List<Building>[] {
                b(1, () => bf.NewGangHeadquarters(gang)),
                b(Random.Range(1, 2), () => bf.NewGangBarracks(gang)),
                b(Random.Range(0, 2), () => bf.NewHome()), 
                b(Random.Range(0, 1), () => bf.NewSaloon())
            }
        );
    }   

    public TownLocation NewConnectingTown(TownLocation t1, TownLocation t2) {
        // TODO: What actually needs to happen here?
        // If t1 and t2 are already connecting, we can replace the
        // teleporters with teleporters to here 
        return new TownLocation(
            "Wilderness",
            map, 
            icon: "~",
            setConnections: new System.Guid[]{t1.guid, t2.guid}.ToList()
        );
    }

    // shorthand for a list of buildings
    private List<Building> b(int count, System.Func<Building> supplier) {
        return Enumerable.Range(0,  count).Select(f => supplier()).ToList();
    } 
}