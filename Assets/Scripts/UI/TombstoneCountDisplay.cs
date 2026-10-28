using System.Collections.Generic;
using UnityEngine;

public class TombstoneCountDisplay : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TombstoneCountIcon[] icons;
    [SerializeField] private Sprite completeIcon;
    [SerializeField] private Sprite brokenIcon;
    [SerializeField] private Sprite completeOutline;
    [SerializeField] private Sprite brokenOutline;
    [SerializeField, Range(0f, 100f)] private float completeIconThresholdPercent = 50f;
    [SerializeField] private Color emptyColor = Color.red;
    [SerializeField] private Color fullColor = Color.green;

    private readonly List<Tombstone> sortedTombstones = new List<Tombstone>();

    private void Start()
    {
        if (gameController == null || icons == null)
        {
            Debug.LogError("TombstoneCountDisplay needs a GameController and an array of tombstone count icons.", this);
            return;
        }

        sortedTombstones.AddRange(gameController.TrackedTombstones);
        if (icons.Length < sortedTombstones.Count)
            Debug.LogError($"TombstoneCountDisplay has {icons.Length} icons for {sortedTombstones.Count} tombstones. Assign at least one icon per tombstone.", this);

        foreach (Tombstone tombstone in sortedTombstones)
            tombstone.DurabilityChanged += OnTombstoneDurabilityChanged;

        RefreshIcons();
    }

    private void OnDestroy()
    {
        foreach (Tombstone tombstone in sortedTombstones)
        {
            if (tombstone != null)
                tombstone.DurabilityChanged -= OnTombstoneDurabilityChanged;
        }
    }

    private void OnTombstoneDurabilityChanged(Tombstone tombstone)
    {
        RefreshIcons();
    }

    private void RefreshIcons()
    {
        sortedTombstones.Sort((a, b) => Health(b).CompareTo(Health(a)));

        for (int i = 0; i < icons.Length; i++)
        {
            TombstoneCountIcon icon = icons[i];
            if (icon == null || !icon.IsAssigned)
            {
                Debug.LogError($"TombstoneCountDisplay icon {i} needs both outline and sprite Images assigned.", this);
                continue;
            }

            bool used = i < sortedTombstones.Count;
            icon.gameObject.SetActive(used);
            if (!used)
                continue;

            float health = Health(sortedTombstones[i]);
            bool complete = health * 100f >= completeIconThresholdPercent;
            icon.SetAppearance(
                complete ? completeOutline : brokenOutline,
                complete ? completeIcon : brokenIcon,
                Color.Lerp(emptyColor, fullColor, health));
        }
    }

    private static float Health(Tombstone tombstone)
    {
        return tombstone.MaxDurability > 0
            ? Mathf.Clamp01((float)tombstone.GetCurrentDurability() / tombstone.MaxDurability)
            : 0f;
    }
}
