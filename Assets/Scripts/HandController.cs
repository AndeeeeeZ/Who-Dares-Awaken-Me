using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private Image leftHand, rightHand, center;
    [SerializeField]
    private Sprite leftIdle, rightIdle, leftHoldBar, rightHoldBoard, rightHoldHammer, bothHold; 
    private InputActions input;
    private Animator leftHandAnimator, rightHandAnimator;

    private void Awake()
    {
        input = new InputActions();
    }

    private void Start()
    {
        leftHandAnimator = leftHand.gameObject.GetComponent<Animator>();
        rightHandAnimator = rightHand.gameObject.GetComponent<Animator>();
        StopHoldingBoardOnWall();
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
        GameController g = GameController.Instance;
        if (g.isHoldingBar)
            leftHand.sprite = leftHoldBar;
        else
            leftHand.sprite = leftIdle;

        if (g.isHoldingBoard)
            rightHand.sprite = rightHoldBoard;
        else
            rightHand.sprite = rightIdle;
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
