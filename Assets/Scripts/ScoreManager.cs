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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void IncreaseLines(int linesToAdd)
    {
        score += lineClearScores[linesToAdd] * level;
        lines += linesToAdd;
        level = (lines / 10) + 1;

        ChangeScore(score, true);
        ChangeLevel(level);
        ChangeLines(lines);
    }

    public void IncreaseLines(int scoreToAdd, int linesToAdd)
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
}
