using System;
using System.Collections.Generic;
using System.Text;

/**
  Represents a grid of T, indexed by x/y starting at the BOTTOM LEFT
 */

[System.Serializable]
public class Grid<T> {
    // maps x -> y -> T
    private Dictionary<int, Dictionary<int, T>> grid = new Dictionary<int, Dictionary<int, T>>();
    public readonly int width, height;

    // If supplier is not null, fills each value in the grid using the supplier
    public Grid(int width, int height, Func<T> supplier = null) {
        this.width = width;
        this.height = height;

        if (supplier != null) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Set(x, y, supplier());
                }
            }
        }
    }

    public bool WithinBounds(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void Set(int x, int y, T val) {
        if (!WithinBounds(x, y)) {
            throw new System.IndexOutOfRangeException();
        }
        if (!grid.ContainsKey(x)) {
            grid[x] = new Dictionary<int, T>();
        }
        grid[x][y] = val;
    }

    // If the element is not found, the notFoundSupplier will set the value
    public T Get(int x, int y, Func<T> notFoundSupplier = null) {
        if (grid.ContainsKey(x) && grid[x].ContainsKey(y)) {
            return grid[x][y];
        }
        if (notFoundSupplier != null) {
            Set(x, y, notFoundSupplier());
            return grid[x][y];
        }
        return default(T);
    }

    // NOT GUARANTEED ANY ORDER
    // Runs on each inserted val, not every x/y
    public void ForEach(Action<T> lambda) {
        foreach (var col in grid.Values) {
            foreach (var pt in col.Values) {
                lambda(pt);
            }
        }
    }

    // Takes a lambda with params <T, int x, int y>
    // NOT GUARANTEED ANY ORDER
    // Runs on each inserted val, not every x/y
    public void ForEach(Action<T, int, int> lambda) {
        foreach (int x in grid.Keys) {
            foreach (int y in grid[x].Keys) {
                lambda(grid[x][y], x, y);
            }
        }
    }

    public string ToString(Func<T, char> lambda) {
        StringBuilder sb = new StringBuilder();
        for (int y = height-1; y >= 0; y--) {
            for (int x = 0; x < width; x++) {
                sb.Append(lambda(Get(x, y)));
            }
            sb.Append('\n');
        }
        return sb.ToString().Trim('\n');
    }

    // Takes a lambda with type (T, int x, int y) => char
    public string ToString(Func<T, int, int, char> lambda) {
        StringBuilder sb = new StringBuilder();
        for (int y = height-1; y >= 0; y--) {
            for (int x = 0; x < width; x++) {
                sb.Append(lambda(Get(x, y), x, y));
            }
            sb.Append('\n');
        }
        return sb.ToString().Trim('\n');
    }

    public override string ToString() {
        return ToString(x => x == null ? ' ' : 'X');
    }

    public Grid<T> RotatedClockwise() {
        Grid<T> newGrid = new Grid<T>(height, width);
        ForEach((val, x, y) => newGrid.Set(y, width-x-1, val));
        return newGrid;
    }

    // returns an expanded grid with the given padding
    public Grid<T> Expanded(int top, int bottom, int left, int right) {
        Grid<T> newGrid = new Grid<T>(width + left + right, height + top + bottom);
        ForEach((val, x, y) => newGrid.Set(x + left, y + bottom, val));
        return newGrid;        
    }
}