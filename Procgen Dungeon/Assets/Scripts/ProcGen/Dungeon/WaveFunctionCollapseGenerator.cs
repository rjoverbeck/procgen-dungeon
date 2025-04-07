using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapseGenerator
{
    public static Queue<Vector2Int> Generate(List<Texture2D> tiles, List<int> tileWeights, int outputWidth, int outputHeight)
    {
        Color[][][] tilesEdgesColors = GetTilesEdgesColors(tiles);

        int[][][] tilesEdgesViableTiles = GetTilesEdgesViableTiles(tilesEdgesColors);

        // temporary logging
        for (int i = 0; i < tilesEdgesViableTiles.Length; i++)
        {
            string logMsgTile0 = $"Tile {i}:" +
                                 $"\nBottom: {string.Join(", ", tilesEdgesViableTiles[i][0])}";
            string logMsgTile1 = $"Tile {i}:" +
                                 $"\nTop: {string.Join(", ", tilesEdgesViableTiles[i][1])}";
            string logMsgTile2 = $"Tile {i}:" +
                                 $"\nLeft: {string.Join(", ", tilesEdgesViableTiles[i][2])}";
            string logMsgTile3 = $"Tile {i}:" +
                                 $"\nRight: {string.Join(", ", tilesEdgesViableTiles[i][3])}";
            Debug.Log(logMsgTile0);
            Debug.Log(logMsgTile1);
            Debug.Log(logMsgTile2);
            Debug.Log(logMsgTile3);
        }

        Dictionary<Vector2Int, int[]> outputGrid = InitializeOutputGrid(outputWidth, outputHeight, tiles.Count);

        // temporary logging
        foreach(var item in outputGrid)
        {
            string logMsg = $"Cell {item.Key} - viable tiles: {string.Join(", ", item.Value)}";
            Debug.Log(logMsg);
        }

        return null;
    }

    // Get the color of every tile's edge pixels
    private static Color[][][] GetTilesEdgesColors(List<Texture2D> tiles)
    {
        Color[][][] tilesEdgesColors = new Color[tiles.Count][][];

        for (int i = 0; i < tiles.Count; i++)
        {
            Texture2D tile = tiles[i];
            int width = tile.width;
            int height = tile.height;

            Color[] bottom = new Color[width];
            Color[] top = new Color[width];
            Color[] left = new Color[height];
            Color[] right = new Color[height];

            for (int x = 0; x < width; x++)
            {
                bottom[x] = tile.GetPixel(x, 0);
                top[x] = tile.GetPixel(x, height - 1);
            }

            for (int y = 0; y < height; y++)
            {
                left[y] = tile.GetPixel(0, y);
                right[y] = tile.GetPixel(width - 1, y);
            }

            tilesEdgesColors[i] = new Color[4][];
            tilesEdgesColors[i][0] = bottom;
            tilesEdgesColors[i][1] = top;
            tilesEdgesColors[i][2] = left;
            tilesEdgesColors[i][3] = right;
        }

        return tilesEdgesColors;
    }

    // Get the index of every viable tile (tiles whose complementary edges e.g. right-left have the same colors for each pixel)
    // for each tile's edge
    private static int[][][] GetTilesEdgesViableTiles(Color[][][] tilesEdgesColors)
    {
        int tileCount = tilesEdgesColors.Length;
        int[][][] tilesEdgesViableTiles = new int[tileCount][][];

        for (int i = 0; i < tileCount; i++)
        {
            tilesEdgesViableTiles[i] = new int[4][];
            for (int edge = 0; edge < 4; edge++)
            {
                List<int> viableTiles = new List<int>();
                int complementaryEdge = GetComplementaryEdge(edge);

                for (int j = 0; j < tileCount; j++)
                {
                    if (EdgesMatch(tilesEdgesColors[i][edge], tilesEdgesColors[j][complementaryEdge]))
                    {
                        viableTiles.Add(j);
                    }
                }

                tilesEdgesViableTiles[i][edge] = viableTiles.ToArray();
            }
        }

        return tilesEdgesViableTiles;
    }

    // Return the complementary edge index
    private static int GetComplementaryEdge(int edge)
    {
        // 0=bottom, 1=top, 2=left, 3=right
        switch (edge)
        {
            case 0: return 1;
            case 1: return 0;
            case 2: return 3;
            case 3: return 2;
            default: return -1;
        }
    }

    // Return true if edge colors match (pixel for pixel)
    private static bool EdgesMatch(Color[] edge1, Color[] edge2)
    {
        if (edge1.Length != edge2.Length)
        {
            return false;
        }

        for (int pixel = 0; pixel < edge1.Length; pixel++)
        {
            if (!edge1[pixel].Equals(edge2[pixel]))
            {
                return false;
            }
        }

        return true;
    }

    // Construct a grid of given width and height where each cell contains the index for every tile (as all are viable to start)
    private static Dictionary<Vector2Int, int[]> InitializeOutputGrid(int outputWidth, int outputHeight, int tileCount)
    {
        Dictionary<Vector2Int, int[]> outputGrid = new Dictionary<Vector2Int, int[]>();

        for (int x = 0; x < outputWidth; x++)
        {
            for (int y = 0; y < outputHeight; y++)
            {
                int[] allTileIndices = new int[tileCount];

                for (int i = 0; i < tileCount; i++)
                {
                    allTileIndices[i] = i;
                }

                outputGrid.Add(new Vector2Int(x, y), allTileIndices);
            }
        }

        return outputGrid;
    }
}
