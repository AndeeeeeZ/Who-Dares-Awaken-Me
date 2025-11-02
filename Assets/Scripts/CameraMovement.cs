using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private Vector3 cameraOffset;

    private Transform cameraTransform;
    private InputActions input;
    private InputAction lookAction;
    private float xCameraRotation, yCameraRotation;

    private void Awake()
    {
        input = new InputActions();
        lookAction = input.Player.Look;
    }

    private void Start()
    {
        if (!playerTransform)
        {
            Debug.LogError("CameraController missing reference to player");
        }

        cameraTransform = transform;
        xCameraRotation = 180f;
        // cameraTransform.eulerAngles = playerTransform.rotation.eulerAngles; 
    }

    private void LateUpdate()
    {
        if (!playerTransform) return;

        cameraTransform.position = playerTransform.position + cameraOffset;

        Vector2 mouseDelta = lookAction.ReadValue<Vector2>();

        xCameraRotation -= mouseDelta.y * rotationSpeed * Time.deltaTime;
        xCameraRotation = Mathf.Clamp(xCameraRotation, -80f, 80f);

        float yRotationDelta = mouseDelta.x * rotationSpeed * Time.deltaTime;
        yCameraRotation += yRotationDelta;

        cameraTransform.eulerAngles = new Vector3(xCameraRotation, yCameraRotation, 0f);

        playerTransform.gameObject.GetComponent<PlayerMovement>().RotateBy(yRotationDelta);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}