public class InteriorFactory {

    private Map map;
    private TownLocation town;

    public InteriorFactory(Map map, TownLocation town) {
        this.map = map;
        this.town = town;
    }

    public InteriorLocation InteriorFor(Building.BuildingType type) {
        Room room1 = new Room('a', '/',
			"      #//T/#",
			"      #////#",
			"//####/////#",
			"///////#####"
		);

		Room room2 = new Room('b', '/',
			"///////////#",
			"///////////#",
			"///////////#",
			"############"
		);
		
		return new InteriorBuilder(room1)
				.Attach("##", room2)
				.AddTeleporter('T', town.guid, "front door")
				.Build(map, town.guid, "SOME BUILDING");
    }
}