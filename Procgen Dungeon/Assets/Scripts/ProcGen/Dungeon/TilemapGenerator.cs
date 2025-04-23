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
    [SerializeField]
    private float tileGenerationSpeed = 0.1f;

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

    [Header("Wave Function Collapse")]
    [SerializeField]
    private List<Texture2D> tiles;
    [SerializeField]
    private List<int> tileWeights;
    [SerializeField]
    private int outputWidth;
    [SerializeField]
    private int outputHeight;

    [Header("UI Elements")]
    [SerializeField]
    private TMP_Text tileCountText;

    private Queue<Vector2Int> _floorPositions;
    private int _totalTiles;
    private int _placedTiles;
    private Coroutine _coroutine;

    private void Start()
    {
        _totalTiles = 0;
        _placedTiles = 0;

        UpdateTileCountUI();
    }

    public void CreateBSPTilemap()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        BoundsInt dungeonArea = new BoundsInt(
            position: bspStartPosition,
            size: new Vector3Int(dungeonWidth, dungeonHeight, 0)
        );

        _floorPositions = BinarySpacePartitioningGenerator.Generate(dungeonArea, minRoomWidth, minRoomHeight, roomOffset);

        _totalTiles = _floorPositions.Count;
        _placedTiles = 0;

        UpdateTileCountUI();

        _coroutine = StartCoroutine(DrawFloorTilesOverTime());
    }

    public void CreateRWTilemap()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _floorPositions = RandomWalkGenerator.Generate(rwStartPosition, numberOfSteps, numberOfWalks);

        _totalTiles = _floorPositions.Count;
        _placedTiles = 0;

        UpdateTileCountUI();

        _coroutine = StartCoroutine(DrawFloorTilesOverTime());
    }

    public void CreateWFCTilemap()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _floorPositions = WaveFunctionCollapseGenerator.Generate(tiles, tileWeights, outputWidth, outputHeight);

        _totalTiles = _floorPositions.Count;
        _placedTiles = 0;

        UpdateTileCountUI();

        _coroutine = StartCoroutine(DrawFloorTilesOverTime());
    }

    private void UpdateTileCountUI()
    {
        tileCountText.text = $"Tiles Placed: {_placedTiles} / {_totalTiles}";
    }

    private IEnumerator DrawFloorTilesOverTime()
    {
        floorTilemap.ClearAllTiles();

        while (_floorPositions.Count > 0)
        {
            Vector2Int floorPosition = _floorPositions.Dequeue();
            Vector3Int tilePosition = new Vector3Int(floorPosition.x, floorPosition.y, 0);

            floorTilemap.SetTile(tilePosition, floorTile);

            _placedTiles++;

            UpdateTileCountUI();

            yield return new WaitForSeconds(tileGenerationSpeed);
        }
    }
}
