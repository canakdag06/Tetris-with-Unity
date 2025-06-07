using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    public List<SoundEntry> sounds;

    public SoundEntry Get(SoundType type)
    {
        return sounds.Find(x => x.type == type);
    }
}
