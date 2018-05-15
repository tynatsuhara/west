using UnityEngine;
using System.Linq;

public class TownFactory {

    private Map map;
	public readonly string[] ICONS = new string[]{"{", "}", "[", "]", "> <", "*", "@", ">", "<"};    

    public TownFactory(Map map) {
        this.map = map;
    }

    public TownLocation NewLargeTown() {
        return new TownLocation(
            map, 
            x: Random.Range(0, Map.WORLD_COORD_SIZE), 
            y: Random.Range(0, Map.WORLD_COORD_SIZE), 
            icon: ICONS[Random.Range(0, ICONS.Length)],
            additionalPossibleConnections: Random.Range(1, 6),
            homeCount: Random.Range(5, 10),
            saloonCount: Random.Range(0, 3),
            sheriffOfficeCount: Random.Range(0, 2)
        );
    }

    public TownLocation NewSmallTown() {
        return new TownLocation(
            map, 
            x: Random.Range(0, Map.WORLD_COORD_SIZE), 
            y: Random.Range(0, Map.WORLD_COORD_SIZE), 
            icon: "=",
            additionalPossibleConnections: Random.Range(0, 2),
            homeCount: Random.Range(2, 7),
            saloonCount: Random.Range(0, 1),
            sheriffOfficeCount: Random.Range(0, 1)
        );
    }

    public TownLocation NewGangTown() {
        return new TownLocation(
            map, 
            x: Random.Range(0, Map.WORLD_COORD_SIZE), 
            y: Random.Range(0, Map.WORLD_COORD_SIZE), 
            icon: "=",
            controllingGroup: "gang name",
            additionalPossibleConnections: Random.Range(0, 2),
            homeCount: Random.Range(2, 7),
            saloonCount: Random.Range(0, 1),
            gangBarracksCount: Random.Range(1, 2),
            gangHeadquartersCount: 1
        );
    }    

    public TownLocation NewConnectingTown(TownLocation t1, TownLocation t2) {
        // TODO: What actually needs to happen here?
        // If t1 and t2 are already connecting, we can replace the
        // teleporters with teleporters to here 
        return new TownLocation(
            map, 
            x: Random.Range(0, Map.WORLD_COORD_SIZE), 
            y: Random.Range(0, Map.WORLD_COORD_SIZE),
            icon: "~",
            setConnections: new System.Guid[]{t1.guid, t2.guid}.ToList()
        );
    }
}