using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScorePanel : MonoBehaviour
{
    public ScoreRowUI[] scoreRows;

    public void ShowScores()
    {
        List<ScoreData> scores = ScoreManager.Instance.GetHighScores();

        for (int i = 0; i < scoreRows.Length; i++)
        {
            if (i < scores.Count)
            {
                scoreRows[i].SetData(scores[i].playerName, scores[i].score);
            }
            else
            {
                scoreRows[i].SetData("---", 0);
            }
        }
    }
}
