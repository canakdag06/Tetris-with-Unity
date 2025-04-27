using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score => score;
    public int Level => level;
    public int Lines => lines;

    private int score;
    private int level = 1;
    private int lines;

    private int lastScore = -1;
    private int lastLevel = -1;
    private int lastLines = -1;

    private static readonly int[] lineClearScores = { 0, 100, 300, 500, 800 };

    public static event Action<ScoreEventData> OnScoreEarned;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void IncreaseLines(int linesToAdd, Vector3 pos)
    {
        score += lineClearScores[linesToAdd] * level;
        Debug.Log("pos: " + pos);
        ScoreEventData data = new ScoreEventData(pos, GetScoreType(linesToAdd), lineClearScores[linesToAdd] * level);
        OnScoreEarned?.Invoke(data);
        lines += linesToAdd;
        level = (lines / 10) + 1;

        ChangeScore(score, true);
        ChangeLevel(level);
        ChangeLines(lines);

    }

    public void IncreaseLines(int scoreToAdd, int linesToAdd, Vector3 pos)
    {
        score += scoreToAdd;
        lines += linesToAdd;
        level = (lines / 10) + 1;

        if(linesToAdd == 0)
        {
            ChangeScore(score, false);
        }
        else
        {
            ChangeScore(score, false);
            ChangeLevel(level);
            ChangeLines(lines);
        }
    }

    private void ChangeScore(int newScore, bool isAnimated = false)
    {
        if (lastScore == newScore) return;
        lastScore = newScore;

        UIManager.Instance.UpdateScore(score, isAnimated);
    }

    private void ChangeLevel(int newLevel)
    {
        if (lastLevel == newLevel) return;
        lastLevel = newLevel;

        UIManager.Instance.UpdateLevel(level);
    }

    private void ChangeLines(int newLines)
    {
        if (lastLines == newLines) return;
        lastLines = newLines;

        UIManager.Instance.UpdateLines(lines);
    }

    public void AddDropScore()
    {
        score++;
        ChangeScore(score);
    }

    public void AddDropScore(int scoreToAdd)
    {
        score += scoreToAdd;
        ChangeScore(score);
    }

    public void ResetStats()
    {
        score = 0;
        level = 1;
        lines = 0;

        lastScore = -1;
        lastLevel = -1;
        lastLines = -1;

        ChangeScore(score);
        ChangeLevel(level);
        ChangeLines(lines);
    }

    private ScoreType GetScoreType(int clearedLineCount)
    {
        switch (clearedLineCount)
        {
            case 1:
                return ScoreType.Single;
            case 2:
                return ScoreType.Double;
            case 3:
                return ScoreType.Triple;
            case 4:
                return ScoreType.Tetris;
            default:
                Debug.LogWarning("Unexpected line clear count: " + clearedLineCount);
                return ScoreType.Single;
        }
    }
}
