using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    [SerializeField] private GameObject scoreBoardPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text confirmationText;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private ScoreRowUI[] scoreRows;

    private Action confirmationAction;
    private string[] actions = { "Move", "RotateCW", "RotateCCW", "HardDrop", "Hold", "Escape" };



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

        EventSystem.current.SetSelectedGameObject(null);
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

    public void ShowConfirmation(string message, Action onConfirm)
    {
        OpenPanel(dialoguePanel);
        confirmationText.text = message;

        confirmationAction = onConfirm;

        okButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        okButton.onClick.AddListener(() =>
        {
            confirmationAction?.Invoke();
            ClosePanel(dialoguePanel);
        });

        cancelButton.onClick.AddListener(() =>
        {
            ClosePanel(dialoguePanel);
        });
    }

    public void OnResetHighScores()
    {
        ShowConfirmation("Reset High Scores?", ResetHighScores);
    }

    public void OnResetKeyBindings()
    {
        ShowConfirmation("Reset Key Bindings?", ResetKeyBindings);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ResetHighScores()
    {
        PlayerPrefs.DeleteKey("HighScores");
        PlayerPrefs.Save();
    }


    private void ResetKeyBindings()
    {
        foreach (var action in actions)
        {
            for (int i = 0; i < 5; i++)
            {
                string key = action + "_binding_" + i;
                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
        }

        PlayerPrefs.Save();
    }
}
