using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private Image leftHand, rightHand, center, leftHoldBar, rightHoldBoard, rightHoldHammer;
    private InputActions input;
    // private Animator leftHandAnimator, rightHandAnimator;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private float raycastDistance;

    [SerializeField]
    private TextMeshProUGUI promptText;
    private Camera cam;
    private void Awake()
    {
        input = new InputActions();
    }

    private void Start()
    {
        cam = Camera.main;
        UpdateVisuals();
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
                promptText.text = hitInfo.collider.GetComponent<IInteractable>().GetPromptMessage();
                if (hitInfo.collider.GetComponent<Item>() == null
                    && hitInfo.collider.GetComponent<Wall>() == null
                    && hitInfo.collider.GetComponent<Door>() == null)
                {
                    rightHoldBoard.gameObject.SetActive(false);
                    rightHand.gameObject.SetActive(false);
                    rightHoldHammer.gameObject.SetActive(true);
                }

                if (hitInfo.collider.GetComponent<Wall>() != null)
                {
                    hitInfo.collider.GetComponent<Wall>().Hold();
                    center.gameObject.SetActive(true);
                    DisableTwoHands();
                }
                else
                {
                    center.gameObject.SetActive(false);
                    UpdateVisuals();
                }
            }
            else
            {
                promptText.text = "";
                center.gameObject.SetActive(false);
                UpdateVisuals();
            }
        }
        else
        {
            rightHoldHammer.gameObject.SetActive(false);
            center.gameObject.SetActive(false);
            promptText.text = "";
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
        // if (rightHandAnimator != null)
        //     rightHandAnimator.Play("Interact");
        // else
        //     Debug.LogError("Missing right hand animator");
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        UpdateVisuals();
        // if (leftHandAnimator != null)
        //     leftHandAnimator.Play("Grab");
        // else
        //     Debug.LogError("Missing left hand animator");
    }

    private void UpdateVisuals()
    {
        if (GameController.Instance != null && GameController.Instance.isHoldingBar)
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
        else if (GameController.Instance != null && GameController.Instance.isHoldingBoard)
        {
            rightHoldBoard.gameObject.SetActive(true);
            rightHand.gameObject.SetActive(false);
            rightHoldHammer.gameObject.SetActive(false);
        }
        else
        {
            rightHand.gameObject.SetActive(true);
            rightHoldHammer.gameObject.SetActive(false);
            rightHoldBoard.gameObject.SetActive(false);
        }
    }

    private void DisableTwoHands()
    {
        leftHand.gameObject.SetActive(false);
        leftHoldBar.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);
        rightHoldHammer.gameObject.SetActive(false);
        rightHoldHammer.gameObject.SetActive(false);
    }
}
