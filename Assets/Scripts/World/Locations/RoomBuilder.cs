using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomBuilder {
    private List<string> grid = new List<string>();
    private int[] removableRows;
    private int[] removableCols;
    private Dictionary<RoomBuilder, string> connections = new Dictionary<RoomBuilder, string>();
    private Dictionary<char, Action<Vector3>> functions = new Dictionary<char, Action<Vector3>>();

    public RoomBuilder(params string[] grid) {
        this.grid.AddRange(grid);
    }

    public RoomBuilder SetRemovableRows(params int[] rows) {
        removableRows = rows;
        return this;
    }

    public RoomBuilder SetRemovableColumns(params int[] columns) {
        removableCols = columns;
        return this;
    }

    // rooms should be attached in order of importance (building out from the root room)
    // todo: additional parameters? (optional, etc)
    public RoomBuilder Attach(string on, string replacement, RoomBuilder room) {
        connections.Add(room, on);
        return this;
    }

    public RoomBuilder Replace(string str, string with) {
        for (int i = 0; i < grid.Count; i++) {
            grid[i] = grid[i].Replace(str, with);
        }
        return this;
    }

    // Merges all the attached rooms together
    public RoomBuilder Merge() {
        foreach (RoomBuilder room in connections.Keys) {
            string connector = connections[room];
            TryAttachRoom(room, connector);
        }
        Debug.Log("finished! \n" + ToString());
        return this;
    }

    public override string ToString() {
        return String.Join("\n", grid.ToArray());
    }

    private bool TryAttachRoom(RoomBuilder room, string on) {
        int initialRotation = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < initialRotation; i++) {
            room.Rotate();
        }

        for (int i = 0; i < 2; i++) {
            // get all "on" strings in the other grid with space above/below them
            List<FindResult> thatDoors = room.Find(on);
            Debug.Log("thatDoors = " + String.Join(" + ", thatDoors.Select(x => x.ToString()).ToArray()));
            List<FindResult> thatSpaceAboveDoors = thatDoors.Where(x => x.spaceAbove).OrderBy(x => System.Guid.NewGuid()).ToList();
            List<FindResult> thatSpaceBelowDoors = thatDoors.Where(x => x.spaceBelow).OrderBy(x => System.Guid.NewGuid()).ToList();
            
            if (thatSpaceAboveDoors.Count + thatSpaceBelowDoors.Count > 0) {
                for (int j = 0; j < 4; j++) {
                    List<FindResult> thisDoors = Find(on);
                    List<FindResult> spaceAboveDoors = thisDoors.Where(x => x.spaceAbove).OrderBy(x => System.Guid.NewGuid()).ToList();
                    List<FindResult> spaceBelowDoors = thisDoors.Where(x => x.spaceBelow).OrderBy(x => System.Guid.NewGuid()).ToList();
                    Debug.Log("thisDoors = " + String.Join(" + ", thisDoors.Select(x => x.ToString()).ToArray()));

                    List<FindResult>[] sides;
                    if (UnityEngine.Random.Range(0, 2) == 0) {
                        sides = new List<FindResult>[]{spaceAboveDoors, thatSpaceBelowDoors, spaceBelowDoors, thatSpaceAboveDoors};
                    } else {
                        sides = new List<FindResult>[]{spaceBelowDoors, thatSpaceAboveDoors, spaceAboveDoors, thatSpaceBelowDoors};
                    }
                    
                    for (int f = 0; f < 3; f += 2) {
                        foreach (FindResult f1 in sides[f]) {
                            foreach (FindResult f2 in sides[f+1]) {
                                if (Merge(f1, f2, room)) {
                                    return true;
                                }
                            }
                        }
                    }

                    Rotate();
                }
            }

            room.Rotate();
        }

        return false;
    }

    // Merge and resize the grid (will always be a rectangle, no different-length rows)
    private bool Merge(FindResult thisPos, FindResult otherPos, RoomBuilder other) {
        int minRow = 0, maxRow = 0, minCol = 0, maxCol = 0;

        // make sure all overlapping is safe and account for padding
        for (int r = 0; r < other.grid.Count; r++) {
            for (int c = 0; c < other.grid[0].Length; c++) {
                int overlapRow = thisPos.row + r - otherPos.row;
                bool outsideRow = overlapRow < 0 || overlapRow >= grid.Count;
                minRow = Mathf.Min(minRow, overlapRow);
                maxRow = Mathf.Max(maxRow, overlapRow);

                int overlapCol = thisPos.col + c - otherPos.col;
                bool outsideCol = overlapCol < 0 || overlapCol >= grid[0].Length;
                minCol = Mathf.Min(minCol, overlapCol);
                maxCol = Mathf.Max(maxCol, overlapCol);
                
                if (!outsideCol && !outsideRow && Overlap(grid[overlapRow][overlapCol], other.grid[r][c]) == null) {
                    Debug.Log("failed overlap, thisPos=" + thisPos + ", otherPos=" + otherPos + ", a=" + grid[overlapRow][overlapCol] + ", b=" + other.grid[r][c]);
                    return false;
                }
            }
        }

        int bottomPad = Mathf.Max(-minRow, 0);
        int topPad = Mathf.Max(maxRow - grid.Count + 1, 0);
        int leftPad = Mathf.Max(-minCol, 0);
        int rightPad = Mathf.Max(maxCol - grid[0].Length + 1, 0);
        Debug.LogFormat("{0} {1} {2} {3}", bottomPad, topPad, leftPad, rightPad);

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
        for (int r = 0; r < other.grid.Count; r++) {
            for (int c = 0; c < other.grid[0].Length; c++) {
                int overlapRow = thisPos.row + r - otherPos.row;
                int overlapCol = thisPos.col + c - otherPos.col;
                char winner = (char) Overlap(other.grid[r][c], arrs[overlapRow][overlapCol]);
                Debug.Log("overlapRow=" + overlapRow + ", overlapCol=" + overlapCol);
                arrs[overlapRow][overlapCol] = winner;
            }
        }
        grid = arrs.Select(x => new string(x)).ToList();

        return true;
    }

    private char? Overlap(char a, char b) {
        char wall = '=';
        if (a == ' ')
            return b;
        if (b == ' ')
            return a;
        if (a == b)
            return a;
        if (a == wall)
            return a;
        if (b == wall)
            return b;
        return null;
    }

    // returns [row, col] or null if not found
    private List<FindResult> Find(string on) {
        List<FindResult> result = new List<FindResult>();
        for (int r = 0; r < grid.Count; r++) {
            string row = grid[r];
            List<int> rowHits = new List<int>();
            for (int i = 0; i < row.Length - on.Length + 1; i++) {
                int index = row.IndexOf(on, i);
                // track all the hits in the row
                if (index != -1 && (rowHits.Count == 0 || rowHits[rowHits.Count - 1] != index)) {
                    rowHits.Add(index);
                    bool spaceAbove = r == 0 || grid[r-1].Substring(index, on.Length).Trim().Length == 0;
                    bool spaceBelow = r == grid.Count-1 || grid[r+1].Substring(index, on.Length).Trim().Length == 0;
                    result.Add(new FindResult(r, index, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    private class FindResult {
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

    public void Rotate() {
        grid = RotateGrid(grid);
    }

    // rotates 90 degrees clockwise
    // MUST BE A SQUARE GRID
    private List<string> RotateGrid(List<string> grid) {
        List<string> result = new List<string>();
        for (int i = 0; i < grid[0].Length; i++) {
            StringBuilder sb = new StringBuilder(grid.Count);
            result.Add(String.Join("", grid.Select(str => "" + str[i]).Reverse().ToArray()));
        }
        return result;
    }

    // TODO: how do I want to map to a grid of objects? string constants? needs to be serializable, actions aren't
    public void Map(char c, Action<Vector3> func) {
        functions[c] = func;
    }

    public void MapToTeleporter(char c) {

    }
}