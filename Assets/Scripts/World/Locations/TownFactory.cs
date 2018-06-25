using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TownFactory {

    private Map map;
    private BuildingFactory bf;
	public readonly string[] ICONS = new string[]{"{", "}", "[", "]", "> <", "*", "@", ">", "<"};    

    private List<System.Func<TownLocation>> townSuppliers = new List<System.Func<TownLocation>>();
    private List<float> townWeights = new List<float>();
    private float weightSum;

    public TownFactory(Map map) {
        this.map = map;
        this.bf = new BuildingFactory(new NPCFactory());

        Weight(2, () => NewGangTown());
        Weight(6, () => NewLargeTown());
        Weight(4, () => NewSmallTown());
    }

    private void Weight(float weight, System.Func<TownLocation> func) {
        townSuppliers.Add(func);
        townWeights.Add(weight);
        weightSum += weight;
    }

    // Returns a town using the defined weights above
    public TownLocation NewRandomTown() {
        float val = Random.Range(0, weightSum);
        float run = 0;
		for (int i = 0; i < townSuppliers.Count; i++) {
            run += townWeights[i];
            if (val < run) {
                return townSuppliers[i]();
            }
        }
        Debug.Log("something fucked up :o");
        return null;
    }

    public TownLocation NewLargeTown() {
        return new TownLocation(
            NameGen.TownName(),
            map, 
            icon: ICONS[Random.Range(0, ICONS.Length)],
            availableConnections: Random.Range(2, 6),
            buildingsToAttempt: new List<Building>[] {
                b(Random.Range(5, 10), () => bf.NewHome()), 
                b(Random.Range(1, 3), () => bf.NewSaloon()),
                b(Random.Range(1, 3), () => bf.NewSheriffsOffice())
            }
        );
    }

    public TownLocation NewSmallTown() {
        return new TownLocation(
            NameGen.TownName(),
            map, 
            icon: "s",
            availableConnections: Random.Range(2, 4),
            buildingsToAttempt: new List<Building>[] {
                b(Random.Range(2, 7), () => bf.NewHome()), 
                b(Random.Range(0, 2), () => bf.NewSaloon()),
                b(Random.Range(0, 2), () => bf.NewSheriffsOffice())
            }
        );
    }

    public TownLocation NewGangTown(string gangName = null) {
        if (gangName == null) {
            do {
                gangName = NameGen.GangName(NameGen.CharacterFirstName(), NameGen.CharacterLastName());
                Debug.Log("new gang name " + gangName);				
            } while (SaveGame.currentGame.groups.ContainsKey(gangName));
            Group gang = new Group(gangName);
            SaveGame.currentGame.groups.Add(gangName, gang);
            Group cops = SaveGame.currentGame.groups[Group.LAW_ENFORCEMENT];
            gang.SetReputationWith(Group.LAW_ENFORCEMENT, new Reputation((float) Reputation.Rank.ENEMIES));
            cops.SetReputationWith(gangName, new Reputation((float) Reputation.Rank.HATE));
        }

        return new TownLocation(
            gangName + " Hideout",
            map, 
            icon: "H",
            controllingGroup: gangName,
            availableConnections: Random.Range(1, 3),
            buildingsToAttempt: new List<Building>[] {
                b(1, () => bf.NewGangHeadquarters(gangName)),
                b(Random.Range(1, 3), () => bf.NewGangBarracks(gangName)),
                b(Random.Range(0, 4), () => bf.NewHome()), 
                b(Random.Range(0, 2), () => bf.NewSaloon())
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