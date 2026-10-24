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
    [SerializeField] private TextMeshProUGUI promptText;

    private void OnEnable()
    {
        if (playerInteract == null || gameController == null || leftHand == null || rightHand == null ||
            center == null || leftHoldBar == null || rightHoldBoard == null || rightHoldHammer == null ||
            promptText == null)
        {
            Debug.LogError("HandController has missing Inspector references.", this);
            enabled = false;
        }
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
        bool showTwoHandHold = aimingAtWall && holdingBoard;
        bool showHands = !showTwoHandHold;
        bool showHammer = target != null && !aimingAtWall && !(target is Item) && !(target is Door);

        SetVisible(center, showTwoHandHold);
        SetVisible(leftHoldBar, showHands && holdingBar);
        SetVisible(leftHand, showHands && !holdingBar);
        SetVisible(rightHoldHammer, showHammer);
        SetVisible(rightHoldBoard, showHands && !showHammer && holdingBoard);
        SetVisible(rightHand, showHands && !showHammer && !holdingBoard);
    }

    private static void SetVisible(Image image, bool visible)
    {
        if (image.gameObject.activeSelf != visible)
            image.gameObject.SetActive(visible);
    }
}
