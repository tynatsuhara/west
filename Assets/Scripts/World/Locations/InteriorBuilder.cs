using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InteriorBuilder {

    private Dictionary<char, Room> rooms = new Dictionary<char, Room>();
    private List<List<Room>> grid;

    public InteriorBuilder(Room root) {
        rooms.Add(root.charKey, root);
        grid = Enumerable.Range(0, root.height)
                .Select(r => Enumerable.Range(0, root.width).Select(c => (root.Occupied(r, c) ? root : null)).ToList())
                .ToList();
        Debug.Log(this);
    }

    // rooms should be attached in order of importance (building out from the root room)
    // todo: additional parameters? (optional, etc)
    public InteriorBuilder Attach(string on, Room room) {
        TryAttachRoom(room, on);
        return this;
    }

    public InteriorLocation Build(Map parent, System.Guid outside) {
        return new InteriorLocation(parent, outside, grid);
    }

    private bool TryAttachRoom(Room room, string on) {
        CheckRep();
        
        int initialRotation = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < initialRotation; i++) {
            room.Rotate();
        }
        Debug.Log("rotating other " + initialRotation + " times");

        for (int i = 0; i < 2; i++) {
            // get all "on" strings in the other grid with space above/below them
            List<FindResult> thatDoors = room.Find(on);
            // Debug.Log("thatDoors = " + String.Join(" + ", thatDoors.Select(x => x.ToString()).ToArray()));
            List<FindResult> thatSpaceAboveDoors = thatDoors.Where(x => x.spaceAbove).OrderBy(x => System.Guid.NewGuid()).ToList();
            List<FindResult> thatSpaceBelowDoors = thatDoors.Where(x => x.spaceBelow).OrderBy(x => System.Guid.NewGuid()).ToList();
            
            if (thatSpaceAboveDoors.Count + thatSpaceBelowDoors.Count > 0) {
                for (int j = 0; j < 4; j++) {
                    List<FindResult> thisDoors = Find(on);
                    List<FindResult> spaceAboveDoors = thisDoors.Where(x => x.spaceAbove).OrderBy(x => System.Guid.NewGuid()).ToList();
                    List<FindResult> spaceBelowDoors = thisDoors.Where(x => x.spaceBelow).OrderBy(x => System.Guid.NewGuid()).ToList();
                    // Debug.Log("thisDoors = " + String.Join(" + ", thisDoors.Select(x => x.ToString()).ToArray()));

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
                    Debug.Log("rotating this");
                }
            }

            room.Rotate();
        }

        return false;
    }

    public List<FindResult> Find(string on) {
        List<FindResult> result = new List<FindResult>();
        for (int r = 0; r < grid.Count; r++) {
            for (int i = 0; i < grid[r].Count - on.Length + 1; i++) {
                // track all the hits in the row
                if (RowStartsWith(r, on, i)) {
                    bool spaceAbove = r == 0 || EmptyX(r-1, i, on.Length);
                    bool spaceBelow = r == grid.Count-1 || EmptyX(r+1, i, on.Length);
                    result.Add(new FindResult(r, i, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    private bool EmptyX(int row, int col, int x) {
        return grid[row].Skip(col).Take(x).All(r => r == null);
    }

    private bool RowStartsWith(int rowIndex, string match, int startIndex) {
        List<Room> row = grid[rowIndex];
        for (int i = 0; i < match.Length; i++) {
            int col = startIndex + i;
            if ((row[col] == null && match[i] != ' ' ) || (row[col] != null && row[col].CharAt(rowIndex, col) != match[i])) {
                return false;
            }
        }
        return true;
    } 

    // Merge and resize the grid (will always be a rectangle, no different-length rows)
    private bool Merge(FindResult thisPos, FindResult otherPos, Room other, string on, bool placeOtherAbove) {
        CheckRep();

        int minRow = 0, maxRow = 0, minCol = 0, maxCol = 0;
        int shift = placeOtherAbove ? -1 : 1;

        // the coordinates in this grid for placing other
        int topLeftRow = thisPos.row - otherPos.row + shift;
        int topLeftCol = thisPos.col - otherPos.col;

        // make sure all overlapping is safe and account for padding
        for (int r = 0; r < other.height; r++) {
            for (int c = 0; c < other.width; c++) {
                int overlapRow = topLeftRow + r;
                bool outsideRow = overlapRow < 0 || overlapRow >= grid.Count;
                minRow = Mathf.Min(minRow, overlapRow);
                maxRow = Mathf.Max(maxRow, overlapRow);

                int overlapCol = topLeftCol + c;
                bool outsideCol = overlapCol < 0 || overlapCol >= grid.First().Count;
                minCol = Mathf.Min(minCol, overlapCol);
                maxCol = Mathf.Max(maxCol, overlapCol);
                
                if (!outsideCol && !outsideRow && other.Occupied(r, c) && grid[overlapRow][overlapCol] != null) {
                    // Debug.Log("failed overlap, thisPos=" + thisPos + ", otherPos=" + otherPos + ", a=" + grid[overlapRow][overlapCol] + ", b=" + other.grid[r][c]);
                    return false;
                }
            }
        }

        Debug.Log(this);
        Debug.Log(thisPos);
        Debug.Log(other);
        Debug.Log(otherPos);

        int bottomPad = Mathf.Max(-minRow, 0);
        int topPad = Mathf.Max(maxRow - grid.Count + 1, 0);
        int leftPad = Mathf.Max(-minCol, 0);
        int rightPad = Mathf.Max(maxCol - grid.First().Count + 1, 0);

        // adjust the size of the grid
        topLeftRow += bottomPad;
        topLeftCol += leftPad;
        int originalLength = grid.First().Count;

        Debug.LogFormat("padding = [b:{0}, t:{1}, l:{2}, r:{3}], old l = {4}, new l = {5}, old w = {6}, new w = {7}", bottomPad, topPad, leftPad, rightPad, grid.Count, grid.Count + bottomPad + topPad, grid.First().Count, originalLength + leftPad + rightPad);

        for (int i = 0; i < bottomPad; i++) {
            grid.Insert(0, new List<Room>());
        }
        for (int i = 0; i < topPad; i++) {
            grid.Add(new List<Room>());
        }
        for (int i = 0; i < grid.Count; i++) {
            grid[i].InsertRange(0, Enumerable.Repeat((Room) null, originalLength + leftPad - grid[i].Count));
            grid[i].AddRange(Enumerable.Repeat((Room) null, originalLength + leftPad + rightPad - grid[i].Count));
        }
        foreach (Room movedRoom in rooms.Values) {
            movedRoom.IncrementOffset(bottomPad, leftPad);
        }

        CheckRep();

        // place references in grid for occupied spaces
        for (int r = 0; r < other.height; r++) {
            for (int c = 0; c < other.width; c++) {
                if (other.Occupied(r, c)) {
                    grid[topLeftRow + r][topLeftCol + c] = other;
                }
            }
        }

        other.IncrementOffset(topLeftRow, topLeftCol);
        other.MakeDoorway(otherPos, on);
        rooms.Add(other.charKey, other);

        // TODO: put doorway on connecting room
        CheckRep();

        return true;
    }

    // rotate clockwise
    private void Rotate() {
        foreach (Room room in rooms.Values) {
            room.RotatePlaced(grid.Count);
        }
        List<List<Room>> result = new List<List<Room>>();
        for (int i = 0; i < grid.First().Count; i++) {
            result.Add(grid.Select(x => x[i]).Reverse().ToList());
        }
        grid = result;
        CheckRep();
    }

    public override string ToString() {
        string result = "";
        for (int r = 0; r < grid.Count; r++) {
            for (int c = 0; c < grid[r].Count; c++) {
                result += grid[r][c] != null ? grid[r][c].CharAt(r, c) : ' ';
            }
            result += '\n';
        }
        return result.Substring(0, result.Length - 1);
    }

    public class FindResult {
        public int row, col;
        public bool spaceAbove, spaceBelow;

        public FindResult(int row, int col, bool spaceAbove, bool spaceBelow) {
            this.row = row;
            this.col = col;
            this.spaceAbove = spaceAbove;
            this.spaceBelow = spaceBelow;
        }

        public override string ToString() {
            return "[" + string.Join(", ", new string[]{row.ToString(), col.ToString(), spaceAbove.ToString(), spaceBelow.ToString()}) + "]";
        }
    }

    private void CheckRep() {
        int w = grid.First().Count;
        Debug.Assert(grid.All(x => x.Count == w));
        for (int r = 0; r < grid.Count; r++) {
            for (int c = 0; c < w; c++) {
                if (grid[r][c] != null) {
                    Debug.Assert(grid[r][c].CharAt(r, c) != ' ');
                }
            }
        }
    }
}