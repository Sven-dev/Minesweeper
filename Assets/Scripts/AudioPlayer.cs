using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private string Name;

    public void Play()
    {
        AudioManager.Instance.Play(Name);
    }

    public void PlayRandom()
    {
        AudioManager.Instance.PlayRandom(Name);
    }
}
