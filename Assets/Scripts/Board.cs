using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public static event Action<int> OnGameOver;

    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public NextPiecesAndHoldDisplayer nextPiecesAndHoldDisplayer;

    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public TetrominoData holdPieceData { get; private set; } = default;
    private bool holdUsed = false;
    public float stepDelay;
    public float[] stepDelays = { 1f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.1f };
    private int stepDelayIndex = 0;
    private List<int> bag = new();
    private List<int> tempBag = new();
    private System.Random random = new();

    public RectInt Bounds
    {   // bounds of the rectangle from bottom left to top right
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);   // bottom left point
            return new RectInt(position, boardSize);
        }
    }

    private void OnEnable()
    {
        ScoreManager.OnLevelChanged += HandleLevelChanged;
    }

    private void OnDisable()
    {
        ScoreManager.OnLevelChanged -= HandleLevelChanged;
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        InputReader.Instance.ResetInputs();
        ScoreManager.Instance.ResetStats();
        stepDelay = stepDelays[stepDelayIndex];
        bag.Clear();
        GenerateNewBag();
        SpawnPiece(GetNextPiece());
    }

    private void GenerateNewBag()
    {
        foreach (Tetromino tetromino in Enum.GetValues(typeof(Tetromino)))
        {
            tempBag.Add((int)tetromino);
        }

        // Shuffle (Fisher-Yates algorithm)
        for (int i = tempBag.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (tempBag[i], tempBag[j]) = (tempBag[j], tempBag[i]); // Swap
        }

        bag.AddRange(tempBag);
        tempBag.Clear();
    }
    public int GetNextPiece()
    {
        holdUsed = false;

        if (bag.Count == 3)
        {
            GenerateNewBag();
        }

        // Take the last piece out of the bag
        int nextPieceIndex = bag[0];
        bag.RemoveAt(0);
        nextPiecesAndHoldDisplayer.UpdateNextPiecesDisplay(bag);
        return nextPieceIndex;
    }

    public void SpawnPiece(int random)
    {
        ;
        TetrominoData data = this.tetrominoes[random];

        this.activePiece.Initialize(this, this.spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (tilemap.HasTile(tilePosition) || !Bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public int ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int lines = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                ClearThisLine(row);
                lines++;
            }
            else
            {
                row++;
            }
        }

        ScoreManager.Instance.IncreaseLines(lines, activePiece.position);
        if (lines > 0)
            AudioManager.Instance.PlaySFX(SoundType.LineClear);

        return lines;
    }

    public void Hold()
    {
        if (holdUsed)
        {
            AudioManager.Instance.PlaySFX(SoundType.HoldFailed);
            return;
        }

        Clear(activePiece);

        if (holdPieceData.Equals(default(TetrominoData)))
        {
            holdPieceData = activePiece.data;
            SpawnPiece(GetNextPiece());
        }
        else
        {
            TetrominoData temp = holdPieceData;
            holdPieceData = activePiece.data;
            SpawnPiece(Array.IndexOf(tetrominoes, temp));
        }
        holdUsed = true;
        AudioManager.Instance.PlaySFX(SoundType.Hold);
        nextPiecesAndHoldDisplayer.UpdateHoldPiece(holdPieceData);
    }

    private void HandleLevelChanged()
    {
        if (stepDelayIndex == 10) return;
        stepDelayIndex++;
        stepDelay = stepDelays[stepDelayIndex];
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void ClearThisLine(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }
            row++;
        }
    }

    private void GameOver()
    {
        int finalScore = ScoreManager.Instance.Score;
        OnGameOver?.Invoke(finalScore);
        activePiece.enabled = false;
    }

}