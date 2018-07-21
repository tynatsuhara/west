public class InteriorFactory {

    private Map map;
    private TownLocation town;

    public InteriorFactory(Map map, TownLocation town) {
        this.map = map;
        this.town = town;
    }

    public InteriorLocation InteriorFor(Building.BuildingType type) {
        Room room1 = new Room(
			"      #//T/#",
			"      #////#",
			"//####/////#",
			"///////#####"
		);

		Room room2 = new Room(
			"///////////#",
			"///////////#",
			"///////////#",
			"############"
		);
		
		InteriorBuilder builder = new InteriorBuilder(room1);

		var attachData = builder.FindAttachPoint(room2, "##");
		if (attachData != null) {
			builder.AttachRoom(attachData, "//", "//");
		}

		return builder.AddTeleporter('T', town.guid, "front door")
					  .Build(map, town.guid, "SOME BUILDING");
    }

	private InteriorLocation InteriorForHome() {
		return null;
	}

	private InteriorLocation InteriorForSaloon() {
		Room mainRoom = new Room(
			"#$$$$$$$$$$#",
			"#----------#",
			"#//////////#",
			"#//////////#",
			"#++++++++++#"
		);

		/* TODO: possible other rooms:
			- bathroom
			- stairwell up/down to barkeep's living quarters
			- stairwell down to storage cellar 
			- poker room
			- additional seating
			- several random layouts
		*/

		return null;		
	}
}