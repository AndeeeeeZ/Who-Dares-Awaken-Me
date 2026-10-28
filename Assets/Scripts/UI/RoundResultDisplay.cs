using UnityEngine;

public class RoundResultDisplay : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private GameObject victoryPanel;

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
        victoryPanel.SetActive(roundController.State == RoundController.GameState.Victory);
    }
}
