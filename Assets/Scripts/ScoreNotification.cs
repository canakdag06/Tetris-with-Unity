using UnityEngine;

public class ScoreNotification : MonoBehaviour
{

    private void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}

public class ScoreEventData
{
    public Vector3 pos;
    public ScoreType scoreType;
    public int scoreAmount;

    public ScoreEventData(Vector3 pos, ScoreType scoreType, int scoreAmount)
    {
        this.pos = pos;
        this.scoreType = scoreType;
        this.scoreAmount = scoreAmount;
    }
}

public enum ScoreType
{
    None,
    Single,
    Double,
    Triple,
    Tetris,
    TSpin,
    TSpinSingle,
    TSpinDouble,
    TSpinTriple,
    Combo
}