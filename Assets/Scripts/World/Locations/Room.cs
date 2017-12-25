using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Room {
    // top left corner
    private int row;
    private int col;

    public int width {
        get { return grid[0].Length; }
    }
    public int height {
        get { return grid.Count; }
    }

    public readonly char charKey;
    private List<string> grid = new List<string>();

    public Room(char charKey, params string[] grid) {
        this.charKey = charKey;
        this.grid.AddRange(grid);
    }

    public char CharAt(int row, int col) {
        return grid[row - this.row][col - this.col];
    }

    public bool Occupied(int row, int col) {
        return grid[row - this.row][col - this.col] != ' ';
    }

    public Room Replace(string str, string with) {
        for (int i = 0; i < grid.Count; i++) {
            grid[i] = grid[i].Replace(str, with);
        }
        return this;
    }

    public override string ToString() {
        return String.Join("\n", grid.ToArray());
    }

    public void Place(int row, int col) {
        this.row = row;
        this.col = col;
    }

    public void Rotate() {
        List<string> result = new List<string>();
        for (int i = 0; i < grid[0].Length; i++) {
            StringBuilder sb = new StringBuilder(grid.Count);
            result.Add(string.Join("", grid.Select(str => "" + str[i]).Reverse().ToArray()));
        }
        grid = result;
    }

    public List<InteriorBuilder.FindResult> Find(string on) {
        List<InteriorBuilder.FindResult> result = new List<InteriorBuilder.FindResult>();
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
                    result.Add(new InteriorBuilder.FindResult(r, index, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    public void RotatePlaced(int overallHeightBeforeRotation) {
        Rotate();

        int newRow = col;
        int newCol = overallHeightBeforeRotation - row - grid[0].Length;

        row = newRow;
        col = newCol;

        /*
        ++++++++++++
        ++*~~+++++++
        ++~~~+++++++
        ++~~~+++++++
        ++++++++++++
         */
    }
}