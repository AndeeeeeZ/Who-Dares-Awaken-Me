using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemTypes itemType;

    [SerializeField]
    private string promptMessage, audioClipName;

    public void Interact()
    {
        Debug.Log("Interacted with item");

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