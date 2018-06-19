using UnityEngine;

public class BuildingFactory {

    private NPCFactory npcFactory;

    public BuildingFactory(NPCFactory npcFactory) {
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
                .AddNPCs((() => npcFactory.MakeSheriff()), Random.Range(1, 6))
                .AddNPCs(() => npcFactory.MakeMarshal(), Random.Range(0, 5) == 0 ? 1 : 0);                
    }

    // TODO

    public Building NewBank() {
        return null;
    }

    public Building NewGeneralStore() {
        return null;
    }      

    public Building NewGunShop() {
        return null;
    }    

    public Building NewDoctorsOffice() {
        return null;
    }

    public Building NewTailor() {
        return null;
    }    

    public Building NewStables() {
        return null;
    }

    public Building NewBarber() {
        return null;
    }    

    public Building NewGraveyard() {
        return null;
    }
}