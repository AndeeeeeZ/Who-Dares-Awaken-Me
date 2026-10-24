using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private string[] sceneNames;

    public void LoadSceneWithNum(int i)
    {
        SceneManager.LoadScene(sceneNames[i], LoadSceneMode.Single);
    }

    public void LoadSceneWithName(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }
}
