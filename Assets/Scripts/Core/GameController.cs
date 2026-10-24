using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool isHoldingBoard, isHoldingBar;
    public string nextSceneName;
    public GameObject player; 
    [SerializeField]
    private Tombstone[] Tombstones;
    public Door[] Doors;
    public Wall[] Walls; 

    private readonly List<Tombstone> trackedTombstones = new List<Tombstone>();

    public int TotalTombstones => trackedTombstones.Count;
    public int StandingTombstones { get; private set; }
    public event Action<int, int> TombstoneCountChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        TrackTombstones();
    }

    private void Start()
    {
        if (Instance != this)
            return;

        isHoldingBar = false;
        isHoldingBoard = false;
        RefreshTombstoneCount();
    }

    private void OnDestroy()
    {
        foreach (Tombstone tombstone in trackedTombstones)
        {
            if (tombstone != null)
                tombstone.DurabilityChanged -= OnTombstoneDurabilityChanged;
        }

        if (Instance == this)
            Instance = null;
    }

    private void TrackTombstones()
    {
        if (Tombstones == null)
            return;

        foreach (Tombstone tombstone in Tombstones)
        {
            if (tombstone == null)
            {
                Debug.LogWarning("GameController has a missing tombstone reference.", this);
                continue;
            }

            if (trackedTombstones.Contains(tombstone))
                continue;

            trackedTombstones.Add(tombstone);
            tombstone.DurabilityChanged += OnTombstoneDurabilityChanged;
        }
    }

    private void OnTombstoneDurabilityChanged(Tombstone tombstone)
    {
        RefreshTombstoneCount();
    }

    private void RefreshTombstoneCount()
    {
        int standing = 0;
        foreach (Tombstone tombstone in trackedTombstones)
        {
            if (tombstone != null && tombstone.GetCurrentDurability() > 0)
                standing++;
        }

        if (StandingTombstones == standing)
            return;

        StandingTombstones = standing;
        TombstoneCountChanged?.Invoke(StandingTombstones, TotalTombstones);
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
                if (Tombstones[j] != null && Tombstones[j].GetCurrentDurability() > 0)
                {
                    possibleLocations.Add(Tombstones[j].transform); 
                }
            }
            possibleLocations.Add(player.transform); 
        }


        int i = UnityEngine.Random.Range(0, possibleLocations.Count);
        return possibleLocations[i]; 
    }
}
