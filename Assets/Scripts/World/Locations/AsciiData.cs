using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class AsciiData {

    private Dictionary<string, Grid<char>> data = new Dictionary<string, Grid<char>>();

    public AsciiData() {
        TextAsset asset = Resources.Load("roomdata") as TextAsset;
        string[] lines = asset.text.Split('\n');

        /*  Assumes a well-formatted data file
            > indicates the start of a grid
            everything following the start is part of that grid (excess whitespace after the body is removed)
         */

        List<int> breaks = Enumerable.Range(0, lines.Length)
                                     .Where(i => lines[i].StartsWith(">"))
                                     .ToList();
        breaks.Add(lines.Length);

        for (int i = 0; i < breaks.Count - 1; i++) {
            string title = lines[breaks[i]].Remove(0, 1).Trim();
            List<string> body = lines.Take(breaks[i+1])
                                     .Skip(breaks[i] + 1)
                                     .Select(x => x.TrimEnd())
                                     .ToList();
            while (body.Last().Trim().Length == 0) {
                body.RemoveAt(body.Count - 1);
            }
            int width = body.Select(x => x.Length).Max();
            int height = body.Count;
            Grid<char> g = new Grid<char>(width, height, () => ' ');
            for (int y = 0; y < height; y++) {
                string row = body[body.Count-y-1];
                for (int x = 0; x < row.Length; x++) {
                    g.Set(x, y, row[x]);
                }
            }
            data.Add(title, g);
        }
    }

    public Grid<char> Get(string title) {
        return data[title];
    }
}