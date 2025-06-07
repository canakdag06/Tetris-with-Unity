using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;    // Instance neden büyük harfle baþlýyor?

    public SoundData soundData;
    public AudioSource sfxSource;
    public AudioSource musicSource;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(SoundType type)
    {
        var entry = soundData.Get(type);

        if (entry != null)
        {
            sfxSource.PlayOneShot(entry.clip, entry.volume);
        }
        else
        {
            Debug.Log($"SFX not found: {type}");
        }
    }

    public void PlayMusic(SoundType type)
    {
        var entry = soundData.Get(type);
        if (entry != null)
        {
            musicSource.clip = entry.clip;
            musicSource.volume = entry.volume;
            musicSource.Play();
        }
        else
        {
            Debug.Log($"Music not found: {type}");
        }
    }

}
