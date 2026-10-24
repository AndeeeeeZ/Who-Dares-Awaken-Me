using TMPro;
using UnityEngine;

public class TombstoneCountDisplay : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private TextMeshProUGUI countText;

    private void Start()
    {
        if (gameController == null || countText == null)
        {
            Debug.LogError("TombstoneCountDisplay needs a GameController and a TextMeshProUGUI label.", this);
            return;
        }

        gameController.TombstoneCountChanged += UpdateCount;
        UpdateCount(gameController.StandingTombstones, gameController.TotalTombstones);
    }

    private void OnDestroy()
    {
        if (gameController != null)
            gameController.TombstoneCountChanged -= UpdateCount;
    }

    private void UpdateCount(int standing, int total)
    {
        countText.text = $"tombstones: {standing}/{total}";
    }
}
