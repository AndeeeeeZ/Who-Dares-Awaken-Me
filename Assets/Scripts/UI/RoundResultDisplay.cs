using TMPro;
using UnityEngine;

public class RoundResultDisplay : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;

    private void OnEnable()
    {
        if (roundController == null || victoryPanel == null)
        {
            Debug.LogError("RoundResultDisplay needs a RoundController and victory panel.", this);
            enabled = false;
            return;
        }

        roundController.StateChanged += OnStateChanged;
        RefreshPanels();
    }

    private void OnDisable()
    {
        if (roundController != null)
            roundController.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged(RoundController.GameState state)
    {
        RefreshPanels();
    }

    private void RefreshPanels()
    {
        bool isVictory = roundController.State == RoundController.GameState.Victory;
        if (isVictory)
        {
            if (finalTimeText != null)
                finalTimeText.text = $"You finally went back to sleep \nafter {FormatTime(roundController.ElapsedPlayTime)} seconds.";

            if (bestTimeText != null)
            {
                bestTimeText.text = BestTimeRecord.TryGet(out float bestTimeSeconds)
                    ? $"The best time was {FormatTime(bestTimeSeconds)} seconds."
                    : "Best time unavailable.";
            }
        }

        victoryPanel.SetActive(isVictory);
    }

    private static string FormatTime(float seconds)
    {
        int hundredths = Mathf.FloorToInt(seconds * 100f);
        return $"{hundredths / 100:00}.{hundredths % 100:00}";
    }
}
