using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private bool drawRay;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private float raycastDistance;
    private InputActions input;

    private void Awake()
    {
        input = new InputActions();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Interact.performed += OnInteractPerformed;
        input.Player.Grab.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Interact.performed -= OnInteractPerformed;
        input.Player.Grab.performed -= OnInteractPerformed;
    }

    private void Update()
    {
        if (drawRay)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        // Debug.DrawRay(ray.origin, ray.direction * raycastDistance);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, raycastDistance, mask))
        {
            if (hitInfo.collider.GetComponent<IInteractable>() != null)
            {
                hitInfo.collider.GetComponent<IInteractable>().Interact();

                if (hitInfo.collider.GetComponent<Enemy>() != null)
                {
                    Debug.Log("Going to hit enemy");
                    hitInfo.collider.GetComponent<Enemy>().HitBack(transform.position); 
                }
            }
        }
    }


}
