using System.Collections.Generic;
using UnityEngine;

public static class RandomPositionGenerator
{
    // TODO: Extend base class for each algorithm
    public static HashSet<Vector2Int> Generate(Vector2Int topLeft, Vector2Int topRight, Vector2Int bottomLeft, Vector2Int bottomRight, int numberOfPositions)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();

        int minX = Mathf.Min(topLeft.x, bottomLeft.x);
        int maxX = Mathf.Max(topRight.x, bottomRight.x);
        int minY = Mathf.Min(bottomLeft.y, bottomRight.y);
        int maxY = Mathf.Max(topLeft.y, topRight.y);

        while (positions.Count < numberOfPositions)
        {
            Vector2Int randomPosition = new Vector2Int(Random.Range(minX, maxX + 1), Random.Range(minY, maxY + 1));
            positions.Add(randomPosition);
        }

        return positions;
    }
}
