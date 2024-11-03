﻿using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } // because Tilemap wants the vector to be 3d
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }


    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);      // clears the board just before the new position occurs. MIGHT BE OPTIMIZED

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.W))    // Clockwise Rotation
        {
            Rotate(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))     // Counter Clockwise Rotation
        {
            Rotate(-1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        board.Set(this);    // sets the new situation of board after the new position is processed. MIGHT BE OPTIMIZED
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool isValid = board.IsValidPosition(this, newPosition);

        if (isValid)
        {
            position = newPosition;
        }
        return isValid;
    }

    private void Rotate(int direction)
    {
        rotationIndex += Wrap(rotationIndex, 0, 4);     // there are 4 rotation cases

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = this.cells[i];   // The reason why it is "Vector3" is that the I and O blocks' pivots are not centered.
                                            // check Tetris Wiki for more information

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }
            cells[i] = new Vector3Int(x, y, 0);
        }

    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
        /*
         * ?
         * return ((input - min) % (max - min)) + min;
         * ?
         */
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
    }
}
