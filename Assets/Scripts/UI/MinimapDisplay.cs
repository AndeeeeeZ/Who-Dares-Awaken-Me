using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapDisplay : MonoBehaviour
{
    private sealed class TombstoneMarker
    {
        public Tombstone Tombstone;
        public Image Image;
        public int LastDurability;
        public float DamageFlashUntil;
    }

    private sealed class DefenseMarker
    {
        public HasDurability Defense;
        public Image Image;
    }

    [Header("Map and scene references")]
    [SerializeField] private Camera mapCamera;
    [SerializeField] private RectTransform mapArea;
    [SerializeField] private RectTransform playerMarker;
    [SerializeField] private Transform player;
    [SerializeField] private GameController gameController;
    [SerializeField] private RoundController roundController;

    [Header("Production spots")]
    [SerializeField] private ProductionSpot woodProductionSpot;
    [SerializeField] private Transform woodMarkerPosition;
    [SerializeField] private ProductionSpot chainProductionSpot;
    [SerializeField] private Transform chainMarkerPosition;

    [Header("Marker appearance")]
    [SerializeField] private Image markerPrefab;
    [SerializeField] private Sprite tombstoneIcon;
    [SerializeField] private Sprite brokenTombstoneIcon;
    [SerializeField, Range(0f, 100f)] private float brokenTombstoneIconThresholdPercent = 50f;
    [SerializeField] private Sprite doorIcon;
    [SerializeField] private Sprite wallIcon;
    [SerializeField] private Sprite enemyIcon;
    [SerializeField] private Sprite woodProductionIcon;
    [SerializeField] private Sprite chainProductionIcon;
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color damagedColor = Color.red;
    [SerializeField] private Color brokenColor = Color.gray;
    [SerializeField] private Color securedColor = Color.cyan;
    [SerializeField] private Color unsecuredColor = Color.gray;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private Color damageFlashColor = Color.white;
    [SerializeField] private Color attackPulseColor = Color.yellow;

    [Header("Attack alert")]
    [SerializeField, Min(0f)] private float attackDistance = 8f;
    [SerializeField, Min(0f)] private float pulseSpeed = 5f;
    [SerializeField, Min(0f)] private float damageFlashDuration = 0.5f;

    private readonly List<TombstoneMarker> tombstoneMarkers = new List<TombstoneMarker>();
    private readonly List<DefenseMarker> defenseMarkers = new List<DefenseMarker>();
    private readonly Dictionary<Enemy, Image> enemyMarkers = new Dictionary<Enemy, Image>();
    private Image woodProductionMarker;
    private Image chainProductionMarker;
    private bool hasStarted;

    private void OnEnable()
    {
        if (hasStarted)
            InitializeMarkers();
    }

    private void Start()
    {
        hasStarted = true;
        InitializeMarkers();
    }

    private void InitializeMarkers()
    {
        if (mapCamera == null || mapArea == null || playerMarker == null || player == null)
        {
            Debug.LogError("MinimapDisplay needs its camera, map area, player marker, and player assigned.", this);
            enabled = false;
            return;
        }

        if (!mapCamera.orthographic)
        {
            Debug.LogError("MinimapDisplay needs an orthographic overhead camera.", this);
            enabled = false;
            return;
        }

        if (playerMarker.parent != mapArea)
        {
            Debug.LogError("MinimapDisplay needs the player marker directly under the map area.", this);
            enabled = false;
            return;
        }

        playerMarker.anchorMin = Vector2.zero;
        playerMarker.anchorMax = Vector2.zero;

        if (gameController == null || roundController == null || markerPrefab == null)
        {
            Debug.LogWarning("MinimapDisplay needs GameController, RoundController, and Marker Prefab for tombstone, defense, and enemy icons. The player marker will still update.", this);
            return;
        }

        BuildStaticMarkers();
        woodProductionMarker = CreateProductionMarker(woodProductionSpot, woodMarkerPosition, woodProductionIcon, "wood");
        chainProductionMarker = CreateProductionMarker(chainProductionSpot, chainMarkerPosition, chainProductionIcon, "chain");

        roundController.EnemySpawned += AddEnemy;
        roundController.EnemyRemoved += RemoveEnemy;
        foreach (Enemy enemy in roundController.ActiveEnemies)
            AddEnemy(enemy);
    }

    private void OnDisable()
    {
        if (roundController != null)
        {
            roundController.EnemySpawned -= AddEnemy;
            roundController.EnemyRemoved -= RemoveEnemy;
        }

        foreach (TombstoneMarker marker in tombstoneMarkers)
        {
            if (marker.Tombstone != null)
                marker.Tombstone.DurabilityChanged -= OnTombstoneDurabilityChanged;
            DestroyMarker(marker.Image);
        }
        tombstoneMarkers.Clear();

        foreach (DefenseMarker marker in defenseMarkers)
            DestroyMarker(marker.Image);
        defenseMarkers.Clear();

        foreach (Image marker in enemyMarkers.Values)
            DestroyMarker(marker);
        enemyMarkers.Clear();

        DestroyMarker(woodProductionMarker);
        DestroyMarker(chainProductionMarker);
        woodProductionMarker = null;
        chainProductionMarker = null;
    }

    private void LateUpdate()
    {
        PlaceMarker(playerMarker, player.position);
        Vector3 facing = player.forward;
        float right = Vector3.Dot(facing, mapCamera.transform.right);
        float up = Vector3.Dot(facing, mapCamera.transform.up);
        playerMarker.localRotation = Quaternion.Euler(0f, 0f, -Mathf.Atan2(right, up) * Mathf.Rad2Deg);

        foreach (TombstoneMarker marker in tombstoneMarkers)
        {
            if (marker.Tombstone == null)
                continue;

            PlaceMarker(marker.Image.rectTransform, marker.Tombstone.transform.position);
            float health = marker.Tombstone.MaxDurability > 0
                ? (float)marker.Tombstone.GetCurrentDurability() / marker.Tombstone.MaxDurability
                : 0f;
            Sprite icon = brokenTombstoneIcon != null &&
                          (marker.LastDurability <= 0 || health * 100f < brokenTombstoneIconThresholdPercent)
                ? brokenTombstoneIcon
                : tombstoneIcon;
            if (icon != null && marker.Image.sprite != icon)
                marker.Image.sprite = icon;
            Color color = marker.LastDurability > 0
                ? Color.Lerp(damagedColor, healthyColor, health)
                : brokenColor;

            if (Time.unscaledTime < marker.DamageFlashUntil)
                color = damageFlashColor;
            else if (marker.LastDurability > 0 && IsUnderAttack(marker.Tombstone))
                color = Color.Lerp(color, attackPulseColor,
                    0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * pulseSpeed));

            marker.Image.color = color;
        }

        foreach (DefenseMarker marker in defenseMarkers)
        {
            if (marker.Defense == null)
                continue;

            PlaceMarker(marker.Image.rectTransform, marker.Defense.transform.position);
            bool active = marker.Defense is Door door && door.IsDefending ||
                          marker.Defense is Wall wall && wall.IsDefending;
            marker.Image.color = active ? securedColor : unsecuredColor;
        }

        foreach (KeyValuePair<Enemy, Image> marker in enemyMarkers)
        {
            if (marker.Key != null)
                PlaceMarker(marker.Value.rectTransform, marker.Key.transform.position);
        }

        UpdateProductionMarker(woodProductionMarker, woodProductionSpot, woodMarkerPosition);
        UpdateProductionMarker(chainProductionMarker, chainProductionSpot, chainMarkerPosition);
    }

    private Image CreateProductionMarker(ProductionSpot spot, Transform position, Sprite icon, string resource)
    {
        if (spot == null && position == null)
            return null;

        if (spot == null || position == null)
        {
            Debug.LogWarning($"MinimapDisplay needs both the {resource} production spot and marker position assigned.", this);
            return null;
        }

        return CreateMarker(icon);
    }

    private void UpdateProductionMarker(Image marker, ProductionSpot spot, Transform position)
    {
        if (marker == null)
            return;

        if (spot == null || position == null || !spot.gameObject.activeInHierarchy)
        {
            marker.gameObject.SetActive(false);
            return;
        }

        PlaceMarker(marker.rectTransform, position.position);
    }

    private void BuildStaticMarkers()
    {
        foreach (Tombstone tombstone in gameController.TrackedTombstones)
        {
            if (tombstone == null)
            {
                Debug.LogWarning("MinimapDisplay has an unassigned tombstone.", this);
                continue;
            }

            TombstoneMarker marker = new TombstoneMarker
            {
                Tombstone = tombstone,
                Image = CreateMarker(tombstoneIcon),
                LastDurability = tombstone.GetCurrentDurability()
            };
            tombstoneMarkers.Add(marker);
            tombstone.DurabilityChanged += OnTombstoneDurabilityChanged;
        }

        foreach (Door door in gameController.ConfiguredDoors)
        {
            if (door != null)
                defenseMarkers.Add(new DefenseMarker { Defense = door, Image = CreateMarker(doorIcon) });
        }

        foreach (Wall wall in gameController.ConfiguredWalls)
        {
            if (wall != null)
                defenseMarkers.Add(new DefenseMarker { Defense = wall, Image = CreateMarker(wallIcon) });
        }
    }

    private void OnTombstoneDurabilityChanged(Tombstone tombstone)
    {
        foreach (TombstoneMarker marker in tombstoneMarkers)
        {
            if (marker.Tombstone != tombstone)
                continue;

            int durability = tombstone.GetCurrentDurability();
            if (durability < marker.LastDurability &&
                roundController.State == RoundController.GameState.Playing)
                marker.DamageFlashUntil = Time.unscaledTime + damageFlashDuration;
            marker.LastDurability = durability;
            return;
        }
    }

    private bool IsUnderAttack(Tombstone tombstone)
    {
        float distanceSquared = attackDistance * attackDistance;
        Vector3 position = tombstone.transform.position;
        foreach (Enemy enemy in enemyMarkers.Keys)
        {
            if (enemy == null || enemy.CurrentTarget != tombstone)
                continue;

            Vector3 offset = enemy.transform.position - position;
            offset.y = 0f;
            if (offset.sqrMagnitude <= distanceSquared)
                return true;
        }
        return false;
    }

    private void AddEnemy(Enemy enemy)
    {
        if (enemy == null || enemyMarkers.ContainsKey(enemy))
            return;

        Image marker = CreateMarker(enemyIcon);
        marker.color = enemyColor;
        enemyMarkers.Add(enemy, marker);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        if (!enemyMarkers.TryGetValue(enemy, out Image marker))
            return;

        enemyMarkers.Remove(enemy);
        DestroyMarker(marker);
    }

    private Image CreateMarker(Sprite icon)
    {
        Image marker = Instantiate(markerPrefab, mapArea);
        marker.gameObject.SetActive(true);
        marker.raycastTarget = false;
        if (icon != null)
            marker.sprite = icon;
        marker.rectTransform.anchorMin = Vector2.zero;
        marker.rectTransform.anchorMax = Vector2.zero;
        return marker;
    }

    private void PlaceMarker(RectTransform marker, Vector3 worldPosition)
    {
        Vector3 viewport = mapCamera.WorldToViewportPoint(worldPosition);
        bool visible = viewport.z > 0f && viewport.x >= 0f && viewport.x <= 1f &&
                       viewport.y >= 0f && viewport.y <= 1f;
        if (marker.gameObject.activeSelf != visible)
            marker.gameObject.SetActive(visible);
        if (visible)
            marker.anchoredPosition = new Vector2(viewport.x * mapArea.rect.width,
                                                   viewport.y * mapArea.rect.height);
    }

    private static void DestroyMarker(Image marker)
    {
        if (marker == null)
            return;

        marker.gameObject.SetActive(false);
        Destroy(marker.gameObject);
    }
}
