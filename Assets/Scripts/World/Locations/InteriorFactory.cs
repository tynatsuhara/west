public class InteriorFactory {

    private Map map;
    private TownLocation town;
	private AsciiData ascii = new AsciiData();

    public InteriorFactory(Map map, TownLocation town) {
        this.map = map;
        this.town = town;
    }

    public InteriorLocation InteriorFor(Building.BuildingType type) {
        Room room1 = new Room(ascii.Get("room1"));
		Room room2 = new Room(ascii.Get("room2"));
		
		InteriorBuilder builder = new InteriorBuilder(room1);

		/*var attachData = builder.FindAttachPoint(room2, "##");
		if (attachData != null) {
			builder.AttachRoom(attachData, "//", "//");
		}*/

		builder.TryAttachRoom(ascii.Get("door"), room2, ascii.Get("door"), ascii.Get("doorrep"));
		builder.TryPlaceElement(ascii.Get("bed"));

		return builder.AddTeleporter('T', town.guid, "front door")
					  .Build(map, town.guid, "SOME BUILDING");
    }

	private InteriorLocation InteriorForHome() {
		return null;
	}

	private InteriorLocation InteriorForSaloon() {
		Room mainRoom = new Room(ascii.Get("saloon"));

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