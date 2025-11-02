using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private Image leftHand, rightHand;

    private InputActions input;
    private Animator leftHandAnimator, rightHandAnimator;
    private SpriteRenderer leftSR, rightSR; 

    private void Awake()
    {
        input = new InputActions();
    }

    private void Start()
    {
        leftHandAnimator = leftHand.gameObject.GetComponent<Animator>();
        rightHandAnimator = rightHand.gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        input.Enable();
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
        if (rightHandAnimator != null)
            rightHandAnimator.Play("Interact");
        else
            Debug.LogError("Missing right hand animator");
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        if (leftHandAnimator != null)
            leftHandAnimator.Play("Grab");
        else
            Debug.LogError("Missing left hand animator");
    }
}
