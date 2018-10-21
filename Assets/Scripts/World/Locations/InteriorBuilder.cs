using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using World;

public class InteriorBuilder {

    private List<Room> rooms = new List<Room>();
    private Grid<Room> grid;
    private Room root;

    public InteriorBuilder(Room root) {
        this.root = root;
        rooms.Add(root);
        grid = new Grid<Room>(root.width, root.height);
        for (int x = 0; x < grid.width; x++) {
            for (int y = 0; y < grid.height; y++) {
                grid.Set(x, y, root.Occupied(x, y) ? root : null);
            }
        }
    }

    public InteriorLocation Build(Map parent, System.Guid outside, string name) {
        return new InteriorLocation(parent, outside, TilesFromRoomGrid(), grid, name, teleporters);
    }

    private Grid<List<TileElement>> TilesFromRoomGrid() {
        Grid<List<TileElement>> tiles = new Grid<List<TileElement>>(grid.width, grid.height);
        grid.ForEach((room, x, y) => {
            if (room != null) {
                tiles.Set(x, y, room.TileElementsAt(x, y));
            }
        });
        return tiles;
    }

    public bool TryPlaceElement(Grid<char> g, TileElement element = null) {
        for (int i = 0; i < 4; i++) {
            List<FindResult> findResults = Find(g);
            if (findResults.Count > 0) {
                FindResult fr = findResults.First();
                fr.room.WriteASCII(g, fr.x, fr.y);
                if (element != null) {
                    grid.Get(fr.x, fr.y).AddTileElementAt(fr.x, fr.y, element);
                    for (int j = 0; j < i; j++) {
                        element.Rotate();
                    }
                }
                return true;
            }
            Rotate();
        }
        return false;
    }

    // private void PlaceGrid(Grid<char> g, FindResult fr) {
    //     for (int x = 0; x < g.width; x++) {
    //         for (int y = 0; y < g.height; y++) {
    //             int realX = x + fr.x;
    //             int realY = y + fr.y;
                
    //             Room r = grid.Set(realX, realY);
    //             if (g.Get(x, y) != ' ' && r != null && r.Occupied(realX, realY)) {
    //                 return false;
    //             }
    //         }
    //     }
    // }

    // private bool CanPlaceGrid(Grid<char> g, FindResult fr) {
    //     for (int x = 0; x < g.width; x++) {
    //         for (int y = 0; y < g.height; y++) {
    //             int realX = x + fr.x;
    //             int realY = y + fr.y;
    //             Room r = grid.Get(realX, realY);
    //             if (g.Get(x, y) != ' ' && r != null && r.Occupied(realX, realY)) {
    //                 return false;
    //             }
    //         }
    //     }
    //     return true;
    // }

    private List<Teleporter.TeleporterData> teleporters = new List<Teleporter.TeleporterData>();
    public InteriorBuilder AddTeleporter(char gridIcon, System.Guid town, string tag) {
        FindResult fr = Find(new Grid<char>(gridIcon)).First();
        Vector3 pos = new Vector3((fr.x + .5f) * LevelBuilder.TILE_SIZE, 1f, (fr.y + .5f) * LevelBuilder.TILE_SIZE);
        teleporters.Add(new Teleporter.TeleporterData(town, pos, tag));
        return this;
    }

    // public InteriorBuilder ReplaceWithFloor(string toReplace) {
    //     for (int x = 0; x < grid.width; x++) {
    //         for (int y = 0; y < grid.height; y++) {
    //             Room rm = grid.Get(x, y);
    //             if (rm != null) {
    //                 rm.SquareAt(x, y).ch = rm.floor;
    //             }
    //         }
    //     }
    //     return this;
    // }

    public class RoomAttachResult {
        public Room room;
        public FindResult f1;
        public FindResult f2;
        public bool placeOtherAbove;
    }

    // rooms should be attached in order of importance (building out from the root room)
    // returns whether or not the room successfully attached
    // both doors should have the attach point at the TOP of the grid
    public bool TryAttachRoom(Grid<char> thisDoor, Room newRoom, Grid<char> newRoomDoor, Grid<char> replacement) {
        if (thisDoor.width != replacement.width || thisDoor.width != replacement.width 
                || thisDoor.height != 1 || newRoomDoor.height != 1 || replacement.height != 1) {  // todo: make doors > 1 tall
            throw new UnityException("illegal grid dimensions");
        }
        RoomAttachResult rar = FindAttachPoint(thisDoor, newRoom, newRoomDoor);
        if (rar == null) {
            return false;
        }
        AttachRoom(rar, replacement);
        return true;
    }

