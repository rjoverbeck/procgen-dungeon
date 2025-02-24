using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkGenerator
{
    public static Queue<Vector2Int> Generate(Vector2Int startPosition, int numberOfSteps, int numberOfWalks)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> path = new Queue<Vector2Int>();

        for (int i = 0; i < numberOfWalks; i++)
        {
            Vector2Int prevPosition = startPosition;

            if (visited.Add(prevPosition))
            {
                path.Enqueue(prevPosition);
            }

            for (int j = 0; j < numberOfSteps; j++)
            {
                Vector2Int nextPosition = prevPosition + GetRandomDirection();

                if (visited.Add(nextPosition))
                {
                    path.Enqueue(nextPosition);
                }

                prevPosition = nextPosition;
            }
        }

        return path;
    }

    private static Vector2Int GetRandomDirection()
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
