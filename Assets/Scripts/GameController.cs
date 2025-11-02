using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool isHoldingBoard, isHoldingBar;
    public string nextSceneName; 
    public GameObject[] Tombstones;
    public GameObject[] Doors;
    public GameObject[] Walls; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        isHoldingBar = false;
        isHoldingBar = false;
    }
    
    public void ToNextScene()
    {
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single); 
    }
}
