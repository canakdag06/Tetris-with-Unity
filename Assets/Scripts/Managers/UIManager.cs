using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Board board;

    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private TextMeshPro levelText;
    [SerializeField] private TextMeshPro linesText;

    private Animation scoreAnim;
    private Animation levelAnim;
    private Animation linesAnim;

    [SerializeField] private GameObject notificationPrefab;
    private TextMeshPro scoreTypeText;
    private TextMeshPro scoreAmountText;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject newHighScorePanel;
    [SerializeField] private GameObject exitGamePanel;
    [SerializeField] private TextMeshProUGUI newHighScoreTxt;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private ScoreRowUI[] scoreRows;

    private int finalScore;
    private bool isPaused = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        board = FindAnyObjectByType<Board>();

        scoreAnim = scoreText.GetComponent<Animation>();
        levelAnim = levelText.GetComponent<Animation>();
        linesAnim = linesText.GetComponent<Animation>();
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreEarned += HandleScoreEarned;
        Board.OnGameOver += HandleGameOver;
        InputReader.Instance.OnPause += TogglePause;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreEarned -= HandleScoreEarned;
        Board.OnGameOver -= HandleGameOver;
        InputReader.Instance.OnPause -= TogglePause;
    }

    private void HandleScoreEarned(ScoreEventData data)
    {
        GameObject notification = Instantiate(notificationPrefab);


        RectInt bounds = board.Bounds;

        int minX = bounds.xMin;
        int maxX = bounds.xMax;

        int minY = bounds.yMin;
        int maxY = bounds.yMax;

        Vector3 clampedPos = data.pos;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX + 2, maxX - 2);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY + 3, maxY - 3);
        notification.transform.position = clampedPos;

        scoreTypeText = notification.transform.GetChild(0).GetComponent<TextMeshPro>();
        scoreTypeText.text = data.scoreType.ToString();

        if (data.scoreType == ScoreType.Combo)
        {
            scoreTypeText.text += $" x{data.scoreAmount / 50}!";
        }

        scoreAmountText = notification.transform.GetChild(1).GetComponent<TextMeshPro>();
        scoreAmountText.text = "+" + data.scoreAmount.ToString();

        AudioManager.Instance.PlaySFX(SoundType.ComboNotification);
    }

    private void HandleGameOver(int finalScore)
    {
        List<ScoreData> list = ScoreManager.Instance.GetHighScores();
        this.finalScore = finalScore;
        int minHighScore = list.Count < 5 ? 0 : list.Last().score;

        if (finalScore > minHighScore)
        {
            newHighScoreTxt.text = finalScore.ToString();
            newHighScorePanel.SetActive(true);
            nameInputField.text = "";
            nameInputField.ActivateInputField();
            AudioManager.Instance.PlaySFX(SoundType.NewHighScore);
        }
        else
        {
            ShowGameOverPanel();
            AudioManager.Instance.PlaySFX(SoundType.GameOver);
        }
    }

    public void OnSubmit()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrWhiteSpace(playerName)) return;

        ScoreManager.Instance.AddNewScore(playerName, finalScore);

        newHighScorePanel.SetActive(false);
        ShowGameOverPanel();
    }

    public void UpdateScore(int score, bool isAnimated = false)
    {
        scoreText.text = score.ToString();

        if (isAnimated)
        {
            scoreAnim.Play();
        }
    }

    public void UpdateLevel(int level)
    {
        levelText.text = level.ToString();
        if (level == 1)
            return;
        levelAnim.Play();
        AudioManager.Instance.PlaySFX(SoundType.LevelUp);
    }

    public void UpdateLines(int lines)
    {
        linesText.text = lines.ToString();
        if (lines == 0)
            return;
        linesAnim.Play();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Tetris");
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        gameOverPanel.transform.localScale = Vector3.zero;
        gameOverPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        ShowHighScores();
    }

    private void ShowHighScores()
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

    private void ShowExitGamePanel()
    {
        exitGamePanel.SetActive(true);
        exitGamePanel.transform.localScale = Vector3.zero;
        exitGamePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)
        .SetUpdate(true);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            InputReader.Instance.InputActions.Disable();
            Time.timeScale = 0f;
            ShowExitGamePanel();
        }
        else
        {
            InputReader.Instance.InputActions.Enable();
            Time.timeScale = 1f;
            exitGamePanel.SetActive(false);
        }
    }

    public void ConfirmExit()
    {
        InputReader.Instance.InputActions.Enable();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void CancelExit()
    {
        TogglePause();
        exitGamePanel.transform.DOScale(Vector3.zero, 0.25f)
            .OnComplete(() => exitGamePanel.SetActive(false)).SetUpdate(true);
    }
}
