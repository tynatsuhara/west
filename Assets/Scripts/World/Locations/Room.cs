using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Room {
    // top left corner
    private int xOffset, yOffset;

    public int width {
        get { return squares.width; }
    }
    public int height {
        get { return squares.height; }
    }

    public readonly char charKey;
    public readonly char floor;
    private Grid<Square> squares;

    public Room(char charKey, char floor, params string[] grid) {
        this.charKey = charKey;
        this.floor = floor;
        PlaceSquares(grid);
    }

    public Square SquareAt(int x, int y) {
        return squares.Get(x - xOffset, y - yOffset);
    }
    
    public char CharAt(int x, int y) {
        if (SquareAt(x, y) == null) {
            Debug.Log("yo wtf x=" + x + " y=" + y + " width=" + width + " height=" + height);
        }
        return SquareAt(x, y).ch;
    }

    public bool Occupied(int x, int y) {
        return CharAt(x, y) != ' ';
    }

    public override string ToString() {
        return squares.ToString(x => x.ch);
    }

    // change offset in interior grid
    public void IncrementOffset(int xIncrease, int yIncrease) {
        xOffset += xIncrease;
        yOffset += yIncrease;
    }

    // TODO: move this to grid?
    public List<InteriorBuilder.FindResult> Find(string on) {
        List<InteriorBuilder.FindResult> result = new List<InteriorBuilder.FindResult>();
        for (int y = 0; y < squares.height; y++) {
            for (int x = 0; x < squares.width - on.Length + 1; x++) {
                // track all the hits in the row
                if (Matches(x, y, on)) {
                    bool spaceAbove = y == squares.height-1 || EmptyX(x, y+1, on.Length);
                    bool spaceBelow = y == 0 || EmptyX(x, y-1, on.Length);
                    result.Add(new InteriorBuilder.FindResult(x, y, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    private bool EmptyX(int x, int y, int count) {
        return Enumerable.Range(x, count).All(xi => SquareAt(xi, y).ch == ' ');
    }

    private bool Matches(int x, int y, string match) {
        for (int i = 0; i < match.Length; i++) {
            if (SquareAt(x + i, y).ch != match[i]) {
                return false;
            }
        }
        return true;
    } 

    public void RotatePlaced(int overallWidthBeforeRotation) {
        int newX = yOffset;
        int newY = overallWidthBeforeRotation - xOffset - width;
        Rotate();
        xOffset = newX;
        yOffset = newY;
    }

    // rotate clockwise
    public void Rotate() {
        squares = squares.RotatedClockwise();
        squares.ForEach(sq => sq.Rotate());
    }

    private void PlaceSquares(string[] grid) {
        squares = new Grid<Square>(grid[0].Length, grid.Length);
        // separate empty from non-empty spaces with walls
        for (int x = 0; x < squares.width; x++) {
            for (int y = 0; y < squares.height; y++) {
                char ch = grid[squares.height-y-1][x];
                Square sq = new Square(ch);
                squares.Set(x, y, sq);
            }
        }

        squares.ForEach((sq, x, y) => {
            sq.wallTop = y == squares.height-1 || !Occupied(x, y+1);
            sq.wallBottom = y == 0 || !Occupied(x, y-1);
            sq.wallLeft = x == 0 || !Occupied(x-1, y);
            sq.wallRight = x == squares.width-1 || !Occupied(x+1, y);
        });
    }


    [System.Serializable]
    public class Square {
        public char ch;
        public bool wallRight, wallBottom, wallLeft, wallTop;

        public Square(char ch) {
            this.ch = ch;
        }

        // rotate clockwise
        public void Rotate() {
            bool oldRight = wallRight;
            wallRight = wallTop;
            wallTop = wallLeft;
            wallLeft = wallBottom;
            wallBottom = oldRight;
        }

        public void RemoveWalls(char newFloor) {
            ch = newFloor;
            wallRight = wallLeft = wallBottom = wallTop = false;
        }
    }
}