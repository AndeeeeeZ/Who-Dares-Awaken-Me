using TMPro;
using UnityEngine;

public abstract class HasDurability : MonoBehaviour
{
    [SerializeField]
    private bool textDisplay, progressBarDisplay;

    [SerializeField, Min(0)]
    protected int maxDurability, currentDurability, startingDurability, durabilityDecliningRate;

    [SerializeField]
    protected float durabilityDeclinePeriod;
    [SerializeField]
    protected bool durabilityDeclines;

    [SerializeField]
    protected TextMeshProUGUI display;
    [SerializeField]
    protected ProgressBar progressBar; 

    private float timer;
    protected void OnStart()
    {
        currentDurability = startingDurability;
        timer = 0;

        if (!textDisplay)
            display.gameObject.SetActive(false);

        if (!progressBarDisplay)
            progressBar.HideProgressBar();

        UpdateDurabilityDisplay();
    }

    private void Update()
    {
        if (durabilityDeclines)
        {
            timer += Time.deltaTime;
            if (timer >= durabilityDeclinePeriod)
            {
                timer %= durabilityDeclinePeriod;
                currentDurability -= durabilityDecliningRate;
                currentDurability = Mathf.Clamp(currentDurability, 0, maxDurability);
                UpdateDurabilityDisplay();
                DurabilityUpdateCheck();
            }
        }
    }

    public void AddCurrentDurability(int amount)
    {
        currentDurability += amount;
        currentDurability = Mathf.Clamp(currentDurability, 0, maxDurability);
        UpdateDurabilityDisplay();
    }

    public int GetCurrentDurability()
    {
        return currentDurability;
    }

    protected float GetCurrentDurabilityPercentage()
    {
        return ((float)currentDurability / maxDurability) * 100f;
    }

    protected virtual int DurabilityUpdateCheck() { return -1; }

    protected void UpdateDurabilityDisplay()
    {
        if (textDisplay && display != null)
            display.text = currentDurability.ToString();
        if (progressBarDisplay && progressBar != null)
            progressBar.UpdateProgressBar((float) currentDurability / maxDurability); 
    }
}
