using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InteriorBuilder {

    private Dictionary<char, Room> rooms = new Dictionary<char, Room>();
    private Grid<Room> grid;

    public InteriorBuilder(Room root) {
        rooms.Add(root.charKey, root);
        grid = new Grid<Room>(root.width, root.height);
        for (int x = 0; x < grid.width; x++) {
            for (int y = 0; y < grid.height; y++) {
                grid.Set(x, y, root.Occupied(x, y) ? root : null);
            }
        }
    }

    // rooms should be attached in order of importance (building out from the root room)
    // todo: additional parameters? (optional, etc)
    public InteriorBuilder Attach(string on, Room room) {
        if (!TryAttachRoom(room, on)) {
            Debug.Log("failed to attach room");
        }
        return this;
    }

    public InteriorLocation Build(Map parent, System.Guid outside, string name) {
        return new InteriorLocation(parent, outside, grid, name);
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

    private bool TryAttachRoom(Room room, string on) {
        int initialRotation = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < initialRotation; i++) {
            room.Rotate();
        }

        for (int i = 0; i < 2; i++) {
            // get all "on" strings in the other grid with space above/below them
            List<FindResult> thatDoors = room.Find(on);
            List<FindResult> thatSpaceAboveDoors = thatDoors.Where(x => x.spaceAbove).OrderBy(x => System.Guid.NewGuid()).ToList();
            List<FindResult> thatSpaceBelowDoors = thatDoors.Where(x => x.spaceBelow).OrderBy(x => System.Guid.NewGuid()).ToList();
            
            if (thatSpaceAboveDoors.Count + thatSpaceBelowDoors.Count > 0) {
                for (int j = 0; j < 4; j++) {
                    List<FindResult> thisDoors = Find(on);
                    List<FindResult> spaceAboveDoors = thisDoors.Where(x => x.spaceAbove).OrderBy(x => System.Guid.NewGuid()).ToList();
                    List<FindResult> spaceBelowDoors = thisDoors.Where(x => x.spaceBelow).OrderBy(x => System.Guid.NewGuid()).ToList();

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
                                if (Merge(f1, f2, room, on, checkAbove)) {
                                    return true;
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

        return false;
    }

    public List<FindResult> Find(string on) {
        List<FindResult> result = new List<FindResult>();
        for (int x = 0; x < grid.width; x++) {
            for (int y = 0; y < grid.height; y++) {
                if (Matches(x, y, on)) {
                    bool spaceAbove = y == grid.height-1 || EmptyX(x, y+1, on.Length);
                    bool spaceBelow = y == 0 || EmptyX(x, y-1, on.Length);
                    result.Add(new FindResult(x, y, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    private bool EmptyX(int x, int y, int count) {
        return Enumerable.Range(x, count).All(xi => grid.Get(xi, y) == null);
    }

    private bool Matches(int x, int y, string match) {
        for (int i = 0; i < match.Length; i++) {
            Room rm = grid.Get(x + i, y);
            if ((rm == null && match[i] != ' ') || (rm != null && rm.CharAt(x + i, y) != match[i])) {
                return false;
            }
        }
        return true;
    } 

    // Merge and resize the grid (will always be a rectangle, no different-length rows)
    private bool Merge(FindResult thisPos, FindResult otherPos, Room other, string on, bool placeOtherAbove) {
        int minX = 0, maxX = 0, minY = 0, maxY = 0;
        int shift = placeOtherAbove ? 1 : -1;

        // the coordinates in this grid for placing other
        int bottomLeftX = thisPos.x - otherPos.x;
        int bottomLeftY = thisPos.y - otherPos.y + shift;

        // make sure all overlapping is safe and account for padding
        for (int y = 0; y < other.height; y++) {
            for (int x = 0; x < other.width; x++) {
                int overlapX = bottomLeftX + x;
                bool outsideX = overlapX < 0 || overlapX >= grid.width;
                minX = Mathf.Min(minX, overlapX);
                maxX = Mathf.Max(maxX, overlapX);

                int overlapY = bottomLeftY + y;
                bool outsideY = overlapY < 0 || overlapY >= grid.height;
                minY = Mathf.Min(minY, overlapY);
                maxY = Mathf.Max(maxY, overlapY);
                
                if (!outsideX && !outsideY && other.Occupied(x, y) && grid.Get(overlapX, overlapY) != null) {
                    return false;
                }
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

        foreach (Room movedRoom in rooms.Values) {
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

        MakeDoorway(bottomLeftX, bottomLeftY, on);
        MakeDoorway(bottomLeftX, bottomLeftY - shift, on);

        rooms.Add(other.charKey, other);

        return true;
    }

    public void MakeDoorway(int x, int y, string door) {
        for (int i = 0; i < door.Length; i++) {
            Room rm = grid.Get(x + i, y);
            if (rm != null) {
                rm.SquareAt(x + i, y).RemoveWalls(rm.floor);
            }
        }
    }

    // rotate clockwise
    private void Rotate() {
        foreach (Room room in rooms.Values) {
            room.RotatePlaced(grid.width);
        }
        grid = grid.RotatedClockwise();
    }

    public override string ToString() {
        return grid.ToString((room, x, y) => room == null ? ' ' : room.CharAt(x, y));
    }

    public class FindResult {
        public int x, y;
        public bool spaceAbove, spaceBelow;

        public FindResult(int x, int y, bool spaceAbove, bool spaceBelow) {
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