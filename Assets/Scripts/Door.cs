using System.IO.IsolatedStorage;
using UnityEngine;

public class Door : HasDurability, IInteractable
{
    [SerializeField]
    private string promptMessage;

    [SerializeField]
    private ItemTypes requiredItemType;

    [SerializeField]
    private GameObject fixChain;

    [SerializeField]
    private bool isOpen;

    public bool locked;

    [SerializeField]
    private GameObject open, close;

    private void Start()
    {
        fixChain.SetActive(false);
        if (isOpen)
        {
            open.SetActive(true);
            close.SetActive(false);
        }
        else
        {
            open.SetActive(false);
            close.SetActive(true); 
        }
        locked = false;
    }
    
    public void Interact()
    {
        if (isOpen)
        {
            if (GameController.Instance.isHoldingBar && !locked)
            {
                GameController.Instance.isHoldingBar = false; 
                open.SetActive(false);
                close.SetActive(true); 
                fixChain.SetActive(true);
                locked = true;
                GetUpToFullDurability(); 
            }
        }
    }

    public string GetPromptMessage()
    {
        if (isOpen && !locked)
            return promptMessage;
        return ""; 
    }
}
