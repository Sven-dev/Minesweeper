using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    #region Fields
    public static AudioManager Instance;

    [Header("Volume controls")]
    [Range(0, 1)]
    [SerializeField] private float _MasterVolume = 0.5f;
    public float MasterVolume
    {
        get { return _MasterVolume; }
        set { _MasterVolume = value; OnValidate(); }
    }

    [Space]
    [Range(0, 1)]
    [SerializeField] private float _SoundVolume = 1;
    public float SoundVolume
    {
        get { return _SoundVolume; }
        set { _SoundVolume = value; OnValidate(); }
    }

    [Range(0, 1)]
    [SerializeField] private float _MusicVolume = 1;
    public float MusicVolume
    {
        get { return _MusicVolume; }
        set { _MusicVolume = value; OnValidate(); }
    }

    [Range(0, 1)]
    [SerializeField] private float _DialogueVolume = 1;
    public float DialogueVolume
    {
        get { return _DialogueVolume; }
        set { _DialogueVolume = value; OnValidate(); }
    }

    [Header("Sound effects")]
    [SerializeField] private List<Sound> Sounds;
    [Header("Music")]
    [SerializeField] private List<Sound> Music;
    [Header("Dialogue")]
    [SerializeField] private List<Sound> Dialogue;

    #region Private
    private bool Playing = false;

    private int serialNumber = 0;
    private Dictionary<string, Sound> Dictionary = new Dictionary<string, Sound>();

    private GameObject SoundContainer;
    private GameObject MusicContainer;
    private GameObject DialogueContainer;
    #endregion
    #endregion

    #region Initialization
    /// <summary>
    /// Gets called whenever a value gets updated in the Unity editor
    /// </summary>
    private void OnValidate()
    {
        // Updates the AudioSources to play at the updated settings when the editor is in play mode
        if (Playing)
        {
            foreach (Sound sound in Sounds)
            {              
                sound.Volume = MasterVolume * SoundVolume * sound.MaxVolume;
                sound.ApplyValues();
            }

            foreach (Sound sound in Music)
            {
                sound.Volume = MasterVolume * MusicVolume * sound.MaxVolume;
                sound.ApplyValues();
            }

            foreach (Sound sound in Dialogue)
            {
                sound.Volume = MasterVolume * DialogueVolume * sound.MaxVolume;
                sound.ApplyValues();
            }
        }
    }

    /// <summary>
    /// Gets called once when the game is started
    /// </summary>
    private void Awake()
    {
        //Create a singleton pattern that doesn't get destroyed on load
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //Initialize the sound lists
        InitializeSounds("Sounds", Sounds, SoundType.Sound, SoundVolume);
        InitializeSounds("Music", Music, SoundType.Music, MusicVolume);
        InitializeSounds("Dialogue", Dialogue, SoundType.Dialogue, DialogueVolume);

        Playing = true;
    }

    /// <summary>
    /// Creates a GameObject and starts initializing an array of Sounds
    /// Initializes a list of Sounds by creating a child GameObject, creating an AudioSource for every Sound
    /// </summary>
    private void InitializeSounds(string name, List<Sound> list, SoundType type, float volume)
    {
        //Create the container, and set its parent to the music manager
        GameObject container = new GameObject(name);
        container.transform.parent = transform;

        //Get a reference to the container based on the SoundType
        switch(type)
        {
            case SoundType.Sound:
                SoundContainer = container;
                break;
            case SoundType.Music:
                MusicContainer = container;
                break;
            case SoundType.Dialogue:
                DialogueContainer = container;
                break;
        }

        //Initialize every sound in the sound list
        foreach (Sound sound in list)
        {
            InitializeSound(container, sound, type, volume);
        }
    }

    /// <summary>
    /// Initializes a Sound by creating an AudioSource, setting the proper clip, and leveling the volume
    /// </summary>
    private void InitializeSound(GameObject container, Sound sound, SoundType type, float typeVolume)
    {
        Dictionary.Add(sound.Name, sound);

        sound.Type = type;
        sound.Source = container.AddComponent<AudioSource>();
        sound.Source.clip = sound.Clips[0];

        sound.MaxVolume = sound.Volume;
        sound.Volume = MasterVolume * typeVolume * sound.Volume;

        sound.ApplyValues();
        if (sound.PlayOnAwake)
        {
            sound.Source.Play();
        }
    }
    #endregion

    #region private functions

    /// <summary>
    /// Returns the Sound with the given name, or throws an exception if the sound isn't found
    /// </summary>
    private Sound GetSound(string name)
    {
        foreach (KeyValuePair<string, Sound> pair in Dictionary)
        {
            if (pair.Key == name)
            {
                return pair.Value;
            }
        }

        throw new Exception("Sound '" + name + "' not found. Did you spell it right?");
    }

    /// <summary>
    /// Returns the volume level field of the associated enum value of a sound
    /// </summary>
    private float GetVolumeDial(SoundType type)
    {
        switch (type)
        {
            case SoundType.Sound:
                return SoundVolume;
            case SoundType.Music:
                return MusicVolume;
            case SoundType.Dialogue:
                return DialogueVolume;
            default:
                throw new System.Exception("SoundType " + type.ToString() + "not implemented");
        }
    }

    #endregion

    /// <summary>
    /// Creates a copy of an existing Sound
    /// Useful if the game needs to play a sound effect multiple times and/or at once during runtime
    /// </summary>
    /// <returns>A string containing a unique name to play the sound effect</returns>
    public string CreateSound(string name)
    {
        //Create the new sound based on the original, but with a unique name
        Sound original = GetSound(name);
        Sound newSound = new Sound(original, original.Name + " " + serialNumber);
        serialNumber++;

        //Get the correct options based on the SoundType of the original
        float volume;
        GameObject container;

        switch (original.Type)
        {
            case SoundType.Sound:
                container = SoundContainer;
                Sounds.Add(newSound);
                volume = _SoundVolume;
                break;
            case SoundType.Music:
                container = MusicContainer;
                Music.Add(newSound);
                volume = _MusicVolume;
                break;
            case SoundType.Dialogue:
                container = DialogueContainer;
                Dialogue.Add(newSound);
                volume = _DialogueVolume;
                break;
            default:
                throw new Exception("Sound type " + original.Type + "not implemented");
        }

        //Initialize the new sound
        InitializeSound(container, newSound, original.Type, volume);

        //Because newSound gets instantiated during runtime, InitializeSound calculates the maximum volume based on the current volume, not the volume set at start, so newSound needs to copy the max volume of the original.
        newSound.MaxVolume = original.MaxVolume;
        SetVolume(newSound.Name, 1);

        //Return the unique name
        return newSound.Name;
    }

    /// <summary>
    /// Destroys the sound with the given name
    /// </summary>
    public void DestroySound(string name)
    {
        Sound sound = GetSound(name);

        //Remove the sound from the correct list, based on its soundtype
        switch (sound.Type)
        {
            case SoundType.Sound:
                Sounds.Remove(sound);
                break;
            case SoundType.Music:
                Music.Remove(sound);
                break;
            case SoundType.Dialogue:
                Dialogue.Remove(sound);
                break;
            default:
                throw new Exception("Sound type " + sound.Type + "not implemented");
        }

        Dictionary.Remove(name);
        Destroy(sound.Source);
    }

    /// <summary>
    /// Destroys the sound with the given name after a delay
    /// </summary>
    public void DestroySound(string name, float delay)
    {
        StartCoroutine(_DestroySound(name, delay));
    }

    private IEnumerator _DestroySound(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroySound(name);
    }
    //__________________________________________________________

    /// <summary>
    /// Plays a sound with the given name
    /// </summary>
    public void Play(string name)
    {
        Sound sound = GetSound(name);
        sound.Source.Play();
    }

    /// <summary>
    /// Plays the sound that corresponds to the index of the names Clips list
    /// </summary>
    public void Play(string name, int index)
    {
        Sound sound = GetSound(name);
        if (sound.Clips.Length > index)
        {
            sound.SetClip(sound.Clips[index]);
            sound.Source.Play();
        }
        else
        {
            throw new Exception("Failed to play " + name + " because sound list does not have " + index + " items.");
        }
    }

    /// <summary>
    /// Swaps out the AudioClip in name, and plays it
    /// </summary>
    public void Play(string name, AudioClip clip)
    {
        Sound sound = GetSound(name);
        sound.SetClip(clip);
        sound.Source.Play();
    }

    /// <summary>
    /// Loads an AudioClip from storage, and plays it on the AudioSource with the given name
    /// </summary>
    public void Play(string name, string path)
    {
        Sound sound = GetSound(name);
        sound.SetClip(Resources.Load<AudioClip>(path));
        sound.Source.Play();
    }

    /// <summary>
    /// Plays the sound effect with the given name after a certain amount of delay
    /// </summary>
    public void Play(string name, float delay)
    {
        Sound sound = GetSound(name);
        StartCoroutine(_Play(sound, delay));
    }

    private IEnumerator _Play(Sound sound, float delay)
    {
        yield return new WaitForSeconds(delay);
        sound.Source.Play();
    }
    //___________________________________________________________________________

    /// <summary>
    /// Returns whether or not the sound with the given name is playing
    /// </summary>
    public bool IsPlaying(string name)
    {
        Sound sound = GetSound(name);
        return sound.Source.isPlaying;
    }

    /// <summary>
    /// Randomly plays one of the Clips of the sound with the given name
    /// </summary>
    public void PlayRandom(string name)
    {
        Sound sound = GetSound(name);

        int random = UnityEngine.Random.Range(0, sound.Clips.Length);
        while (random == sound.LastClipIndex)
        {
            random = UnityEngine.Random.Range(0, sound.Clips.Length);
        }

        sound.LastClipIndex = random;
        sound.SetClip(sound.Clips[random]);
        sound.Source.Play();
    }

    /// <summary>
    /// Stops the sound effect with the given name
    /// </summary>
    public void Stop(string name)
    {
        Sound sound = GetSound(name);
        sound.Source.Stop();
    }

    /// <summary>
    /// Stops name after a certain amount of time
    /// </summary>
    public void Stop(string name, float time)
    {
        Sound sound = GetSound(name);
        StartCoroutine(_Stop(sound, time));
    }
    

    private IEnumerator _Stop(Sound sound, float time)
    {
        yield return new WaitForSeconds(time);
        sound.Source.Stop();
    }
    //

    /// <summary>
    /// Pauses the sound effect with the given name
    /// </summary>
    public void Pause(string name)
    {
        Sound sound = GetSound(name);
        sound.Source.Pause();
    }
        
    /// <summary>
    /// Unpauses the sound effect with the given name
    /// </summary>
    public void Unpause(string name)
    {
        Sound sound = GetSound(name);
        sound.Source.UnPause();
    }

    /// <summary>
    /// Sets the volume of the sound effect with the given name to a value between 0 and 1.
    /// Calculates the new volume level based on MaxVolume (which is the level the sound is set at on start), TypeVolume (which is the volume of the sound category), and MasterVolume (which is the main volume)
    /// </summary>
    public void SetVolume(string name, float to)
    {
        Sound sound = GetSound(name);
        float typeVolume = GetVolumeDial(sound.Type);
        to = Mathf.Clamp(to, 0, 1);

        sound.Volume = MasterVolume * typeVolume * sound.MaxVolume * to;
        sound.ApplyValues();
    }

    /// <summary>
    /// Changes the volume level of name from the current level to the given one over an amount of time
    /// </summary>
    public void SetVolume(string name, float to, float time)
    {
        Sound sound = GetSound(name);

        to = Mathf.Clamp(to, 0, 1);
        StartCoroutine(_SetVolume(sound, sound.Volume, to, time));
    }

    /// <summary>
    /// Changes the volume level of name from 0 to 1
    /// </summary>
    public void FadeIn(string name, float time)
    {
        Sound sound = GetSound(name);

        sound.Source.Play();
        StartCoroutine(_SetVolume(sound, 0, 1, time));
    }

    /// <summary>
    /// Changes the volume level of name from 1 to 0
    /// </summary>
    public void FadeOut(string name, float time)
    {
        Sound sound = GetSound(name);

        StartCoroutine(_SetVolume(sound, 1, 0, time));
        StartCoroutine(_Stop(sound, time));
    }

    private IEnumerator _SetVolume(Sound sound, float from, float to, float time)
    {
        float typeVolume = GetVolumeDial(sound.Type);     
        float start =  MasterVolume * typeVolume * sound.MaxVolume * from;
        float end = MasterVolume * typeVolume * sound.MaxVolume * to;

        float progress = 0;
        while (progress <= 1)
        {
            //continuously update the end value in case the user changes any volume settings while the volume is changing
            end = MasterVolume * typeVolume * sound.MaxVolume * to;

            sound.Volume = Mathf.Lerp(start, end, progress);
            sound.ApplyValues();

            progress += Time.deltaTime / time;
            yield return null;
        }

        if (end == 0)
        {
            sound.Source.Stop();
            sound.SetVolume(MasterVolume * typeVolume * sound.MaxVolume * 1);
        }
        else
        {
            sound.Volume = end;
        }

        sound.ApplyValues();
    }
    //

    /// <summary>
    /// Sets the pitch of the sound effect with the given name to a value between 0 and 3
    /// </summary>
    public void SetPitch(string name, float value)
    {
        Sound sound = GetSound(name);

        value = Mathf.Clamp(value, 0, 3);
        sound.Pitch = value;
        sound.Source.pitch = value;
    }

    /// <summary>
    /// Sets the pitch of the sound effect with the given name to a random value between 2 parameters, with the minimum and maximum values being 0 and 3
    /// </summary>
    public void RandomizePitch(string name, float min, float max)
    {
        float pitch = UnityEngine.Random.Range(min, max);

        Sound sound = GetSound(name);
        sound.Pitch = pitch;
        sound.Source.pitch = pitch;
    }

    /// <summary>
    /// Starts or stops looping the sound effect with the given name
    /// </summary>
    public void SetLoop(string name, bool value)
    {
        Sound sound = GetSound(name);
        sound.Source.loop = value;
    }
}