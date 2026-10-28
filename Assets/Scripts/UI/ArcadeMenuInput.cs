using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArcadeMenuInput : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button[] upperRowButtons;
    [SerializeField] private Button[] lowerRowButtons;

    private InputActions input;

    private void Awake()
    {
        input = new InputActions();
    }

    private void OnEnable()
    {
        input.UI.Start.performed += OnStart;
        input.UI.Upper.performed += OnUpper;
        input.UI.Lower.performed += OnLower;
        input.UI.Quit.performed += OnQuit;
        input.UI.Enable();
    }

    private void OnDisable()
    {
        input.UI.Disable();
        input.UI.Start.performed -= OnStart;
        input.UI.Upper.performed -= OnUpper;
        input.UI.Lower.performed -= OnLower;
        input.UI.Quit.performed -= OnQuit;
    }

    private void OnDestroy()
    {
        input.Dispose();
    }

    private void OnStart(InputAction.CallbackContext context)
    {
        InvokeIfAvailable(startButton);
    }

    private void OnUpper(InputAction.CallbackContext context)
    {
        InvokeFirstAvailable(upperRowButtons);
    }

    private void OnLower(InputAction.CallbackContext context)
    {
        InvokeFirstAvailable(lowerRowButtons);
    }

    private void OnQuit(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    private static void InvokeFirstAvailable(Button[] buttons)
    {
        if (buttons == null)
            return;

        foreach (Button button in buttons)
        {
            if (InvokeIfAvailable(button))
                return;
        }
    }

    private static bool InvokeIfAvailable(Button button)
    {
        if (button == null || !button.IsActive() || !button.IsInteractable())
            return false;

        button.onClick.Invoke();
        return true;
    }
}
