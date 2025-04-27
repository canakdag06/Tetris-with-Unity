using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Single,
    Double,
    Triple,
    Tetris,
    TSpin,
    TSpinMini,
    TSpinDouble,
}