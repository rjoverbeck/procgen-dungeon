using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap _floorTilemap;
    [SerializeField]
    private TileBase _floorTile;
    [SerializeField]
    private int _numberOfPositions;

    private void Start()
    {
        Vector2Int topLeft = new Vector2Int(-10, 10);
        Vector2Int topRight = new Vector2Int(10, 10);
        Vector2Int bottomLeft = new Vector2Int(-10, -10);
        Vector2Int bottomRight = new Vector2Int(10, -10);

        DrawFloorTiles(RandomPositionGenerator.Generate(topLeft, topRight, bottomLeft, bottomRight, _numberOfPositions));
    }

    private void DrawFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        foreach (var floorPosition in floorPositions)
        {
            Vector3Int tilePosition = new Vector3Int(floorPosition.x, floorPosition.y, 0);
            _floorTilemap.SetTile(tilePosition, _floorTile);
        }
    }
}
