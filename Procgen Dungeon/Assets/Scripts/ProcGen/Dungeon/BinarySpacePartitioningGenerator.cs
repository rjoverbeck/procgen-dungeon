using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitioningGenerator
{
    public static List<BoundsInt> Generate(BoundsInt areaToPartition, int minWidth, int minHeight)
    {
        Queue<BoundsInt> partitionQueue = new Queue<BoundsInt>();
        List<BoundsInt> partitions = new List<BoundsInt>();

        partitionQueue.Enqueue(areaToPartition);

        while (partitionQueue.Count > 0)
        {
            BoundsInt partition = partitionQueue.Dequeue();

            if (partition.size.x >= minWidth && partition.size.y >= minHeight)
            {
                bool preferHorizontal = Random.value < 0.5f;

                if (preferHorizontal)
                {
                    if (partition.size.y >= minHeight * 2)
                    {
                        PartitionHorizontally(partition, partitionQueue, minHeight);
                    }
                    else if (partition.size.x >= minWidth * 2)
                    {
                        PartitionVertically(partition, partitionQueue, minWidth);
                    }
                    else
                    {
                        partitions.Add(partition);
                    }
                }
                else
                {
                    if (partition.size.x >= minWidth * 2)
                    {
                        PartitionVertically(partition, partitionQueue, minWidth);
                    }
                    else if (partition.size.y >= minHeight * 2)
                    {
                        PartitionHorizontally(partition, partitionQueue, minHeight);
                    }
                    else
                    {
                        partitions.Add(partition);
                    }
                }
            }
            else
            {
                partitions.Add(partition);
            }
        }

        return partitions;
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
}
