using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NextPiecesAndHoldDisplayer : MonoBehaviour
{
    public Board board;
    public Tilemap sharedTilemap;
    public Vector3Int nextPieceStartPosition;
    public Vector3Int holdPiecePosition;
    public int spacing;

    public void UpdateNextPiecesDisplay(List<int> bag)
    {
        //sharedTilemap.ClearAllTiles();
        ClearArea(new Vector3Int(-3,-7,0),
                  new Vector3Int(3,7,0));
        var nextTetrominos = bag.Take(3).ToList();

        Vector3Int drawPosition = nextPieceStartPosition;

        foreach (var tetrominoIndex in nextTetrominos)
        {
            var tetrominoType = (Tetromino)tetrominoIndex;
            DrawTetromino(tetrominoType, drawPosition);
            drawPosition.y -= spacing;
        }
    }

    public void UpdateHoldPiece(TetrominoData data)
    {
        ClearArea(new Vector3Int(-21, -2, 0),
                  new Vector3Int(-16, 2, 0));

        foreach (var cell in data.cells)
        {
            Vector3Int tilePosition = holdPiecePosition + (Vector3Int)cell;
            sharedTilemap.SetTile(tilePosition, data.tile);
            //Debug.Log("Piece: " + tilePosition);
        }
    }

    private void DrawTetromino(Tetromino tetromino, Vector3Int position)
    {
        if (!Data.Cells.TryGetValue(tetromino, out var cells))
        {
            Debug.LogWarning($"No cells found for Tetromino {tetromino}.");
            return;
        }

        foreach (var cell in cells)
        {
            Vector3Int tilePosition = position + (Vector3Int)cell;
            //Debug.Log($"Drawing Tetromino at: {tilePosition}");
            sharedTilemap.SetTile(tilePosition, board.tetrominoes[((int)tetromino)].tile);
        }
    }

    private void ClearArea(Vector3Int from, Vector3Int to)
    {
        int minX = Mathf.Min(from.x, to.x);
        int maxX = Mathf.Max(from.x, to.x);
        int minY = Mathf.Min(from.y, to.y);
        int maxY = Mathf.Max(from.y, to.y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                sharedTilemap.SetTile(pos, null);
                //Debug.Log("CLEAR: " + pos);
            }
        }
    }
}
