using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private Image leftHand, rightHand, center, leftHoldBar, rightHoldBoard, rightHoldHammer;
    private InputActions input;
    private Animator leftHandAnimator, rightHandAnimator;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private float raycastDistance;
    private Camera cam;
    private void Awake()
    {
        input = new InputActions();
    }

    private void Start()
    {
        cam = Camera.main;
        UpdateVisuals();
        // leftHandAnimator = leftHand.gameObject.GetComponent<Animator>();
        // rightHandAnimator = rightHand.gameObject.GetComponent<Animator>();
        StopHoldingBoardOnWall();
    }

    private void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        // Debug.DrawRay(ray.origin, ray.direction * raycastDistance);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, raycastDistance, mask))
        {
            if (hitInfo.collider.GetComponent<IInteractable>() != null)
            {
                if (hitInfo.collider.GetComponent<Item>() == null)
                {
                    rightHoldBoard.gameObject.SetActive(false);
                    rightHand.gameObject.SetActive(false);
                    rightHoldHammer.gameObject.SetActive(true);
                }
            }

        }
        else
        {
            rightHoldHammer.gameObject.SetActive(false);
            UpdateVisuals();
        }
    }

    private void OnEnable()
    {
        input.Enable();
        UpdateVisuals();
        input.Player.Interact.performed += OnInteract;
        input.Player.Grab.performed += OnGrab;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Interact.performed -= OnInteract;
        input.Player.Grab.performed -= OnGrab;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        UpdateVisuals();
        if (rightHandAnimator != null)
            rightHandAnimator.Play("Interact");
        else
            Debug.LogError("Missing right hand animator");
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        UpdateVisuals();
        if (leftHandAnimator != null)
            leftHandAnimator.Play("Grab");
        else
            Debug.LogError("Missing left hand animator");
    }

    private void UpdateVisuals()
    {
        if (GameController.Instance.isHoldingBar)
        {
            leftHoldBar.gameObject.SetActive(true);
            leftHand.gameObject.SetActive(false);
        }
        else
        {
            leftHoldBar.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(true);
        }

        if (rightHoldHammer.IsActive())
        {
            rightHand.gameObject.SetActive(false);
            rightHoldBoard.gameObject.SetActive(false);
        }
        else if (GameController.Instance.isHoldingBoard)
        {
            rightHoldBoard.gameObject.SetActive(true);
            rightHand.gameObject.SetActive(false);
            rightHoldHammer.gameObject.SetActive(false);
        }
        else
        {
            rightHand.gameObject.SetActive(true);
            rightHoldBoard.gameObject.SetActive(false);

            rightHoldHammer.gameObject.SetActive(false);
        }
    }

    public void HoldBoardOnWall()
    {
        leftHand.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);
        center.gameObject.SetActive(true);
    }

    public void StopHoldingBoardOnWall()
    {
        UpdateVisuals();
        leftHand.gameObject.SetActive(true);
        rightHand.gameObject.SetActive(true);
        center.gameObject.SetActive(false);
    }
}
