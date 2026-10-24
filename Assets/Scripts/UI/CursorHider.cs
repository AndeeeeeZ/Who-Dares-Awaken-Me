using UnityEngine;
using UnityEngine.InputSystem;

public class CursorHider : MonoBehaviour
{
    InputActions input;
    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Optionally, hide the cursor
        Cursor.visible = false;
        input = new InputActions(); 
        input.Player.StopCursorLock.performed += OnStopCursorLockPerformed;
    }



    void OnStopCursorLockPerformed(InputAction.CallbackContext callbackContext)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
