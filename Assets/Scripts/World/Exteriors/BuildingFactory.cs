using UnityEngine;

public class BuildingFactory {

    private TownLocation town;
    private NPCFactory npcFactory;

    public BuildingFactory(TownLocation town, NPCFactory npcFactory) {
        this.town = town;
        this.npcFactory = npcFactory;
    }

    public Building NewHome() {
		// eventually building will have more parameters for unique exteriors
        return new  Building(Building.BuildingType.HOME, "Home")
                .AddHousingSlot(NPCData.NPCType.NORMIE, Random.Range(2, 5))
                .AddNPCs((() => npcFactory.MakeNormie()), 2);
    }

    public Building NewSaloon() {
        return new Building(Building.BuildingType.HOME, "Saloon")
                .AddHousingSlot(NPCData.NPCType.NORMIE, Random.Range(5, 15))
                .AddNPCs((() => npcFactory.MakeNormie()), 2);
    }

    public Building NewGangBarracks(string gang) {
        return new Building(Building.BuildingType.HOME, "Gang Barracks")
                .AddHousingSlot(NPCData.NPCType.GOON, Random.Range(10, 20))
                .AddNPCs((() => npcFactory.MakeGoon(gang)), 2);                
    }

    public Building NewGangHeadquarters(string gang) {
        return new Building(Building.BuildingType.HOME, "Gang Headquarters")
                .AddHousingSlot(NPCData.NPCType.GOON, Random.Range(15, 30))
                .AddHousingSlot(NPCData.NPCType.GANG_LEADER, Random.Range(3, 6))
                .AddNPCs((() => npcFactory.MakeGoon(gang)), 2);                
    }

    public Building NewSheriffsOffice() {
        return new Building(Building.BuildingType.HOME, "Sheriff's Office")
                .AddHousingSlot(NPCData.NPCType.SHERIFF, Random.Range(3, 10))
                .AddHousingSlot(NPCData.NPCType.GOON, Random.Range(0, 4))
                .AddNPCs((() => npcFactory.MakeNormie()), 2);                
    }
}