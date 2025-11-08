using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class IntroAnimationController : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private string videoFileName; 

    public UnityEvent OnVideoPlayerEnd; 

    private void Start()
    {
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName); 
        videoPlayer.url = videoPath; 
        videoPlayer.loopPointReached += OnVideoEnd; 
    }
    
    private void OnVideoEnd(VideoPlayer vp)
    {
        OnVideoPlayerEnd?.Invoke(); 
    }
}