    // Rotation should be [0, 3]
    public void SetRotation(int rotation) {
        while (root.rotation != rotation) {
            Rotate();
        }
    }

    // returns attach metadata or null if there is no possible attachment
    public RoomAttachResult FindAttachPoint(Grid<char> thisDoor, Room room, Grid<char> newRoomDoor) {

        int initialRotation = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < initialRotation; i++) {
            room.Rotate();
        }

        for (int i = 0; i < 2; i++) {
            // get all "on" strings in the other grid with space above/below them
            List<FindResult> thatDoors = room.Find(newRoomDoor);
            List<FindResult> thatSpaceAboveDoors = thatDoors.Where(x => x.spaceAbove).OrderBy(x => Random.Range(0f, 1f)).ToList();
            List<FindResult> thatSpaceBelowDoors = thatDoors.Where(x => x.spaceBelow).OrderBy(x => Random.Range(0f, 1f)).ToList();
            
            if (thatSpaceAboveDoors.Count + thatSpaceBelowDoors.Count > 0) {
                for (int j = 0; j < 4; j++) {
                    List<FindResult> thisDoors = Find(thisDoor);
                    List<FindResult> spaceAboveDoors = thisDoors.Where(x => x.spaceAbove).OrderBy(x => Random.Range(0f, 1f)).ToList();
                    List<FindResult> spaceBelowDoors = thisDoors.Where(x => x.spaceBelow).OrderBy(x => Random.Range(0f, 1f)).ToList();

                    // weird ass shit to randomize looking above/below first
                    List<FindResult>[] sides;
                    bool checkAbove = UnityEngine.Random.Range(0, 2) == 0;
                    if (checkAbove) {
                        sides = new List<FindResult>[]{spaceAboveDoors, thatSpaceBelowDoors, spaceBelowDoors, thatSpaceAboveDoors};
                    } else {
                        sides = new List<FindResult>[]{spaceBelowDoors, thatSpaceAboveDoors, spaceAboveDoors, thatSpaceBelowDoors};
                    }
                    
                    for (int f = 0; f < 3; f += 2) {
                        foreach (FindResult f1 in sides[f]) {
                            foreach (FindResult f2 in sides[f+1]) {
                                if (CanMerge(f1, f2, room, checkAbove)) {
                                    // return true;
                                    RoomAttachResult rar = new RoomAttachResult();
                                    rar.room = room;
                                    rar.f1 = f1;
                                    rar.f2 = f2;
                                    rar.placeOtherAbove = checkAbove;
                                    return rar;
                                }
                            }
                        }
                        checkAbove = !checkAbove;
                    }

                    Rotate();
                }
            }

            room.Rotate();
        }

