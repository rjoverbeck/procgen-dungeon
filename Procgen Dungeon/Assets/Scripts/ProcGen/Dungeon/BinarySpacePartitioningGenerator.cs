using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitioningGenerator
{
    public static Queue<Vector2Int> Generate(BoundsInt areaToPartition, int minWidth, int minHeight, int partitionOffset)
    {
        Queue<BoundsInt> partitionableArea = new Queue<BoundsInt>();
        Queue<BoundsInt> partitions = new Queue<BoundsInt>();

        partitionableArea.Enqueue(areaToPartition);

        while (partitionableArea.Count > 0)
        {
            BoundsInt partition = partitionableArea.Dequeue();

            bool preferHorizontal = Random.value < 0.5f;

            if (preferHorizontal)
            {
                if (partition.size.y >= minHeight * 2)
                {
                    PartitionHorizontally(partition, partitionableArea, minHeight);
                }
                else if (partition.size.x >= minWidth * 2)
                {
                    PartitionVertically(partition, partitionableArea, minWidth);
                }
                else
                {
                    partitions.Enqueue(partition);
                }
            }
            else
            {
                if (partition.size.x >= minWidth * 2)
                {
                    PartitionVertically(partition, partitionableArea, minWidth);
                }
                else if (partition.size.y >= minHeight * 2)
                {
                    PartitionHorizontally(partition, partitionableArea, minHeight);
                }
                else
                {
                    partitions.Enqueue(partition);
                }
            }

        }

        return GetPositionsFromPartitions(partitions, partitionOffset);
    }

    private static void PartitionHorizontally(BoundsInt partition, Queue<BoundsInt> partitionQueue, int minHeight)
    {
        int splitY = Random.Range(minHeight, partition.size.y - minHeight + 1);

        BoundsInt bottom = new BoundsInt(
            partition.min,
            new Vector3Int(partition.size.x, splitY, partition.size.z)
        );

        BoundsInt top = new BoundsInt(
            new Vector3Int(partition.min.x, partition.min.y + splitY, partition.min.z),
            new Vector3Int(partition.size.x, partition.size.y - splitY, partition.size.z)
        );

        partitionQueue.Enqueue(bottom);
        partitionQueue.Enqueue(top);
    }

    private static void PartitionVertically(BoundsInt partition, Queue<BoundsInt> partitionQueue, int minWidth)
    {
        int splitX = Random.Range(minWidth, partition.size.x - minWidth + 1);

        BoundsInt left = new BoundsInt(
            partition.min,
            new Vector3Int(splitX, partition.size.y, partition.size.z)
        );

        BoundsInt right = new BoundsInt(
            new Vector3Int(partition.min.x + splitX, partition.min.y, partition.min.z),
            new Vector3Int(partition.size.x - splitX, partition.size.y, partition.size.z)
        );

        partitionQueue.Enqueue(left);
        partitionQueue.Enqueue(right);
    }

    private static Queue<Vector2Int> GetPositionsFromPartitions(Queue<BoundsInt> partitions, int partitionOffset)
    {
        Queue<Vector2Int> positions = new Queue<Vector2Int>();

        foreach (var partition in partitions)
        {
            for (int x = partitionOffset; x < partition.size.x - partitionOffset; x++)
            {
                for (int y = partitionOffset; y < partition.size.y - partitionOffset; y++)
                {
                    Vector2Int position = (Vector2Int)partition.min + new Vector2Int(x, y);
                    positions.Enqueue(position);
                }
            }
        }

        return positions;
    }
}
