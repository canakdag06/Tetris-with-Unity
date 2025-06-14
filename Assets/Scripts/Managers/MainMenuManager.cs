using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreBoardPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private ScoreRowUI[] scoreRows;

    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        LoadVolumes();
    }

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
        LoadScoreBoard();
        OpenPanel(scoreBoardPanel);
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


    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.Instance.mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20f);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void LoadVolumes()
    {
        float saved = PlayerPrefs.GetFloat("MusicVolume", 1f);  // default: 1
        musicSlider.value = saved;
        AudioManager.Instance.mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(saved, 0.0001f, 1f)) * 20f);

        saved = PlayerPrefs.GetFloat("SFXVolume", 1f);  // default: 1
        sfxSlider.value = saved;
        AudioManager.Instance.mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(saved, 0.0001f, 1f)) * 20f);
    }
}
