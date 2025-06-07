using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreBoardPanel;
    [SerializeField] private ScoreRowUI[] scoreRows;

    public void PlayGame()
    {
        SceneManager.LoadScene("Tetris");
    }

    public void OpenScoreBoard()
    {
        scoreBoardPanel.SetActive(true);
        scoreBoardPanel.transform.localScale = Vector3.zero;
        scoreBoardPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        LoadScoreBoard();
    }

    public void CloseScoreBoard()
    {
        scoreBoardPanel.transform.DOScale(Vector3.zero, 0.25f)
            .OnComplete(() => scoreBoardPanel.SetActive(false));
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
