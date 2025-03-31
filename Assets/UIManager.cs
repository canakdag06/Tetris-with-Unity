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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        scoreAnim = scoreText.GetComponent<Animation>();
        levelAnim = levelText.GetComponent<Animation>();
        linesAnim = linesText.GetComponent<Animation>();
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
