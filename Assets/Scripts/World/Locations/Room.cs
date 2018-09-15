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
    public int rotation;  // [0, 3], rotates clockwise, based on the original orientation (0) in the data file
    public int width {
        get { return tiles.width; }
    }
    public int height {
        get { return tiles.height; }
    }

    // both grids should contain nonnull elements
    private Grid<List<TileElement>> tiles;
    private Grid<char> ascii;

    public Room(Grid<char> grid) {
        ascii = grid.ShallowCopy();
        PlaceSquares(grid);
    }

    public List<TileElement> TileElementsAt(int x, int y) {
        return tiles.Get(x - xOffset, y - yOffset);
    }
    
    public char CharAt(int x, int y) {
        if (TileElementsAt(x, y) == null) {
            Debug.Log("yo wtf x=" + x + " y=" + y + " width=" + width + " height=" + height);
        }
        return ascii.Get(x - xOffset, y - yOffset);
    }

    public bool Occupied(int x, int y) {
        return CharAt(x, y) != ' ';
    }

    public override string ToString() {
        return ascii.ToString(c => c);
    }

    // change offset in interior grid
    public void IncrementOffset(int xIncrease, int yIncrease) {
        xOffset += xIncrease;
        yOffset += yIncrease;
    }

    // TODO: move this to grid?
    public List<InteriorBuilder.FindResult> Find(Grid<char> g) {
        List<InteriorBuilder.FindResult> result = new List<InteriorBuilder.FindResult>();
        for (int y = 0; y < tiles.height - g.height + 1; y++) {
            for (int x = 0; x < tiles.width - g.width + 1; x++) {
                // track all the hits in the row
                if (MatchesAndNotOccupied(x, y, g)) {
                    bool spaceAbove = y == tiles.height-g.height || EmptyX(x, y+g.height, g.width);
                    bool spaceBelow = y == 0 || EmptyX(x, y-1, g.width);
                    result.Add(new InteriorBuilder.FindResult(this, x + xOffset, y + yOffset, spaceAbove, spaceBelow));
                }
            }
        }
        return result;
    }

    private bool EmptyX(int x, int y, int count) {
        return Enumerable.Range(x, count).All(xi => TileElementsAt(xi + xOffset, y + yOffset) == null 
                || TileElementsAt(xi + xOffset, y + yOffset).All(tile => !tile.occupied));
    }

    private bool MatchesAndNotOccupied(int x, int y, Grid<char> g) {
        for (int c = 0; c < g.width; c++) {
            for (int r = 0; r < g.height; r++) {
                TileElement floorTile  = tiles.Get(x + c, y + r).FirstOrDefault();
                if (ascii.Get(x + c, y + r) != g.Get(c, r) || (floorTile != null && floorTile.occupied)) {
                    return false;
                }
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
        ascii = ascii.RotatedClockwise();
        tiles = tiles.RotatedClockwise();
        tiles.ForEach(stack => stack.ForEach(x => x.Rotate()));
        rotation = (rotation + 1) % 4;
    }

    public void WriteASCII(Grid<char> g, int x, int y) {
        for (int x_ = 0; x_ < g.width; x_++) {
            for (int y_ = 0; y_ < g.height; y_++) {
                char c = g.Get(x_, y_);
                if (c != ' ') {
                    ascii.Set(x - xOffset + x_, y - yOffset + y_, c);
                }
            }
        }
    }

    private void PlaceSquares(Grid<char> grid) {
        tiles = new Grid<List<TileElement>>(grid.width, grid.height);
        // separate empty from non-empty spaces with walls
        for (int x = 0; x < tiles.width; x++) {
            for (int y = 0; y < tiles.height; y++) {
                char ch = grid.Get(x, y);
                TileElement sq = new FloorTile();
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