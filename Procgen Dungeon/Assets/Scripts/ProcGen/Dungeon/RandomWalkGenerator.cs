using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkGenerator
{
    public static HashSet<Vector2Int> Generate(Vector2Int startPosition, int numberOfSteps, int numberOfWalks)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        for (int i = 0; i < numberOfWalks; i++)
        {
            path.Add(startPosition);
            Vector2Int prevPosition = startPosition;

            for (int j = 0; j < numberOfSteps; j++)
            {
                Vector2Int nextPosition = prevPosition + getRandomDirection();
                path.Add(nextPosition);
                prevPosition = nextPosition;
            }
        }

        return path;
    }

    private static Vector2Int getRandomDirection()
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.down
        };

        return directions[Random.Range(0, directions.Count)];
    }
}
