using UnityEngine;

public class CursorHider : MonoBehaviour
{
    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }
}
