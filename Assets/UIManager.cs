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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateLevel(int level)
    {
        levelText.text = level.ToString();
    }

    public void UpdateLines(int lines)
    {
        linesText.text = lines.ToString();
    }
}
