using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemTypes itemType;

    [SerializeField]
    private string promptMessage, audioClipName, errorClipName;

    public void Interact()
    {
        Debug.Log("Interacted with item");

        if (itemType == ItemTypes.WOOD)
        {
            if (GameController.Instance.isHoldingBoard)
            {
                Debug.LogWarning("Player is already holding a board");
                AudioManager.Instance.PlayClip(errorClipName);
                return;
            }
            else
                GameController.Instance.isHoldingBoard = true;
        }
        if (itemType == ItemTypes.METAL_BAR)
        {
            if (GameController.Instance.isHoldingBar)
            {
                Debug.LogWarning("Player is already holding a bar");
                AudioManager.Instance.PlayClip(errorClipName);
                return;
            }
            else
                GameController.Instance.isHoldingBar = true;
        }

        AudioManager.Instance.PlayClip(audioClipName);
        
        Destroy(gameObject);
    }
    public string GetPromptMessage()
    {
        return promptMessage;
    }

    public ItemTypes GetItemType()
    {
        return itemType;
    }

}

public enum ItemTypes
{
    WOOD,
    METAL_BAR
}