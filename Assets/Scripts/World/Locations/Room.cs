using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Room {
    // top left corner
    private int row = 0;
    private int col = 0;

    public int width {
        get { return squares.First().Count; }
    }
    public int height {
        get { return squares.Count; }
    }

    public readonly char charKey;
    public readonly char floor;
    private List<List<Square>> squares;

    public Room(char charKey, char floor, params string[] grid) {
        this.charKey = charKey;
        this.floor = floor;
        PlaceSquares(grid);
        CheckRep();
    }

    public char CharAt(int row, int col) {
        try {
            return squares[row - this.row][col - this.col].ch;
        } catch (System.ArgumentOutOfRangeException e) {
            Debug.Log("room " + charKey + ", row = " + row + ", col = " + col);
            Debug.Log("row offset = " + this.row + ", col offset = " + this.col);
            Debug.Log("height = " + height + ", width = " + width);
            Debug.Log(e.StackTrace);
            throw e;
        }
    }

    public bool Occupied(int row, int col) {
        return squares[row - this.row][col - this.col].ch != ' ';
    }

    public override string ToString() {
        return string.Join("\n", squares.Select(x => string.Join("", x.Select(ch => "" + ch.ch).ToArray())).ToArray());
    }

    public void IncrementOffset(int rowIncrease, int colIncrease) {
        row += rowIncrease;
        col += colIncrease;
    }

    public void MakeDoorway(InteriorBuilder.FindResult place, string door) {
        for (int i = 0; i < door.Length; i++) {
            squares[place.row][place.col + i].ch = floor;
            squares[place.row][place.col + i].RemoveWalls();
        }
    }

    public List<InteriorBuilder.FindResult> Find(string on) {
        List<InteriorBuilder.FindResult> result = new List<InteriorBuilder.FindResult>();
        for (int r = 0; r < squares.Count; r++) {
            List<Square> row = squares[r];
            for (int i = 0; i < row.Count - on.Length + 1; i++) {
                // track all the hits in the row
                if (RowStartsWith(r, on, i)) {
                    bool spaceAbove = r == 0 || EmptyX(r-1, i, on.Length);
                    bool spaceBelow = r == squares.Count-1 || EmptyX(r+1, i, on.Length);
                    result.Add(new InteriorBuilder.FindResult(r, i, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    private bool EmptyX(int row, int col, int x) {
        return squares[row].Skip(col).Take(x).All(sq => sq.ch == ' ');
    }

    private bool RowStartsWith(int rowIndex, string match, int startIndex) {
        List<Square> row = squares[rowIndex];
        for (int i = 0; i < match.Length; i++) {
            if (row[startIndex + i].ch != match[i]) {
                return false;
            }
        }
        return true;
    } 

    public void RotatePlaced(int overallHeightBeforeRotation) {
        Rotate();
        int newRow = col;
        int newCol = overallHeightBeforeRotation - row - width;
        row = newRow;
        col = newCol;
        CheckRep();
    }

    // rotate clockwise
    public void Rotate() {
        CheckRep();
        List<List<Square>> result = new List<List<Square>>();
        for (int i = 0; i < squares.First().Count; i++) {
            result.Add(squares.Select(row => row[i]).Reverse().ToList());
        }
        squares = result;
        squares.ForEach(x => x.ForEach(sq => sq.Rotate()));
        CheckRep();
    }

    private void PlaceSquares(string[] grid) {
        squares = grid.Select(x => x.ToCharArray().Select(c => new Square(c)).ToList()).ToList();
        // separate empty from non-empty spaces with walls
        for (int r = 0; r < squares.Count; r++) {
            for (int c = 0; c < squares[r].Count; c++) {
                if (!Occupied(r, c)) {
                    continue;
                }
                squares[r][c].doorTop = r == 0 || !Occupied(r-1, c);
                squares[r][c].doorBottom = r == squares.Count-1 || !Occupied(r+1, c);
                squares[r][c].doorLeft = c == 0 || !Occupied(r, c-1);
                squares[r][c].doorRight = c == squares[r].Count-1 || !Occupied(r, c+1);
            }
        }
    }

    private void CheckRep() {
        int w = squares.First().Count;
        Debug.Assert(squares.All(x => x.Count == w));
    }

    [System.Serializable]
    private class Square {
        public char ch;
        public bool doorRight, doorBottom, doorLeft, doorTop;

        public Square(char ch) {
            this.ch = ch;
        }

        // rotate clockwise
        public void Rotate() {
            bool oldRight = doorRight;
            doorRight = doorTop;
            doorTop = doorLeft;
            doorLeft = doorBottom;
            doorBottom = oldRight;
        }

        public void RemoveWalls() {
            doorRight = doorLeft = doorBottom = doorTop = false;
        }
    }
}