        return null;
    }

    // Returns a list of possible placement results in a randomized order for the current rotation
    private List<FindResult> Find(Grid<char> g) {
        return rooms.SelectMany(x => x.Find(g))
                    .OrderBy(n => Random.Range(0f, 1f))
                    .ToList();
    }

    // Merge and resize the grid (will always be a rectangle, no different-length rows)
    private bool CanMerge(FindResult thisPos, FindResult otherPos, Room other, bool placeOtherAbove) {
        int shift = placeOtherAbove ? 1 : -1;

        // the coordinates in this grid for placing other
        int bottomLeftX = thisPos.x - otherPos.x;
        int bottomLeftY = thisPos.y - otherPos.y + shift;

        // make sure all overlapping is safe and account for padding
        for (int y = 0; y < other.height; y++) {
            for (int x = 0; x < other.width; x++) {
                int overlapX = bottomLeftX + x;
                bool outsideX = overlapX < 0 || overlapX >= grid.width;

                int overlapY = bottomLeftY + y;
                bool outsideY = overlapY < 0 || overlapY >= grid.height;
                
                if (!outsideX && !outsideY && other.Occupied(x, y) && grid.Get(overlapX, overlapY) != null) {
                    return false;
                }
            }
        }

        return true;
    }

    public void AttachRoom(RoomAttachResult roomAttachResult, Grid<char> replacement) {
        FindResult thisPos = roomAttachResult.f1;
        FindResult otherPos = roomAttachResult.f2;
        Room other = roomAttachResult.room;
        bool placeOtherAbove = roomAttachResult.placeOtherAbove;

        int minX = 0, maxX = 0, minY = 0, maxY = 0;
        int shift = placeOtherAbove ? 1 : -1;

        // the coordinates in this grid for placing other
        int bottomLeftX = thisPos.x - otherPos.x;
        int bottomLeftY = thisPos.y - otherPos.y + shift;

        // find all min/max values
        for (int y = 0; y < other.height; y++) {
            for (int x = 0; x < other.width; x++) {
                int overlapX = bottomLeftX + x;
                minX = Mathf.Min(minX, overlapX);
                maxX = Mathf.Max(maxX, overlapX);

                int overlapY = bottomLeftY + y;
                minY = Mathf.Min(minY, overlapY);
                maxY = Mathf.Max(maxY, overlapY);
            }
        }

        int bottomPad = Mathf.Max(-minY, 0);
        int topPad = Mathf.Max(maxY + 1 - grid.height, 0);
        int leftPad = Mathf.Max(-minX, 0);
        int rightPad = Mathf.Max(maxX + 1 - grid.width, 0);

        // adjust the size of the grid
        bottomLeftY += bottomPad;
        bottomLeftX += leftPad;

        grid = grid.Expanded(topPad, bottomPad, leftPad, rightPad);

        foreach (Room movedRoom in rooms) {
            movedRoom.IncrementOffset(leftPad, bottomPad);
        }

        // place references in grid for occupied spaces
        for (int x = 0; x < other.width; x++) {
            for (int y = 0; y < other.height; y++) {
                if (other.Occupied(x, y)) {
                    grid.Set(bottomLeftX + x, bottomLeftY + y, other);                    
                }
            }
        }

        other.IncrementOffset(bottomLeftX, bottomLeftY);

        MakeDoorway(bottomLeftX + otherPos.x, bottomLeftY + otherPos.y, !placeOtherAbove, replacement);
        MakeDoorway(leftPad + thisPos.x, bottomPad + thisPos.y, placeOtherAbove, replacement);

        rooms.Add(other);
    }

    // Occupies each entry in the grid (except for where there are spaces in g)
    private void OccupySpace(int x, int y, Grid<char> g) {
        for (int x_ = 0; x_ < g.width; x_++) {
            for (int y_ = 0; y_ < g.height; y_++) {
                Room rm = grid.Get(x + x_, y + y_);
                if (g.Get(x_, y_) != ' ' && rm != null) {
                    rm.TileElementsAt(x + x_, y + y_)
                            .Where(el => el is FloorTile)
                            .Select(el => el as FloorTile)
                            .ToList()
                            .ForEach(floorTile => {
                                floorTile.occupied = true;
                            });
                }
            }
        }
    }

    private void MakeDoorway(int x, int y, bool removeTopDoor, Grid<char> replacement) {
        for (int i = 0; i < replacement.width; i++) {
            Room rm = grid.Get(x + i, y);
            if (rm != null) {
                rm.TileElementsAt(x + i, y)
                        .Where(el => el is FloorTile)
                        .Select(el => el as FloorTile)
                        .ToList()
                        .ForEach(floorTile => {
                            floorTile.occupied = true;
                            if (removeTopDoor) {
                                floorTile.wallTop = false;
                            } else {
                                floorTile.wallBottom = false;
                            }
                        });
            }
        }
    }

    // rotate clockwise
    private void Rotate() {
        foreach (Room room in rooms) {
            room.RotatePlaced(grid.width);
        }
        grid = grid.RotatedClockwise();
    }

    public override string ToString() {
        return grid.ToString((room, x, y) => {
            if (room == null) {
                return ' ';
            }
            World.FloorTile sq = room.TileElementsAt(x, y).Where(el => el is FloorTile).First() as World.FloorTile;
            return (sq.wallTop || sq.wallBottom || sq.wallLeft || sq.wallRight) ? '#' : '/';
        });
    }

    public class FindResult {
        public Room room;
        public int x, y;  // represents the bottom left of the overlap in interior space
        public bool spaceAbove, spaceBelow;

        public FindResult(Room room, int x, int y, bool spaceAbove, bool spaceBelow) {
            this.room = room;
            this.x = x;
            this.y = y;
            this.spaceAbove = spaceAbove;
            this.spaceBelow = spaceBelow;
        }

        public override string ToString() {
            return "[" + string.Join(", ", new string[]{x.ToString(), y.ToString(), spaceAbove.ToString(), spaceBelow.ToString()}) + "]";
        }
    }

    // private void CheckRep() {
    //     int w = grid.First().Count;
    //     Debug.Assert(grid.All(x => x.Count == w));
    //     for (int r = 0; r < grid.Count; r++) {
    //         for (int c = 0; c < w; c++) {
    //             if (grid[r][c] != null) {
    //                 Debug.Assert(grid[r][c].CharAt(r, c) != ' ');
    //             }
    //         }
    //     }
    // }
}