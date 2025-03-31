using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapseGenerator
{
    public static Queue<Vector2Int> Generate(List<Texture2D> tiles)
    {
        Color[][][] tilesEdgesColors = GetTilesEdgesColors(tiles);

        // temporary logging
        for (int i = 0; i < tilesEdgesColors.Length; i++)
        {
            string logMsgTile0 = $"Tile {i}:" +
                                 $"\nBottom: {string.Join(", ", tilesEdgesColors[i][0])}";
            string logMsgTile1 = $"Tile {i}:" +
                                 $"\nTop: {string.Join(", ", tilesEdgesColors[i][1])}";
            string logMsgTile2 = $"Tile {i}:" +
                                 $"\nLeft: {string.Join(", ", tilesEdgesColors[i][2])}";
            string logMsgTile3 = $"Tile {i}:" +
                                 $"\nRight: {string.Join(", ", tilesEdgesColors[i][3])}";
            Debug.Log(logMsgTile0);
            Debug.Log(logMsgTile1);
            Debug.Log(logMsgTile2);
            Debug.Log(logMsgTile3);
        }

        return null;
    }

    private static Color[][][] GetTilesEdgesColors(List<Texture2D> tiles)
    {
        Color[][][] tilesEdgesColors = new Color[tiles.Count][][];

        for (int i = 0; i < tiles.Count; i++)
        {
            Texture2D tile = tiles[i];
            int width = tile.width;
            int height = tile.height;

            Color[] topBorder = new Color[width];
            Color[] bottomBorder = new Color[width];
            Color[] leftBorder = new Color[height];
            Color[] rightBorder = new Color[height];

            for (int x = 0; x < width; x++)
            {
                bottomBorder[x] = tile.GetPixel(x, 0);
                topBorder[x] = tile.GetPixel(x, height - 1);
            }

            for (int y = 0; y < height; y++)
            {
                leftBorder[y] = tile.GetPixel(0, y);
                rightBorder[y] = tile.GetPixel(width - 1, y);
            }

            tilesEdgesColors[i] = new Color[4][];
            tilesEdgesColors[i][0] = bottomBorder;
            tilesEdgesColors[i][1] = topBorder;
            tilesEdgesColors[i][2] = leftBorder;
            tilesEdgesColors[i][3] = rightBorder;
        }

        return tilesEdgesColors;
    }
}
