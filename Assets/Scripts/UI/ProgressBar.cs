using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image baseLayer, displayLayer;

    // [SerializeField, Range(0f, 1f)]
    private float currentPercentage;
    private bool useFillColorGradient;
    private Color emptyColor, fullColor;
    private Transform worldSpaceCanvas, mainCamera;

    private void Start()
    {
        UpdateVisuals();
    }

    private void LateUpdate()
    {
        if (worldSpaceCanvas == null)
            return;

        if (mainCamera == null)
        {
            Camera camera = Camera.main;
            if (camera == null)
                return;
            mainCamera = camera.transform;
        }

        worldSpaceCanvas.rotation = mainCamera.rotation;
    }

    // Percentage between 0 to 1
    public void UpdateProgressBar(float percentage)
    {
        currentPercentage = Mathf.Clamp01(percentage);
        UpdateVisuals();
    }

    public void SetFillColorGradient(Color empty, Color full)
    {
        emptyColor = empty;
        fullColor = full;
        useFillColorGradient = true;
        UpdateVisuals();
    }

    public void FaceMainCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            worldSpaceCanvas = canvas.transform;
    }

    private void UpdateVisuals()
    {
        displayLayer.fillAmount = currentPercentage;
        if (useFillColorGradient)
            displayLayer.color = Color.Lerp(emptyColor, fullColor, currentPercentage);
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
