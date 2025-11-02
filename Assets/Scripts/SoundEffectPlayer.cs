using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip audioClip;

    [SerializeField]
    private float volumeOffset, pitchOffset;

    private AudioSource audioSource;

    private float originalVolume;
    private float originalPitch;
    private bool playing;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Missing AudioSource component");
        }

        originalVolume = audioSource.volume;
        originalPitch = audioSource.pitch;
        playing = false;
    }

    public void Play()
    {
        playing = true;
    }

    public void StopPlaying()
    {
        playing = false;
        audioSource.Stop(); 
    }


    public void PlayOneShot()
    {
        RandomizeAudioSource();
        audioSource.PlayOneShot(audioClip);
    }

    private void Update()
    {
        if (playing)
        {
            if (!audioSource.isPlaying)
            {
                PlayOneShot(); 
            }
        }
    }

    private void RandomizeAudioSource()
    {
        audioSource.volume = Random.Range(originalVolume - volumeOffset, originalVolume + volumeOffset);
        audioSource.pitch = Random.Range(originalPitch - pitchOffset, originalPitch + pitchOffset);
    }


}
