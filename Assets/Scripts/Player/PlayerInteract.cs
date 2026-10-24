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
    public IInteractable CurrentTarget { get; private set; }

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
        CurrentTarget = null;
    }

    private void Update()
    {
        TryGetTarget(out _, out IInteractable target);
        CurrentTarget = target;
        if (target is Wall wall)
            wall.Hold();

        if (drawRay)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (TryGetTarget(out Collider hitCollider, out IInteractable target))
        {
            target.Interact();

            if (hitCollider.TryGetComponent(out Enemy enemy))
                enemy.HitBack(transform.position);
        }
    }

    private bool TryGetTarget(out Collider hitCollider, out IInteractable target)
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, mask))
        {
            target = hit.collider.GetComponent<IInteractable>();
            if (target != null)
            {
                hitCollider = hit.collider;
                return true;
            }
        }

        hitCollider = null;
        target = null;
        return false;
    }
}
