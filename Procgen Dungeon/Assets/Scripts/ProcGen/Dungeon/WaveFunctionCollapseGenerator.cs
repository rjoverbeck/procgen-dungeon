using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunctionCollapseGenerator
{
    public static Queue<Vector2Int> Generate(List<Texture2D> tiles, List<int> tileWeights, int outputWidth, int outputHeight)
    {
        Color[][][] tilesEdgesColors = GetTilesEdgesColors(tiles);

        int[][][] tilesEdgesViableTiles = GetTilesEdgesViableTiles(tilesEdgesColors);

        Dictionary<int, int> tilesToWeights = GetTilesToWeights(tiles.Count, tileWeights);

        Dictionary<Vector2Int, Cell> outputGrid = InitializeOutputGrid(outputWidth, outputHeight, tilesToWeights);

        CollapseCells(outputGrid, tilesEdgesViableTiles);

        // TODO: Remove temporary logging
        foreach (var cell in outputGrid)
        {
            Debug.Log($"Cell: {cell.Key}" + $" - Tile: {cell.Value.GetViableTilesToWeights().Keys.First()}");
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

    private static Dictionary<int, int> GetTilesToWeights(int tileCount, List<int> tileWeights)
    {
        Dictionary<int, int> tilesToWeights = new Dictionary<int, int>();

        for (int i = 0; i < tileCount; i++)
        {
            tilesToWeights.Add(i, tileWeights[i]);
        }

        return tilesToWeights;
    }

    private class Cell
    {
        private Dictionary<int, int> viableTilesToWeights;
        private float entropy;

        public Cell(Dictionary<int, int> viableTilesToWeights)
        {
            this.viableTilesToWeights = viableTilesToWeights;
            entropy = ComputeEntropy();
        }

        private int ComputeTotalWeight()
        {
            int totalWeight = 0;

            foreach (int weight in viableTilesToWeights.Values)
            {
                totalWeight += weight;
            }

            return totalWeight;
        }

        private float ComputeEntropy()
        {
            float totalWeight = ComputeTotalWeight();
            float entropy = 0f;
            foreach (int weight in viableTilesToWeights.Values)
            {
                float probability = weight / totalWeight;
                entropy -= probability * Mathf.Log(probability);
            }

            return entropy;
        }

        public void Collapse()
        {
            List<int> tilesByWeight = new List<int>();
            foreach (var entry in viableTilesToWeights)
            {
                for (int i = 0; i < entry.Value; i++)
                {
                    tilesByWeight.Add(entry.Key);
                }
            }

            int selectedTile = tilesByWeight[Random.Range(0, tilesByWeight.Count)];

            viableTilesToWeights.Clear();
            viableTilesToWeights.Add(selectedTile, 1);
            entropy = 0f;
        }

        public Dictionary<int, int> GetViableTilesToWeights() { return viableTilesToWeights; }

        public float GetEntropy()
        {
            ComputeEntropy();
            return entropy;
        }
    }

    private static Dictionary<Vector2Int, Cell> InitializeOutputGrid(int outputWidth, int outputHeight, Dictionary<int, int> tilesToWeights)
    {
        Dictionary<Vector2Int, Cell> outputGrid = new Dictionary<Vector2Int, Cell>();

        for (int x = 0; x < outputWidth; x++)
        {
            for (int y = 0; y < outputHeight; y++)
            {
                Dictionary<int, int> copy = tilesToWeights.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                outputGrid.Add(new Vector2Int(x, y), new Cell(copy));
            }
        }

        return outputGrid;
    }

    private static void CollapseCells(Dictionary<Vector2Int, Cell> outputGrid, int[][][] tilesEdgesViableTiles)
    {
        Dictionary<Vector2Int, int> neighborToEdgeMap = new Dictionary<Vector2Int, int>
        {
            { new Vector2Int(0, 1), 0 }, // neighbor below - bottom edge
            { new Vector2Int(0, -1), 1 }, // neighbor above - top edge
            { new Vector2Int(-1, 0), 2 }, // neighbor to the left - left edge
            { new Vector2Int(1, 0), 3 } // neighbor to the right - right edge
        };

        for (int i = 0; i < outputGrid.Count; i++)
        {
            Vector2Int minEntropyCellPosition = new Vector2Int(-1, -1);
            float minEntropy = float.MaxValue;

            foreach (var cell in outputGrid)
            {
                float currEntropy = cell.Value.GetEntropy();

                if (currEntropy < minEntropy)
                {
                    minEntropy = currEntropy;
                    minEntropyCellPosition = cell.Key;
                }
            }

            outputGrid[minEntropyCellPosition].Collapse();
            int collapsedTile = outputGrid[minEntropyCellPosition].GetViableTilesToWeights().Keys.First();

            foreach (var neighborToEdge in neighborToEdgeMap)
            {
                Vector2Int neighborPos = minEntropyCellPosition + neighborToEdge.Key;

                if (!outputGrid.ContainsKey(neighborPos))
                    continue;

                Cell neighborCell = outputGrid[neighborPos];

                if (neighborCell.GetViableTilesToWeights().Count <= 1)
                    continue;

                int edge = neighborToEdge.Value;
                HashSet<int> allowedTiles = new HashSet<int>(tilesEdgesViableTiles[collapsedTile][edge]);

                List<int> keysToRemove = new List<int>();
                foreach (var viableTile in neighborCell.GetViableTilesToWeights().Keys)
                {
                    if (!allowedTiles.Contains(viableTile))
                        keysToRemove.Add(viableTile);
                }

                foreach (int key in keysToRemove)
                {
                    neighborCell.GetViableTilesToWeights().Remove(key);
                }
            }
        }
    }
}
