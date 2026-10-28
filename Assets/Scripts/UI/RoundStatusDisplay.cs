using UnityEngine;

public class RoundStatusDisplay : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private TMPro.TextMeshProUGUI preparationTimeText;
    [SerializeField] private TMPro.TextMeshProUGUI remainingEnemiesText;

    private void OnEnable()
    {
        if (roundController == null || preparationTimeText == null || remainingEnemiesText == null)
        {
            Debug.LogError("RoundStatusDisplay needs the game controller and two TextMeshProUGUI labels.", this);
            enabled = false;
            return;
        }

        roundController.RepairProgressChanged += UpdateRepairProgress;
        roundController.AliveEnemiesChanged += UpdateAliveEnemies;
        preparationTimeText.enabled = true;
        remainingEnemiesText.enabled = true;
        UpdateRepairProgress(roundController.FullyRepairedTombstones, roundController.TotalTombstones);
        UpdateAliveEnemies(roundController.AliveEnemies);
    }

    private void OnDisable()
    {
        if (roundController == null)
            return;

        roundController.RepairProgressChanged -= UpdateRepairProgress;
        roundController.AliveEnemiesChanged -= UpdateAliveEnemies;
    }

    private void UpdateRepairProgress(int repaired, int total)
    {
        preparationTimeText.text = $"Fully repaired: {repaired}/{total}";
    }

    private void UpdateAliveEnemies(int alive)
    {
        remainingEnemiesText.text = $"Enemies active: {alive}";
    }
}
