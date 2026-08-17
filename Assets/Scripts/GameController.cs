using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool isHoldingBoard, isHoldingBar;
    public string nextSceneName;
    public GameObject player; 
    public GameObject[] Tombstones;
    public Door[] Doors;
    public Wall[] Walls; 

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
    
    public Transform GetEnemyTarget()
    {
        List<Transform> possibleLocations = new List<Transform>();
        if (Doors[0].locked)
            possibleLocations.Add(Doors[0].transform);
        if (Walls[0].boardPlaced)
            possibleLocations.Add(Walls[0].transform); 
        if (possibleLocations.Count < 2)
        {
            for (int j = 0; j < Tombstones.Length; j++)
            {
                if (Tombstones[j].GetComponent<Tombstone>()?.GetCurrentDurability() > 0f)
                {
                    possibleLocations.Add(Tombstones[j].transform); 
                }
            }
            possibleLocations.Add(player.transform); 
        }


        int i = Random.Range(0, possibleLocations.Count);
        return possibleLocations[i]; 
    }
}
