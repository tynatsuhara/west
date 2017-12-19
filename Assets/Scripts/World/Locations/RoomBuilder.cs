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
    public RoomBuilder Attach(string on, RoomBuilder room) {
        connections.Add(room, on);
        return this;
    }

    // Merges all the attached rooms together
    public RoomBuilder Finish() {
        foreach (RoomBuilder room in connections.Keys) {
            string connector = connections[room];
            TryAttachRoom(room, connector);
            // find place to attach (looking at all already placed rooms)
            // attach it (rotate if needed)
        }
        Debug.Log("finished! \n" + String.Join("\n", grid.ToArray()));
        return this;
    }

    private bool TryAttachRoom(RoomBuilder room, string on) {
        int initialRotation = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < initialRotation; i++) {
            room.Rotate();
        }

        for (int i = 0; i < 4; i++) {
            List<FindResult> thatDoors = room.Find(on);
            List<FindResult> thatSpaceAboveDoors = thatDoors.Where(x => x.spaceAbove).OrderBy(x => System.Guid.NewGuid()).ToList();
            List<FindResult> thatSpaceBelowDoors = thatDoors.Where(x => x.spaceBelow).OrderBy(x => System.Guid.NewGuid()).ToList();
            
            if (thatDoors.Count > 0) {
                List<FindResult> spaceAboveDoors = new List<FindResult>();
                List<FindResult> spaceBelowDoors = new List<FindResult>();
                for (int j = 0; j < 4 && spaceAboveDoors.Count == 0 && spaceBelowDoors.Count == 0; j++) {
                    List<FindResult> thisDoors = Find(on);
                    spaceAboveDoors = thisDoors.Where(x => x.spaceAbove).OrderBy(x => System.Guid.NewGuid()).ToList();
                    spaceBelowDoors = thisDoors.Where(x => x.spaceBelow).OrderBy(x => System.Guid.NewGuid()).ToList();

                    bool matchAbove = spaceAboveDoors.Count > 0 && thatSpaceBelowDoors.Count > 0;
                    bool matchBelow = spaceBelowDoors.Count > 0 && thatSpaceAboveDoors.Count > 0;
                    if ((matchAbove && !matchBelow) || (matchAbove && matchBelow && UnityEngine.Random.Range(0, 2) == 0)) {
                        // place room above
                        if (Merge(spaceAboveDoors.First(), thatSpaceBelowDoors.First(), room)) {
                            return true;
                        }
                    } else if (matchBelow) {
                        // place room below
                        if (Merge(spaceBelowDoors.First(), thatSpaceAboveDoors.First(), room)) {
                            return true;
                        }
                    }

                    Rotate();
                }
            }

            room.Rotate();
        }

        return false;
    }

    private bool Merge(FindResult thisPos, FindResult otherPos, RoomBuilder other) {
        int bottomPad = 0, topPad = 0, leftPad = 0, rightPad = 0;

        for (int r = 0; r < other.grid.Count; r++) {
            for (int c = 0; c < other.grid[0].Length; c++) {
                int overlapRow = thisPos.row + r - otherPos.row;
                bottomPad = Mathf.Min(bottomPad, overlapRow);
                topPad = Mathf.Max(topPad, overlapRow);
                bool outsideRow = overlapRow < 0 || overlapRow >= grid.Count;

                int overlapCol = thisPos.col + c - otherPos.col;
                leftPad = Mathf.Min(leftPad, overlapCol);
                rightPad = Mathf.Max(rightPad, overlapCol);
                bool outsideCol = overlapCol < 0 || overlapCol >= grid[0].Length;
                
                if (!outsideCol && !outsideRow && !CanOverlap(grid[overlapRow][overlapCol], other.grid[r][c])) {
                    return false;
                }
            }
        }

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

        List<char[]> arrs = grid.Select(x => x.ToCharArray()).ToList();
        for (int r = 0; r < other.grid.Count; r++) {
            for (int c = 0; c < other.grid[0].Length; c++) {
                int overlapRow = thisPos.row + r - otherPos.row;
                int overlapCol = thisPos.col + c - otherPos.col;
                char winner = other.grid[r][c] != ' ' ? other.grid[r][c] : arrs[overlapRow][overlapCol];
                arrs[overlapRow][overlapCol] = winner;
            }
        }

        return true;
    }

    private bool CanOverlap(char a, char b) {
        char wall = 'a';
        return (a == ' ' || a == wall) && (b == ' ' || b == wall);
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
            result.Add(String.Join("", grid.Select(str => "" + str[i]).ToArray()));
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