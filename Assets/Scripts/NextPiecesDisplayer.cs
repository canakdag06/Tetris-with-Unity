using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NextPiecesDisplayer : MonoBehaviour
{
    public Board board;
    public Tilemap tilemap;
    public Vector3Int startPosition;
    public int spacing = 3;

    public void UpdateNextPiecesDisplay(List<int> bag)
    {
        tilemap.ClearAllTiles();
        var nextTetrominos = bag.TakeLast(3).Reverse().ToList();    // the reason i use reverse is positioning the first next tetromino to the top

        Vector3Int drawPosition = startPosition;

        foreach (var tetrominoIndex in nextTetrominos)
        {
            var tetrominoType = (Tetromino)tetrominoIndex;
            DrawTetromino(tetrominoType, drawPosition);
            drawPosition.y -= spacing;
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
            tilemap.SetTile(tilePosition, board.tetrominoes[((int)tetromino)].tile);
        }
    }
}
