using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InteriorBuilder {

    private Dictionary<char, Room> rooms = new Dictionary<char, Room>();
    private List<string> grid;

    public InteriorBuilder(Room root) {
        rooms.Add(root.charKey, root);
        grid = Enumerable.Range(0, root.height)
                .Select(r => string.Join("", Enumerable.Range(0, root.width)
                        .Select(c => "" + (root.Occupied(r, c) ? root.charKey : ' '))
                        .ToArray()))
                .ToList();
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
        int initialRotation = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < initialRotation; i++) {
            room.Rotate();
        }

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
                }
            }

            room.Rotate();
        }

        return false;
    }

    public List<FindResult> Find(string on) {
        List<FindResult> result = new List<FindResult>();
        for (int r = 0; r < grid.Count; r++) {
            string row = grid[r];
            List<int> rowHits = new List<int>();
            for (int i = 0; i < row.Length - on.Length + 1; i++) {
                // track all the hits in the row
                if (RowStartsWith(r, on, i)) {
                    rowHits.Add(i);
                    bool spaceAbove = r == 0 || grid[r-1].Substring(i, on.Length).Trim().Length == 0;
                    bool spaceBelow = r == grid.Count-1 || grid[r+1].Substring(i, on.Length).Trim().Length == 0;
                    result.Add(new FindResult(r, i, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    private bool RowStartsWith(int rowIndex, string match, int startIndex) {
        string row = grid[rowIndex];
        for (int i = 0; i < match.Length; i++) {
            int col = startIndex + i;
            char roomKey = row[col];
            if (roomKey != ' ' && rooms[roomKey].CharAt(rowIndex, col) != match[i]) {
                return false;
            }
        }
        return true;
    } 

    // Merge and resize the grid (will always be a rectangle, no different-length rows)
    private bool Merge(FindResult thisPos, FindResult otherPos, Room other, string on, bool placeOtherAbove) {
        int minRow = 0, maxRow = 0, minCol = 0, maxCol = 0;
        int shift = placeOtherAbove ? -1 : 1;

        // make sure all overlapping is safe and account for padding
        for (int r = 0; r < other.height; r++) {
            for (int c = 0; c < other.width; c++) {
                int overlapRow = thisPos.row + r - otherPos.row + shift;
                bool outsideRow = overlapRow < 0 || overlapRow >= grid.Count;
                minRow = Mathf.Min(minRow, overlapRow);
                maxRow = Mathf.Max(maxRow, overlapRow);

                int overlapCol = thisPos.col + c - otherPos.col;
                bool outsideCol = overlapCol < 0 || overlapCol >= grid[0].Length;
                minCol = Mathf.Min(minCol, overlapCol);
                maxCol = Mathf.Max(maxCol, overlapCol);
                
                if (!outsideCol && !outsideRow && other.Occupied(r, c) && grid[overlapRow][overlapCol] != ' ') {
                    // Debug.Log("failed overlap, thisPos=" + thisPos + ", otherPos=" + otherPos + ", a=" + grid[overlapRow][overlapCol] + ", b=" + other.grid[r][c]);
                    return false;
                }
            }
        }

        int bottomPad = Mathf.Max(-minRow, 0);
        int topPad = Mathf.Max(maxRow - grid.Count + 1, 0);
        int leftPad = Mathf.Max(-minCol, 0);
        int rightPad = Mathf.Max(maxCol - grid[0].Length + 1, 0);
        // Debug.LogFormat("{0} {1} {2} {3}", bottomPad, topPad, leftPad, rightPad);

        // adjust the size of the grid
        thisPos.row += bottomPad;
        thisPos.col += leftPad;
        int originalLength = grid[0].Length;

        for (int i = 0; i < bottomPad; i++) {
            grid.Insert(0, "");
        }
        for (int i = 0; i < topPad; i++) {
            grid.Add("");
        }
        for (int i = 0; i < grid.Count; i++) {
            grid[i] = grid[i].PadLeft(originalLength + leftPad).PadRight(originalLength + leftPad + rightPad);
        }

        List<char[]> arrs = grid.Select(x => x.ToCharArray()).ToList();  // expanded grid
        int topLeftRow = thisPos.row - otherPos.row + shift;
        int topLeftCol = thisPos.col - otherPos.col;
        for (int r = 0; r < other.height; r++) {
            for (int c = 0; c < other.width; c++) {
                if (other.Occupied(r, c)) {
                    arrs[topLeftRow + r][topLeftCol + c] = other.charKey;
                }
            }
        }
        grid = arrs.Select(x => new string(x)).ToList();

        other.SetInteriorOffset(topLeftRow, topLeftCol);
        other.MakeDoorway(otherPos, on);

        return true;
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
            return "[" + row + ", " + col + "]";
        }
    }

    private void Rotate() {
        foreach (Room room in rooms.Values) {
            room.RotatePlaced(grid.Count);
        }
        List<string> result = new List<string>();
        for (int i = 0; i < grid[0].Length; i++) {
            StringBuilder sb = new StringBuilder(grid.Count);
            result.Add(string.Join("", grid.Select(str => "" + str[i]).Reverse().ToArray()));
        }
        grid = result;
    }

    public override string ToString() {
        return string.Join("\n", grid.ToArray());
    }
}