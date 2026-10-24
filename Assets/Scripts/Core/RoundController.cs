using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class RoundController : MonoBehaviour
{
    public enum RoundPhase
    {
        WaitingToStart,
        Preparation,
        Wave,
        Victory,
        Defeat
    }

    [Serializable]
    private struct WaveBudget
    {
        [SerializeField, Min(0)] private int smallEnemies;
        [SerializeField, Min(0)] private int legEnemies;

        public int SmallEnemies => smallEnemies;
        public int LegEnemies => legEnemies;

        public WaveBudget(int smallEnemies, int legEnemies)
        {
            this.smallEnemies = smallEnemies;
            this.legEnemies = legEnemies;
        }
    }

    [Header("Scene References")]
    [SerializeField] private GameController gameController;
    [SerializeField] private EnemySpawner smallEnemySpawner;
    [SerializeField] private EnemySpawner legEnemySpawner;
    [SerializeField] private PlayerInteract playerInteract;

    [Header("Round Rules")]
    [SerializeField, Min(0f)] private float initialPreparationDuration = 60f;
    [FormerlySerializedAs("preparationDuration")]
    [SerializeField, Min(0f)] private float betweenWavePreparationDuration = 30f;
    [SerializeField, Min(1)] private int maxSimultaneousEnemies = 6;
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private WaveBudget[] waves =
    {
        new WaveBudget(4, 0),
        new WaveBudget(5, 1),
        new WaveBudget(7, 1),
        new WaveBudget(8, 2),
        new WaveBudget(10, 3)
    };

    private readonly HashSet<Enemy> activeEnemies = new HashSet<Enemy>();
    private int smallEnemiesLeftToSpawn;
    private int legEnemiesLeftToSpawn;
    private int remainingEnemies;
    private float smallSpawnTimer;
    private float legSpawnTimer;
    private float timeScaleBeforeResult;
    private bool pausedForResult;

    public RoundPhase Phase { get; private set; } = RoundPhase.WaitingToStart;
    public int CurrentWave { get; private set; }
    public int TotalWaves => waves == null ? 0 : waves.Length;
    public float PreparationTimeRemaining { get; private set; }
    public int RemainingEnemies => remainingEnemies;
    public int AliveEnemies => activeEnemies.Count;
    public IReadOnlyCollection<Enemy> ActiveEnemies => activeEnemies;

    public event Action<RoundPhase, int> PhaseChanged;
    public event Action<float> PreparationTimeChanged;
    public event Action<int> RemainingEnemiesChanged;
    public event Action<Enemy> EnemySpawned;
    public event Action<Enemy> EnemyRemoved;

    private void OnEnable()
    {
        if (gameController != null)
            gameController.TombstoneCountChanged += OnTombstoneCountChanged;

        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null)
                enemy.Resolved += OnEnemyResolved;
        }
    }

    private void Start()
    {
        if (gameController == null || smallEnemySpawner == null || legEnemySpawner == null ||
            playerInteract == null ||
            smallEnemySpawner == legEnemySpawner ||
            TotalWaves < 1 || maxSimultaneousEnemies < 1 ||
            !smallEnemySpawner.IsConfigured || !legEnemySpawner.IsConfigured)
        {
            Debug.LogError("RoundController needs a GameController, PlayerInteract, two configured enemy spawners, and at least one wave.", this);
            enabled = false;
        }
    }

    private void OnDisable()
    {
        if (gameController != null)
            gameController.TombstoneCountChanged -= OnTombstoneCountChanged;

        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null)
                enemy.Resolved -= OnEnemyResolved;
        }
    }

    private void OnDestroy()
    {
        RestoreTimeScale();
    }

    private void Update()
    {
        if (Phase == RoundPhase.WaitingToStart)
        {
            // GameController initializes the tombstones in Start, which may run after this component's Start.
            if (gameController.TombstonesInitialized)
            {
                if (gameController.StandingTombstones == 0)
                    EndRound(RoundPhase.Defeat);
                else
                    BeginPreparation();
            }
            return;
        }

        if (Phase == RoundPhase.Preparation)
        {
            PreparationTimeRemaining = Mathf.Max(0f, PreparationTimeRemaining - Time.deltaTime);
            PreparationTimeChanged?.Invoke(PreparationTimeRemaining);
            if (PreparationTimeRemaining <= 0f)
                BeginWave();
            return;
        }

        if (Phase == RoundPhase.Wave)
            UpdateWave(Time.deltaTime);
    }

    private void BeginPreparation()
    {
        CurrentWave++;
        PreparationTimeRemaining = CurrentWave == 1
            ? initialPreparationDuration
            : betweenWavePreparationDuration;
        Phase = RoundPhase.Preparation;
        PhaseChanged?.Invoke(Phase, CurrentWave);
        PreparationTimeChanged?.Invoke(PreparationTimeRemaining);

        if (PreparationTimeRemaining <= 0f)
            BeginWave();
    }

    private void BeginWave()
    {
        WaveBudget budget = waves[CurrentWave - 1];
        smallEnemiesLeftToSpawn = budget.SmallEnemies;
        legEnemiesLeftToSpawn = budget.LegEnemies;
        remainingEnemies = smallEnemiesLeftToSpawn + legEnemiesLeftToSpawn;
        smallSpawnTimer = 0f;
        legSpawnTimer = 0f;
        Phase = RoundPhase.Wave;
        PhaseChanged?.Invoke(Phase, CurrentWave);
        RemainingEnemiesChanged?.Invoke(remainingEnemies);
        CheckWaveComplete();
    }

    private void UpdateWave(float deltaTime)
    {
        smallSpawnTimer += deltaTime;
        legSpawnTimer += deltaTime;

        if (activeEnemies.Count < maxSimultaneousEnemies && smallEnemiesLeftToSpawn > 0 &&
            smallSpawnTimer >= smallEnemySpawner.SpawnInterval)
        {
            if (RegisterSpawn(smallEnemySpawner))
            {
                smallEnemiesLeftToSpawn--;
                smallSpawnTimer = 0f;
            }
        }

        if (activeEnemies.Count < maxSimultaneousEnemies && legEnemiesLeftToSpawn > 0 &&
            legSpawnTimer >= legEnemySpawner.SpawnInterval)
        {
            if (RegisterSpawn(legEnemySpawner))
            {
                legEnemiesLeftToSpawn--;
                legSpawnTimer = 0f;
            }
        }
    }

    private bool RegisterSpawn(EnemySpawner spawner)
    {
        Enemy enemy = spawner.SpawnEnemy(gameController);
        if (enemy == null)
            return false;

        activeEnemies.Add(enemy);
        enemy.Resolved += OnEnemyResolved;
        EnemySpawned?.Invoke(enemy);
        return true;
    }

    private void OnEnemyResolved(Enemy enemy)
    {
        if (!activeEnemies.Remove(enemy))
            return;

        enemy.Resolved -= OnEnemyResolved;
        EnemyRemoved?.Invoke(enemy);
        remainingEnemies--;
        RemainingEnemiesChanged?.Invoke(remainingEnemies);
        if (Phase == RoundPhase.Wave)
            CheckWaveComplete();
    }

    private void CheckWaveComplete()
    {
        if (Phase != RoundPhase.Wave || smallEnemiesLeftToSpawn != 0 ||
            legEnemiesLeftToSpawn != 0 || activeEnemies.Count != 0)
            return;

        if (CurrentWave >= TotalWaves)
            EndRound(RoundPhase.Victory);
        else
            BeginPreparation();
    }

    private void OnTombstoneCountChanged(int standing, int total)
    {
        if (gameController.TombstonesInitialized && standing == 0 && total > 0 &&
            Phase != RoundPhase.Victory && Phase != RoundPhase.Defeat)
            EndRound(RoundPhase.Defeat);
    }

    private void EndRound(RoundPhase result)
    {
        if (Phase == RoundPhase.Victory || Phase == RoundPhase.Defeat)
            return;

        Phase = result;
        playerInteract.enabled = false;
        timeScaleBeforeResult = Time.timeScale;
        pausedForResult = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PhaseChanged?.Invoke(Phase, CurrentWave);
    }

    public void RestartGame()
    {
        RestoreTimeScale();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void ReturnToTitle()
    {
        if (string.IsNullOrWhiteSpace(titleSceneName) ||
            !Application.CanStreamedLevelBeLoaded(titleSceneName))
        {
            Debug.LogError("RoundController needs a title scene name included in Build Settings.", this);
            return;
        }

        RestoreTimeScale();
        SceneManager.LoadScene(titleSceneName, LoadSceneMode.Single);
    }

    private void RestoreTimeScale()
    {
        if (!pausedForResult)
            return;

        Time.timeScale = timeScaleBeforeResult;
        pausedForResult = false;
    }
}
