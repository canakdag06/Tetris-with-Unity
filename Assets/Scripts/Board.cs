using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    private List<int> bag = new();
    private System.Random random = new();

    public RectInt Bounds
    {   // bounds of the rectangle from bottom left to top right
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);   // bottom left point
            return new RectInt(position, boardSize);
        }
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
        SpawnPiece(GetNextPiece());
    }

    private void GenerateNewBag()
    {
        bag.Clear();

        foreach (Tetromino tetromino in Enum.GetValues(typeof(Tetromino)))
        {
            bag.Add((int)tetromino);
        }

        // Shuffle (Fisher-Yates algorithm)
        for (int i = bag.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]); // Swap
        }

        Debug.LogError("New Bag:");
        foreach (int index in bag)
        {
            Debug.LogError(this.tetrominoes[index].tetromino);
        }

        //for (int i = 1; i <= 7; i++)
        //{
        //    int random = Random.Range(0, 10);
        //}
    }
    public int GetNextPiece()
    {
        if (bag.Count == 0)
        {
            GenerateNewBag();
        }

        // Take the last piece out of the bag
        int nextPieceIndex = bag[bag.Count - 1];
        bag.RemoveAt(bag.Count - 1);
        return nextPieceIndex;
    }

    public void SpawnPiece(int random)
    {
        //int random = Random.Range(0, this.tetrominoes.Length);
        //TetrominoData data = this.tetrominoes[random];
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

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                ClearThisLine(row);
            }
            else
            {
                row++;
            }
        }
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

    private void GameOver() // add UI later
    {
        tilemap.ClearAllTiles();
    }


}