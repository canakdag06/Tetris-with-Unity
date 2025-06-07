using UnityEngine;
using UnityEngine.Tilemaps;

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

    private int clearedLines;


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

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            board.Hold();
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

            if (translation.y == 0)
            {
                lockTime -= Time.deltaTime * 2; // Extend the lock time a bit but do not reset it
                lockTime = Mathf.Max(lockTime, 0f); // Make sure it's not negative
            }
            else
            {
                lockTime = 0f; // Reset when moved down
            }

            if (isDroppingManually)
            {
                ScoreManager.Instance.AddDropScore();
            }
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
                    if (direction == Vector2Int.down)    // moving down repeatedly has no initial delay
                    {
                        isDroppingManually = true;
                        ScoreManager.Instance.AddDropScore();
                        holdTime = repeatDelay;
                    }
                    else
                    {
                        holdTime = initialDelay;
                        AudioManager.Instance.PlaySFX(SoundType.Move);
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
                        AudioManager.Instance.PlaySFX(SoundType.Move);
                    }
                }
            }
        }
        else if (Input.GetKeyUp(key))
        {
            holdTime = 0f;

            if (key == KeyCode.S)
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

        AudioManager.Instance.PlaySFX(SoundType.Rotate);
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

        if (!isDroppingManually) // because it drops 2 steps in common multiples of delay values
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
        clearedLines = board.ClearLines();
        if(data.tetromino == Tetromino.T)
        {
            CheckTSpin(clearedLines);
        }

        AudioManager.Instance.PlaySFX(SoundType.Lock);

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
        int stepCount = 0;

        while (Move(Vector2Int.down))
        {
            stepCount++;
            continue;
        }

        ScoreManager.Instance.AddDropScore(stepCount * 2);
        Lock();
    }

    private bool IsTSpin(Vector2Int position, int rotationIndex)
    {
        Vector2Int[] cornerOffsets = new Vector2Int[]
        {
        new Vector2Int(-1, 1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1)
        };

        int filledCorners = 0;
        foreach (var offset in cornerOffsets)
        {
            Vector2Int checkPos = position + offset;
            if (IsCellOccupied(checkPos))
            {
                filledCorners++;
            }
        }

        return filledCorners >= 3;
    }

    private void CheckTSpin(int linesCleared)
    {
        bool isTSpin = IsTSpin(new Vector2Int(position.x,position.y), rotationIndex);

        if (isTSpin)
        {
            int scoreToAdd = 0;

            if (linesCleared == 0) scoreToAdd = 400;  // T-Spin
            else if (linesCleared == 1) scoreToAdd = 800;  // T-Spin Single
            else if (linesCleared == 2) scoreToAdd = 1200; // T-Spin Double
            else if (linesCleared == 3) scoreToAdd = 1600; // T-Spin + Triple
            Debug.Log("T SPIN!: " + scoreToAdd + " points.");
            Debug.Log("linesCleared : " + linesCleared);
            ScoreManager.Instance.IncreaseLines(scoreToAdd, linesCleared, this.position);
            //data.tile.transform.GetPosition()
        }
    }

    private bool IsCellOccupied(Vector2Int position)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        return board.tilemap.HasTile(tilePosition);
    }
}
