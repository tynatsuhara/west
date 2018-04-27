public class BuildingFactory {

    public Building NewGenericBuilding() {
		// eventually building will have more parameters for unique exteriors
        return new Building(Building.BuildingType.HOME);
    }
}