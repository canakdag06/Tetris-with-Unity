using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score => score;
    public int Level => level;
    public int Lines => lines;

    public List<ScoreData> highScores = new List<ScoreData>();
    private const int maxScores = 5;
    private const string saveKey = "HighScores";

    private int score;
    private int level = 1;
    private int lines;
    private int comboCount = -1;

    private int lastScore = -1;
    private int lastLevel = -1;
    private int lastLines = -1;

    private static readonly int[] lineClearScores = { 0, 100, 300, 500, 800 };

    public static event Action<ScoreEventData> OnScoreEarned;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadScores();
        }
        else Destroy(gameObject);
    }

    public void IncreaseLines(int linesToAdd, Vector3 pos)
    {
        if(linesToAdd == 0)
        {
            comboCount = -1;
            return;
        }

        score += lineClearScores[linesToAdd] * level;
        //Debug.Log("pos: " + pos);
        ScoreEventData data = new ScoreEventData(pos, GetScoreType(linesToAdd), lineClearScores[linesToAdd] * level);
        OnScoreEarned?.Invoke(data);
        lines += linesToAdd;
        level = (lines / 10) + 1;

        ComboCheck(pos);

        ChangeScore(score, true);
        ChangeLevel(level);
        ChangeLines(lines);

    }

    public void IncreaseLines(int scoreToAdd, int linesToAdd, Vector3 pos)
    {
        score += scoreToAdd;
        lines += linesToAdd;
        level = (lines / 10) + 1;

        ScoreEventData data;

        if (linesToAdd == 0)
        {
            ChangeScore(score, false);
            data = new ScoreEventData(pos, ScoreType.TSpin, scoreToAdd);
        }
        else
        {
            ChangeScore(score, false);
            ChangeLevel(level);
            ChangeLines(lines);

            ScoreType scoreType = ScoreType.None;

            switch (linesToAdd)
            {
                case 1:
                    scoreType = ScoreType.TSpinSingle;
                    break;
                case 2:
                    scoreType = ScoreType.TSpinDouble;
                    break;
                case 3:
                    scoreType = ScoreType.TSpinTriple;
                    break;
            }

            data = new ScoreEventData(pos, scoreType, scoreToAdd);
        }
        StartCoroutine(DelayedScoreEvent(data, 0.5f));
    }

    private void ComboCheck(Vector3 pos)
    {
        comboCount++;

        if (comboCount > 0)
        {
            int comboScore = comboCount * 50;

            ScoreEventData comboData = new ScoreEventData(pos, ScoreType.Combo, comboScore);
            StartCoroutine(DelayedScoreEvent(comboData, 0.5f));

            ChangeScore(comboScore, false);
        }
    }

    private IEnumerator DelayedScoreEvent(ScoreEventData data, float delay)
    {
        yield return new WaitForSeconds(delay);
        OnScoreEarned?.Invoke(data);
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

    public void AddNewScore(string name, int score)
    {
        highScores.Add(new ScoreData(name, score));
        highScores = highScores
            .OrderByDescending(s => s.score)
            .Take(maxScores)
            .ToList();
        SaveScores();
    }

    public List<ScoreData> GetHighScores()
    {
        return new List<ScoreData>(highScores);
    }

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(new ScoreList(highScores));
        PlayerPrefs.SetString(saveKey, json);
    }

    private void LoadScores()
    {
        if(PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            ScoreList loaded = JsonUtility.FromJson<ScoreList>(json);
            highScores = loaded.scores;
        }
    }

    [System.Serializable]
    private class ScoreList     // this if for serializing high score list to json
    {
        public List<ScoreData> scores;

        public ScoreList(List<ScoreData> list)
        {
            scores = list;
        }
    }
}


[System.Serializable]
public class ScoreData
{
    public string playerName;
    public int score;

    public ScoreData(string name, int score)
    {
        this.playerName = name;
        this.score = score;
    }
}