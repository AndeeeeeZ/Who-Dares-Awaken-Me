using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// The class name is retained because GameScene and its buttons reference this component.
public class RoundController : MonoBehaviour
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        Victory
    }

    [Header("Scene References")]
    [SerializeField] private GameController gameController;
    [SerializeField] private EnemySpawner smallEnemySpawner;
    [SerializeField] private EnemySpawner legEnemySpawner;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraMovement cameraMovement;

    [Header("Enemy Spawning")]
    [SerializeField, Min(0.01f)] private float smallEnemySpawnInterval = 5f;
    [SerializeField, Min(0.01f)] private float legEnemySpawnInterval = 1f;
    [SerializeField, Min(1)] private int maxSimultaneousEnemies = 6;
    [SerializeField] private string titleSceneName = "Title";

    private readonly HashSet<Enemy> activeEnemies = new HashSet<Enemy>();
    private float smallSpawnTimer;
    private float legSpawnTimer;
    private float timeScaleBeforeResult;
    private bool pausedForResult;

    public GameState State { get; private set; } = GameState.WaitingToStart;
    public int FullyRepairedTombstones => gameController != null ? gameController.FullyRepairedTombstones : 0;
    public int TotalTombstones => gameController != null ? gameController.TotalTombstones : 0;
    public int AliveEnemies => activeEnemies.Count;
    public IReadOnlyCollection<Enemy> ActiveEnemies => activeEnemies;

    public event Action<GameState> StateChanged;
    public event Action<int, int> RepairProgressChanged;
    public event Action<int> AliveEnemiesChanged;
    public event Action<Enemy> EnemySpawned;
    public event Action<Enemy> EnemyRemoved;

    private void OnEnable()
    {
        if (gameController != null)
            gameController.TombstoneProgressChanged += OnTombstoneProgressChanged;

        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null)
                enemy.Resolved += OnEnemyResolved;
        }
    }

    private void Start()
    {
        if (gameController == null || gameController.TotalTombstones == 0 ||
            smallEnemySpawner == null || legEnemySpawner == null ||
            playerInteract == null || playerMovement == null || cameraMovement == null ||
            smallEnemySpawner == legEnemySpawner ||
            maxSimultaneousEnemies < 1 ||
            !smallEnemySpawner.IsConfigured || !legEnemySpawner.IsConfigured)
        {
            Debug.LogError("Game controller needs tombstones, player interaction, movement, camera movement, and two configured enemy spawners.", this);
            enabled = false;
        }
    }

    private void OnDisable()
    {
        if (gameController != null)
            gameController.TombstoneProgressChanged -= OnTombstoneProgressChanged;

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
        if (State == GameState.WaitingToStart)
        {
            // GameController initializes tombstones in Start, which may run after this Start.
            if (!gameController.TombstonesInitialized)
                return;

            TryWin(gameController.FullyRepairedTombstones, gameController.TotalTombstones);
            if (State != GameState.WaitingToStart)
                return;

            State = GameState.Playing;
            StateChanged?.Invoke(State);
        }

        if (State != GameState.Playing)
            return;

        smallSpawnTimer += Time.deltaTime;
        legSpawnTimer += Time.deltaTime;

        if (activeEnemies.Count < maxSimultaneousEnemies &&
            smallSpawnTimer >= Mathf.Max(0.01f, smallEnemySpawnInterval) &&
            RegisterSpawn(smallEnemySpawner))
            smallSpawnTimer = 0f;

        if (activeEnemies.Count < maxSimultaneousEnemies &&
            legSpawnTimer >= Mathf.Max(0.01f, legEnemySpawnInterval) &&
            RegisterSpawn(legEnemySpawner))
            legSpawnTimer = 0f;
    }

    private bool RegisterSpawn(EnemySpawner spawner)
    {
        Enemy enemy = spawner.SpawnEnemy(gameController);
        if (enemy == null)
            return false;

        if (!activeEnemies.Add(enemy))
            return false;

        enemy.Resolved += OnEnemyResolved;
        EnemySpawned?.Invoke(enemy);
        AliveEnemiesChanged?.Invoke(activeEnemies.Count);
        return true;
    }

    private void OnEnemyResolved(Enemy enemy)
    {
        if (!activeEnemies.Remove(enemy))
            return;

        enemy.Resolved -= OnEnemyResolved;
        EnemyRemoved?.Invoke(enemy);
        AliveEnemiesChanged?.Invoke(activeEnemies.Count);
    }

    private void OnTombstoneProgressChanged(int _, int fullyRepaired, int total)
    {
        RepairProgressChanged?.Invoke(fullyRepaired, total);
        if (gameController.TombstonesInitialized)
            TryWin(fullyRepaired, total);
    }

    private void TryWin(int fullyRepaired, int total)
    {
        if (State == GameState.Victory || total == 0 || fullyRepaired != total)
            return;

        WinGame();
    }

    private void WinGame()
    {
        if (playerMovement == null || cameraMovement == null)
        {
            Debug.LogError("RoundController needs PlayerMovement and CameraMovement assigned.", this);
            enabled = false;
            return;
        }

        State = GameState.Victory;
        playerInteract.enabled = false;
        playerMovement.enabled = false;
        cameraMovement.enabled = false;
        timeScaleBeforeResult = Time.timeScale;
        pausedForResult = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StateChanged?.Invoke(State);
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
            Debug.LogError("Game controller needs a title scene name included in Build Settings.", this);
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
