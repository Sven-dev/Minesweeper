using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    [Space]
    [Range(0, 1)] public float Volume = 1;
    [Range(0, 3)] public float Pitch = 1;
    [Space]
    public bool Loop = false;
    public bool PlayOnAwake = false;
    [Space]
    public AudioClip[] Clips = new AudioClip[1];

    [HideInInspector] public AudioSource Source;
    [HideInInspector] public SoundType Type;
    [HideInInspector] public float MaxVolume;

    [HideInInspector] public float LastClipIndex = -1;

    /// <summary>
    /// Creates a copy of an existing Sound with a unique name as an identifier
    /// </summary>
    public Sound(Sound original, string name)
    {
        Name = name;
        Clips = original.Clips;

        Volume = original.Volume;
        Pitch = original.Pitch;

        Loop = original.Loop;
        PlayOnAwake = original.PlayOnAwake;
    }

    /// <summary>
    /// Apply the Sound parameters to the AudioSource
    /// </summary>
    public void ApplyValues()
    {
        Source.volume = Volume;
        Source.pitch = Pitch;

        Source.loop = Loop;
        Source.playOnAwake = PlayOnAwake;
    }

    public void SetClip(AudioClip clip)
    {
        Clips[0] = clip;
        Source.clip = clip;
    }

    public void SetVolume(float value)
    {
        Volume = value;
        Source.volume = value;
    }
}

public enum SoundType
{
    Sound,
    Music,
    Dialogue
}