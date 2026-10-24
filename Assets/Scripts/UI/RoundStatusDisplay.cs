using UnityEngine;

public class RoundStatusDisplay : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private TMPro.TextMeshProUGUI preparationTimeText;
    [SerializeField] private TMPro.TextMeshProUGUI remainingEnemiesText;

    private int displayedSeconds = -1;

    private void OnEnable()
    {
        if (roundController == null || preparationTimeText == null || remainingEnemiesText == null)
        {
            Debug.LogError("RoundStatusDisplay needs a RoundController and two TextMeshProUGUI labels.", this);
            enabled = false;
            return;
        }

        roundController.PhaseChanged += OnPhaseChanged;
        roundController.PreparationTimeChanged += UpdatePreparationTime;
        roundController.RemainingEnemiesChanged += UpdateRemainingEnemies;
        RefreshDisplay();
    }

    private void OnDisable()
    {
        if (roundController == null)
            return;

        roundController.PhaseChanged -= OnPhaseChanged;
        roundController.PreparationTimeChanged -= UpdatePreparationTime;
        roundController.RemainingEnemiesChanged -= UpdateRemainingEnemies;
    }

    private void OnPhaseChanged(RoundController.RoundPhase phase, int wave)
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        bool preparing = roundController.Phase == RoundController.RoundPhase.Preparation;
        bool inWave = roundController.Phase == RoundController.RoundPhase.Wave;

        preparationTimeText.enabled = preparing;
        remainingEnemiesText.enabled = inWave;

        if (preparing)
        {
            displayedSeconds = -1;
            UpdatePreparationTime(roundController.PreparationTimeRemaining);
        }

        if (inWave)
            UpdateRemainingEnemies(roundController.RemainingEnemies);
    }

    private void UpdatePreparationTime(float remainingTime)
    {
        if (roundController.Phase != RoundController.RoundPhase.Preparation)
            return;

        int seconds = Mathf.CeilToInt(Mathf.Max(0f, remainingTime));
        if (seconds == displayedSeconds)
            return;

        displayedSeconds = seconds;
        preparationTimeText.text = $"Preparation: {seconds / 60}:{seconds % 60:00}";
    }

    private void UpdateRemainingEnemies(int remaining)
    {
        remainingEnemiesText.text = $"Enemies remaining: {remaining}";
    }
}
