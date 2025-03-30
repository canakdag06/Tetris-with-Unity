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
    private static readonly int[] lineClearScores = { 0, 100, 300, 500, 800 };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void IncreaseStats(int linesToAdd)
    {
        score += lineClearScores[linesToAdd];
        lines += linesToAdd;
        level = (lines / 10) + 1;

        UpdateAllStats();
    }

    private void UpdateAllStats()
    {
        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateLevel(level);
        UIManager.Instance.UpdateLines(lines);
    }

    public void ResetStats()
    {
        score = 0;
        level = 1;
        lines = 0;

        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateLevel(level);
        UIManager.Instance.UpdateLines(lines);
    }
}
