﻿using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } // because Tilemap wants the vector to be 3d
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
    public float initialDelay = 0.5f; // delay for first move
    public float repeatDelay = 0.1f;   // delay for repated moves

    private float stepTime;
    private float lockTime;
    private float verticalHoldTime = 0f;     // while the player holds down the key, it stores when the next move will occur
    private float horizontalHoldTime = 0f;
    private bool isDroppingManually;


    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;


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
        lockTime += Time.deltaTime;


        HandleInput(KeyCode.A, Vector2Int.left, ref horizontalHoldTime);
        HandleInput(KeyCode.D, Vector2Int.right, ref horizontalHoldTime);
        HandleInput(KeyCode.S, Vector2Int.down, ref verticalHoldTime);

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Move(Vector2Int.left);
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    Move(Vector2Int.right);
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    Move(Vector2Int.down);
        //}

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

        if (Time.time >= stepTime)
        {
            Step();
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
            lockTime = 0f;  // if the piece is moving, game does not lock the piece
        }
        return isValid;
    }

    private void HandleInput(KeyCode key, Vector2Int direction, ref float holdTime)
    {
        if (Input.GetKey(key))
        {
            if (holdTime == 0f)
            {
                if (Move(direction))
                {
                    if(direction == Vector2Int.down)    // moving down repeatedly has no initial delay
                    {
                        isDroppingManually = true;
                        holdTime = repeatDelay;
                    }
                    else
                    {
                        holdTime = initialDelay;
                    }
                }
            }
            else
            {   // Key Repeat
                holdTime -= Time.deltaTime;
                if (holdTime <= 0f)
                {
                    if (Move(direction))
                    {
                        holdTime = repeatDelay;
                    }
                }
            }
        }
        else if (Input.GetKeyUp(key))
        {
            holdTime = 0f;

            if(key == KeyCode.S)
            {
                isDroppingManually = false;
            }
        }
    }

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);     // there are 4 rotation cases
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
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

    private void Step()
    {
        stepTime = Time.time + this.stepDelay;

        if(!isDroppingManually) // because it drops 2 steps in common multiples of delay values
        {
            Move(Vector2Int.down);
        }

        if (this.lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece(board.GetNextPiece());
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)  // to understand this function
    {                                                                       // please check the table at "tetris wiki/srs/J,L,S,T,Z Tetromino Wall Kick Data
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
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

        Lock();
    }
}
