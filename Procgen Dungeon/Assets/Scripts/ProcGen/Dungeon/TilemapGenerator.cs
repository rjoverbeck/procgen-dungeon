using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private TileBase floorTile;

    [SerializeField]
    private int minRoomWidth = 8;
    [SerializeField]
    private int minRoomHeight = 8;
    [SerializeField]
    private int dungeonWidth = 60;
    [SerializeField]
    private int dungeonHeight = 60;
    [SerializeField]
    private Vector3Int startPosition = new Vector3Int(-30, -30, 0);

    [SerializeField]
    [Range(0, 10)]
    private int roomOffset = 1;

    private void Start()
    {
        GenerateBinarySpacePartitionedRooms();
    }

    private void GenerateBinarySpacePartitionedRooms()
    {
        BoundsInt dungeonArea = new BoundsInt(
            position: startPosition,
            size: new Vector3Int(dungeonWidth, dungeonHeight, 1)
        );

        List<BoundsInt> rooms = BinarySpacePartitioningGenerator.Generate(dungeonArea, minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floorPositions = GetPositionsFromRooms(rooms);
        DrawFloorTiles(floorPositions);
    }

    private HashSet<Vector2Int> GetPositionsFromRooms(List<BoundsInt> rooms)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        foreach (var room in rooms)
        {
            for (int x = roomOffset; x < room.size.x - roomOffset; x++)
            {
                for (int y = roomOffset; y < room.size.y - roomOffset; y++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(x, y);
                    floorPositions.Add(position);
                }
            }
        }

        return floorPositions;
    }

    private void DrawFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        foreach (var floorPosition in floorPositions)
        {
            Vector3Int tilePosition = new Vector3Int(floorPosition.x, floorPosition.y, 0);
            floorTilemap.SetTile(tilePosition, floorTile);
        }
    }
}
