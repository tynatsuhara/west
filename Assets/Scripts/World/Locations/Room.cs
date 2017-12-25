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
    }

    public char CharAt(int row, int col) {
        return squares[row - this.row][col - this.col].ch;
    }

    public bool Occupied(int row, int col) {
        return squares[row - this.row][col - this.col].ch != ' ';
    }

    public override string ToString() {
        return string.Join("\n", squares.Select(x => string.Join("", x.Select(ch => "" + ch.ch).ToArray())).ToArray());
    }

    public void Place(int row, int col) {
        this.row = row;
        this.col = col;
    }

    public void MakeDoorway(InteriorBuilder.FindResult place, string door) {
        for (int i = 0; i < door.Length; i++) {
            squares[place.row][place.col].ch = floor;
        }
    }

    public void Rotate() {
        List<List<Square>> result = new List<List<Square>>();
        for (int i = 0; i < squares.First().Count; i++) {
            result.Add(squares.Select(row => row[i]).Reverse().ToList());
        }
        squares = result;
        squares.ForEach(x => x.ForEach(sq => sq.Rotate()));
    }

    public List<InteriorBuilder.FindResult> Find(string on) {
        List<InteriorBuilder.FindResult> result = new List<InteriorBuilder.FindResult>();
        for (int r = 0; r < squares.Count; r++) {
            List<Square> row = squares[r];
            List<int> rowHits = new List<int>();
            for (int i = 0; i < row.Count - on.Length + 1; i++) {
                // track all the hits in the row
                if (RowStartsWith(r, on, i)) {
                    rowHits.Add(i);
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
    }

    private void PlaceSquares(string[] grid) {
        squares = grid.Select(x => x.ToCharArray().Select(c => new Square(c)).ToList()).ToList();
        squares.First().ForEach(x => x.doorTop = true);
        squares.Last().ForEach(x => x.doorBottom = true);
        squares.ForEach(x => x.First().doorLeft = true);
        squares.ForEach(x => x.Last().doorRight = true);
    }

    [System.Serializable]
    private class Square {
        public char ch;
        public bool doorRight, doorBottom, doorLeft, doorTop;

        public Square(char ch) {
            this.ch = ch;
        }

        public void Rotate() {
            bool oldRight = doorRight;
            doorRight = doorTop;
            doorTop = doorLeft;
            doorLeft = doorBottom;
            doorBottom = oldRight;
        }
    }
}