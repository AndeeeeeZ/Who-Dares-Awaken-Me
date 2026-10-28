using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool isHoldingBoard, isHoldingBar;
    [SerializeField]
    private Tombstone[] Tombstones;
    [Header("Tombstone Starting Durability")]
    [SerializeField, Min(0)]
    private int startingBrokenTombstones = 2;
    [SerializeField, Min(1)]
    private int minimumStartingDurability = 25;
    [SerializeField, Min(1)]
    private int maximumStartingDurability = 75;
    public Door[] Doors;
    [Header("Door Starting State")]
    [SerializeField, Min(0)]
    private int startingOpenDoors = 1;
    public Wall[] Walls; 

    private readonly List<Tombstone> trackedTombstones = new List<Tombstone>();
    private bool initializingTombstones;

    public int TotalTombstones => trackedTombstones.Count;
    public int StandingTombstones { get; private set; }
    public int FullyRepairedTombstones { get; private set; }
    public bool TombstonesInitialized { get; private set; }
    public IReadOnlyList<Tombstone> TrackedTombstones => trackedTombstones;
    public IReadOnlyList<Door> ConfiguredDoors => Doors ?? Array.Empty<Door>();
    public IReadOnlyList<Wall> ConfiguredWalls => Walls ?? Array.Empty<Wall>();
    public event Action<int, int> TombstoneCountChanged;
    public event Action<int, int, int> TombstoneProgressChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        TrackTombstones();
        InitializeDoors();
    }

    private void Start()
    {
        if (Instance != this)
            return;

        isHoldingBar = false;
        isHoldingBoard = false;
        initializingTombstones = true;
        InitializeTombstones();
        initializingTombstones = false;
        TombstonesInitialized = true;
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

    private void InitializeDoors()
    {
        if (Doors == null)
            return;

        List<Door> shuffled = new List<Door>();
        foreach (Door door in Doors)
        {
            if (door == null)
            {
                Debug.LogWarning("GameController has a missing door reference.", this);
                continue;
            }

            if (!shuffled.Contains(door))
                shuffled.Add(door);
        }

        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);
            Door temporary = shuffled[i];
            shuffled[i] = shuffled[swapIndex];
            shuffled[swapIndex] = temporary;
        }

        int openCount = Mathf.Clamp(startingOpenDoors, 0, shuffled.Count);
        for (int i = 0; i < shuffled.Count; i++)
            shuffled[i].SetStartingOpenState(i < openCount);
    }

    private void OnTombstoneDurabilityChanged(Tombstone tombstone)
    {
        if (!initializingTombstones)
            RefreshTombstoneCount();
    }

    private void InitializeTombstones()
    {
        int total = trackedTombstones.Count;
        if (total == 0)
            return;

        int brokenCount = Mathf.Clamp(startingBrokenTombstones, 0, total);
        int minimum = Mathf.Max(1, Mathf.Min(minimumStartingDurability, maximumStartingDurability));
        int maximum = Mathf.Max(minimum, Mathf.Max(minimumStartingDurability, maximumStartingDurability));

        List<Tombstone> shuffled = new List<Tombstone>(trackedTombstones);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);
            Tombstone temporary = shuffled[i];
            shuffled[i] = shuffled[swapIndex];
            shuffled[swapIndex] = temporary;
        }

        for (int i = 0; i < shuffled.Count; i++)
        {
            Tombstone tombstone = shuffled[i];
            if (i < brokenCount || tombstone.MaxDurability <= 0)
            {
                tombstone.InitializeDurability(0);
                continue;
            }

            int lower = Mathf.Min(minimum, tombstone.MaxDurability);
            int upper = Mathf.Min(maximum, tombstone.MaxDurability);
            tombstone.InitializeDurability(UnityEngine.Random.Range(lower, upper + 1));
        }
    }

    private void RefreshTombstoneCount()
    {
        int standing = 0;
        int fullyRepaired = 0;
        foreach (Tombstone tombstone in trackedTombstones)
        {
            if (tombstone == null)
                continue;

            int durability = tombstone.GetCurrentDurability();
            if (durability > 0)
                standing++;
            if (tombstone.MaxDurability > 0 && durability >= tombstone.MaxDurability)
                fullyRepaired++;
        }

        if (StandingTombstones == standing && FullyRepairedTombstones == fullyRepaired)
            return;

        bool standingChanged = StandingTombstones != standing;
        StandingTombstones = standing;
        FullyRepairedTombstones = fullyRepaired;
        if (standingChanged)
            TombstoneCountChanged?.Invoke(StandingTombstones, TotalTombstones);
        TombstoneProgressChanged?.Invoke(StandingTombstones, FullyRepairedTombstones, TotalTombstones);
    }

    public HasDurability GetEnemyTarget()
    {
        HasDurability selected = null;
        int eligibleCount = 0;

        foreach (Tombstone tombstone in trackedTombstones)
        {
            if (tombstone == null || tombstone.GetCurrentDurability() <= 0)
                continue;

            eligibleCount++;
            if (UnityEngine.Random.Range(0, eligibleCount) == 0)
                selected = tombstone;
        }

        if (selected != null)
            return selected;

        eligibleCount = 0;
        if (Doors != null)
        {
            foreach (Door door in Doors)
            {
                if (door == null || !door.IsDefending)
                    continue;

                eligibleCount++;
                if (UnityEngine.Random.Range(0, eligibleCount) == 0)
                    selected = door;
            }
        }

        if (Walls != null)
        {
            foreach (Wall wall in Walls)
            {
                if (wall == null || !wall.IsDefending)
                    continue;

                eligibleCount++;
                if (UnityEngine.Random.Range(0, eligibleCount) == 0)
                    selected = wall;
            }
        }

        return selected;
    }

    public bool IsEnemyTargetActive(HasDurability target)
    {
        if (target == null)
            return false;

        if (target is Door door)
            return door.IsDefending && !HasStandingTombstones();

        if (target is Wall wall)
            return wall.IsDefending && !HasStandingTombstones();

        return target is Tombstone tombstone && tombstone.GetCurrentDurability() > 0;
    }

    private bool HasStandingTombstones()
    {
        foreach (Tombstone tombstone in trackedTombstones)
        {
            if (tombstone != null && tombstone.GetCurrentDurability() > 0)
                return true;
        }

        return false;
    }
}
