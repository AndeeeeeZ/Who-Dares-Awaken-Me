using TMPro;
using UnityEngine;

public class WinTimeDisplay : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private TextMeshProUGUI timeText;

    private int displayedHundredths = -1;

    private void OnEnable()
    {
        if (roundController == null || timeText == null)
        {
            Debug.LogError("WinTimeDisplay needs a RoundController and a TextMeshProUGUI label.", this);
            enabled = false;
            return;
        }

        roundController.StateChanged += OnStateChanged;
        RefreshTime();
    }

    private void OnDisable()
    {
        if (roundController != null)
            roundController.StateChanged -= OnStateChanged;
    }

    private void Update()
    {
        if (roundController.State == RoundController.GameState.Playing)
            RefreshTime();
    }

    private void OnStateChanged(RoundController.GameState state)
    {
        RefreshTime();
    }

    private void RefreshTime()
    {
        int elapsedHundredths = Mathf.FloorToInt(roundController.ElapsedPlayTime * 100f);
        if (elapsedHundredths == displayedHundredths)
            return;

        displayedHundredths = elapsedHundredths;
        timeText.text = $"{elapsedHundredths / 100:00}.{elapsedHundredths % 100:00}";
    }
}
