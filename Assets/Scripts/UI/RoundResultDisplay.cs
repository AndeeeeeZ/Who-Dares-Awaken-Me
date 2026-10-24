using UnityEngine;

public class RoundResultDisplay : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    private void OnEnable()
    {
        if (roundController == null || victoryPanel == null || defeatPanel == null ||
            victoryPanel == defeatPanel)
        {
            Debug.LogError("RoundResultDisplay needs a RoundController and separate victory and defeat panels.", this);
            enabled = false;
            return;
        }

        roundController.PhaseChanged += OnPhaseChanged;
        RefreshPanels();
    }

    private void OnDisable()
    {
        if (roundController != null)
            roundController.PhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(RoundController.RoundPhase phase, int wave)
    {
        RefreshPanels();
    }

    private void RefreshPanels()
    {
        victoryPanel.SetActive(roundController.Phase == RoundController.RoundPhase.Victory);
        defeatPanel.SetActive(roundController.Phase == RoundController.RoundPhase.Defeat);
    }
}
