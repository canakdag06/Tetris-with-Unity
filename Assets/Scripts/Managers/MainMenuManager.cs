using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreBoardPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private ScoreRowUI[] scoreRows;

    public void PlayGame()
    {
        SceneManager.LoadScene("Tetris");
    }

    public void OpenPanel(GameObject panel, Action afterOpen = null)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => afterOpen?.Invoke());
    }

    public void ClosePanel(GameObject panel)
    {
        panel.transform.DOScale(Vector3.zero, 0.25f)
            .OnComplete(() => panel.SetActive(false));
    }

    public void OpenScoreBoard()
    {
        OpenPanel(scoreBoardPanel, LoadScoreBoard);
    }

    public void OpenSettings()
    {
        OpenPanel(settingsPanel);
    }

    public void CloseScoreBoard()
    {
        ClosePanel(scoreBoardPanel);
    }

    public void CloseSettings()
    {
        ClosePanel(settingsPanel);
    }

    public void LoadScoreBoard()
    {
        if (!PlayerPrefs.HasKey("HighScores")) return;

        string json = PlayerPrefs.GetString("HighScores");
        ScoreList loaded = JsonUtility.FromJson<ScoreList>(json);

        for (int i = 0; i < scoreRows.Length; i++)
        {
            if (i < loaded.scores.Count)
            {
                scoreRows[i].SetData(loaded.scores[i].playerName, loaded.scores[i].score);
            }
            else
            {
                scoreRows[i].SetData("---", 0);
            }
        }
    }
    
}
