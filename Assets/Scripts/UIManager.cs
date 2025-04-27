using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private TextMeshPro levelText;
    [SerializeField] private TextMeshPro linesText;

    private Animation scoreAnim;
    private Animation levelAnim;
    private Animation linesAnim;

    [SerializeField] private GameObject notificationPrefab;
    private TextMeshPro scoreTypeText;
    private TextMeshPro scoreAmountText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        scoreAnim = scoreText.GetComponent<Animation>();
        levelAnim = levelText.GetComponent<Animation>();
        linesAnim = linesText.GetComponent<Animation>();
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreEarned += HandleScoreEarned;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreEarned -= HandleScoreEarned;
    }

    private void HandleScoreEarned(ScoreEventData data)
    {
        GameObject notification = Instantiate(notificationPrefab);
        notification.transform.position = data.pos;
        scoreTypeText = notification.transform.GetChild(0).GetComponent<TextMeshPro>();
        scoreTypeText.text = data.scoreType.ToString();
        scoreAmountText = notification.transform.GetChild(1).GetComponent<TextMeshPro>();
        scoreAmountText.text = data.scoreAmount.ToString();
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
        levelAnim.Play();
    }

    public void UpdateLines(int lines)
    {
        linesText.text = lines.ToString();
        linesAnim.Play();
    }
}
