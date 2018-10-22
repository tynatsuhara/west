using UnityEngine;

public class InteriorFactory {

    private Map map;
    private TownLocation town;
	private AsciiData ascii = new AsciiData();

    public InteriorFactory(Map map, TownLocation town) {
        this.map = map;
        this.town = town;
    }

	// key -> ascii shorthand
	private Grid<char> _(string key) {
		return ascii.Get(key);
	}

    public InteriorLocation InteriorFor(Building.BuildingType type) {
		switch (type) {
			case Building.BuildingType.HOME: return InteriorForHome();
			case Building.BuildingType.SALOON: return InteriorForSaloon();
		}
		throw new UnityException("no interior factory method for " + type);
    }

	private InteriorLocation InteriorForHome() {
		Room livingRoom = new Room(_("basic_room_" + Random.Range(0, 2)));
		Room bedroom = new Room(_("bedroom_0"));
		
		InteriorBuilder builder = new InteriorBuilder(livingRoom);

		bool attachedRoom = builder.TryAttachRoom(_("basic_room_door"), bedroom, _("bedroom_door"), _("door_replacement"));
		Debug.Log("attached room? " + attachedRoom);
		bool placedBed = builder.TryPlaceElement(_("bed"), new World.EntityTile(LevelBuilder.PrefabKey.BED));
		Debug.Log("placed bed? " + placedBed);

		return builder.AddTeleporter('T', town.guid, "front door")
					  .Build(map, town.guid, "SOME BUILDING");
	}

	private InteriorLocation InteriorForSaloon() {
		Room mainRoom = new Room(_("saloon"));
		InteriorBuilder builder = new InteriorBuilder(mainRoom);

		/* TODO: possible other rooms:
			- bathroom
			- stairwell up/down to barkeep's living quarters
			- stairwell down to storage cellar 
			- poker room
			- additional seating
			- several random layouts
		*/

		return builder.AddTeleporter('T', town.guid, "front door")
					  .Build(map, town.guid, "SOME SALOON");;		
	}
}