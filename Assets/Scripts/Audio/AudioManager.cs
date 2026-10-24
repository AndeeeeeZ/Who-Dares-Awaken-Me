using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set;  }

    [SerializeField]
    private AudioClip[] audioClips; 
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }

    public void PlayClip(string name)
    {
        AudioClip audioClip = Array.Find(audioClips, x => x.name == name);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
        else
        {
            Debug.LogError("The audio clip " + name + " is missing from audio manager"); 
        }
    }
}
