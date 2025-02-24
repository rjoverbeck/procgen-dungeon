using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapGenerator : MonoBehaviour
{
    [Header("Common")]
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private TileBase floorTile;

    [Header("Binary Space Partitioning")]
    [SerializeField]
    private int minRoomWidth = 8;
    [SerializeField]
    private int minRoomHeight = 8;
    [SerializeField]
    private int dungeonWidth = 60;
    [SerializeField]
    private int dungeonHeight = 60;
    [SerializeField]
    private Vector3Int bspStartPosition = new Vector3Int(-30, -30, 0);
    [SerializeField]
    [Range(0, 10)]
    private int roomOffset = 1;

    [Header("Random Walk")]
    [SerializeField]
    private Vector2Int rwStartPosition = new Vector2Int(0, 0);
    [SerializeField]
    private int numberOfSteps = 40;
    [SerializeField]
    private int numberOfWalks = 20;

    [Header("UI Elements")]
    [SerializeField]
    private TMP_Text tileCountText;

    private Queue<Vector2Int> _floorPositions;
    private int _totalTiles;
    private int _placedTiles;

    private void Start()
    {
        _floorPositions = RandomWalkGenerator.Generate(rwStartPosition, numberOfSteps, numberOfWalks);
        _totalTiles = _floorPositions.Count;
        _placedTiles = 0;

        UpdateTileCountUI();

        StartCoroutine(DrawFloorTilesOverTime());

        //GenerateBinarySpacePartitionedRooms();
        //GenerateRandomWalkRoom();
    }

    private void UpdateTileCountUI()
    {
        tileCountText.text = $"Tiles Placed: {_placedTiles} / {_totalTiles}";
    }

    private IEnumerator DrawFloorTilesOverTime()
    {
        while (_floorPositions.Count > 0)
        {
            Vector2Int floorPosition = _floorPositions.Dequeue();
            Vector3Int tilePosition = new Vector3Int(floorPosition.x, floorPosition.y, 0);

            floorTilemap.SetTile(tilePosition, floorTile);

            _placedTiles++;

            UpdateTileCountUI();

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void GenerateRandomWalkRoom()
    {
        //HashSet<Vector2Int> floorPositions = RandomWalkGenerator.Generate(rwStartPosition, numberOfSteps, numberOfWalks);
        //DrawFloorTiles(floorPositions);
    }

    private void GenerateBinarySpacePartitionedRooms()
    {
        BoundsInt dungeonArea = new BoundsInt(
            position: bspStartPosition,
            size: new Vector3Int(dungeonWidth, dungeonHeight, 0)
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
