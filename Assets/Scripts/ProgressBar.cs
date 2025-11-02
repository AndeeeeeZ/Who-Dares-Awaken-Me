using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image baseLayer, displayLayer;

    // [SerializeField, Range(0f, 1f)]
    private float currentPercentage;

    private void Start()
    {
        currentPercentage = 0f;
        UpdateVisuals();
    }

    // Percentage between 0 to 1
    public void UpdateProgressBar(float percentage)
    {
        currentPercentage = Mathf.Clamp01(percentage);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        displayLayer.fillAmount = currentPercentage;
    }

    public void ShowProgressBar()
    {
        baseLayer.enabled = true;
        displayLayer.enabled = true;
    }
    public void HideProgressBar()
    {
        baseLayer.enabled = false;
        displayLayer.enabled = false;
    }

}
