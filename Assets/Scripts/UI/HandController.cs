using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private GameController gameController;
    [SerializeField] private Image leftHand;
    [SerializeField] private Image rightHand;
    [SerializeField] private Image center;
    [SerializeField] private Image leftHoldBar;
    [SerializeField] private Image rightHoldBoard;
    [SerializeField] private Image rightHoldHammer;
    [SerializeField] private Image rightHoldHammerDown;
    [SerializeField, Min(0f)] private float hammerDownMinDuration = 0.1f;
    [SerializeField] private TextMeshProUGUI promptText;

    private bool hammerDown;
    private float hammerDownUntil;

    private void OnEnable()
    {
        if (playerInteract == null || gameController == null || leftHand == null || rightHand == null ||
            center == null || leftHoldBar == null || rightHoldBoard == null || rightHoldHammer == null ||
            rightHoldHammerDown == null ||
            promptText == null)
        {
            Debug.LogError("HandController has missing Inspector references.", this);
            enabled = false;
            return;
        }

        hammerDown = false;
        hammerDownUntil = 0f;
        SetVisible(rightHoldHammerDown, false);
    }

    private void OnDisable()
    {
        hammerDown = false;
        hammerDownUntil = 0f;
        if (rightHoldHammerDown != null)
            SetVisible(rightHoldHammerDown, false);
    }

    private void LateUpdate()
    {
        IInteractable target = playerInteract.CurrentTarget;

        string prompt = target != null ? target.GetPromptMessage() : string.Empty;
        if (promptText.text != prompt)
            promptText.text = prompt;

        bool holdingBar = gameController.isHoldingBar;
        bool holdingBoard = gameController.isHoldingBoard;
        bool aimingAtWall = target is Wall;
        bool showTwoHandHold = target is Wall wall && wall.boardPlaced;
        bool showHands = !showTwoHandHold;
        bool showHammer = target != null && !aimingAtWall && !(target is Item) && !(target is Door);

        if (!showHammer)
            hammerDown = false;
        else if (playerInteract.WasInteractPressedThisFrame)
        {
            hammerDown = true;
            hammerDownUntil = Time.unscaledTime + hammerDownMinDuration;
        }
        else if (!playerInteract.IsInteractHeld && Time.unscaledTime >= hammerDownUntil)
            hammerDown = false;

        SetVisible(center, showTwoHandHold);
        SetVisible(leftHoldBar, showHands && holdingBar);
        SetVisible(leftHand, showHands && !holdingBar);
        SetVisible(rightHoldHammer, showHammer && !hammerDown);
        SetVisible(rightHoldHammerDown, hammerDown);
        SetVisible(rightHoldBoard, showHands && !showHammer && holdingBoard);
        SetVisible(rightHand, showHands && !showHammer && !holdingBoard);
    }

    private static void SetVisible(Image image, bool visible)
    {
        if (image.gameObject.activeSelf != visible)
            image.gameObject.SetActive(visible);
    }
}
