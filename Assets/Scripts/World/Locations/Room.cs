using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using UnityEngine;
using World;

[System.Serializable]
public class Room {
    // top left corner
    private int xOffset, yOffset;

    public int width {
        get { return tiles.width; }
    }
    public int height {
        get { return tiles.height; }
    }

    private Grid<List<TileElement>> tiles;

    public Room(params string[] grid) {
        PlaceSquares(grid);
    }

    public List<TileElement> TileElementsAt(int x, int y) {
        return tiles.Get(x - xOffset, y - yOffset);
    }
    
    public char CharAt(int x, int y) {
        if (TileElementsAt(x, y) == null) {
            Debug.Log("yo wtf x=" + x + " y=" + y + " width=" + width + " height=" + height);
        }
        return TileElementsAt(x, y).Last().ch;
    }

    public bool Occupied(int x, int y) {
        return CharAt(x, y) != ' ';
    }

    public override string ToString() {
        return tiles.ToString(x => x.Last().ch);
    }

    // change offset in interior grid
    public void IncrementOffset(int xIncrease, int yIncrease) {
        xOffset += xIncrease;
        yOffset += yIncrease;
    }

    // TODO: move this to grid?
    public List<InteriorBuilder.FindResult> Find(string on) {
        List<InteriorBuilder.FindResult> result = new List<InteriorBuilder.FindResult>();
        for (int y = 0; y < tiles.height; y++) {
            for (int x = 0; x < tiles.width - on.Length + 1; x++) {
                // track all the hits in the row
                if (Matches(x, y, on)) {
                    bool spaceAbove = y == tiles.height-1 || EmptyX(x, y+1, on.Length);
                    bool spaceBelow = y == 0 || EmptyX(x, y-1, on.Length);
                    result.Add(new InteriorBuilder.FindResult(x, y, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    private bool EmptyX(int x, int y, int count) {
        return Enumerable.Range(x, count).All(xi => TileElementsAt(xi, y) == null || TileElementsAt(xi, y).All(tile => !tile.occupied));
    }

    private bool Matches(int x, int y, string match) {
        for (int i = 0; i < match.Length; i++) {
            if (TileElementsAt(x + i, y).Last().ch != match[i]) {
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
        tiles = tiles.RotatedClockwise();
        tiles.ForEach(stack => stack.ForEach(x => x.Rotate()));
    }

    private void PlaceSquares(string[] grid) {
        tiles = new Grid<List<TileElement>>(grid[0].Length, grid.Length);
        // separate empty from non-empty spaces with walls
        for (int x = 0; x < tiles.width; x++) {
            for (int y = 0; y < tiles.height; y++) {
                char ch = grid[tiles.height-y-1][x];
                TileElement sq = new FloorTile(ch);
                tiles.Get(x, y, () => new List<TileElement>()).Add(sq);
            }
        }

        tiles.ForEach((stack, x, y) => {
            FloorTile ft = stack.First() as FloorTile;
            ft.wallTop = y == tiles.height-1 || !Occupied(x, y+1);
            ft.wallBottom = y == 0 || !Occupied(x, y-1);
            ft.wallLeft = x == 0 || !Occupied(x-1, y);
            ft.wallRight = x == tiles.width-1 || !Occupied(x+1, y);
        });
    }
}