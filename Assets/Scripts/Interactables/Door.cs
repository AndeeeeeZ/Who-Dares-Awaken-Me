using UnityEngine;

public class Door : HasDurability, IInteractable
{
    [SerializeField]
    private string promptMessage;

    [SerializeField]
    private GameObject fixChain;

    [SerializeField]
    private bool isOpen;

    public bool locked;
    public bool IsDefending => locked && GetCurrentDurability() > 0;

    [SerializeField]
    private GameObject open, close;

    private void Start()
    {
        base.OnStart();
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
        currentDurability = 0;
        UpdateDurabilityDisplay();
    }
    
    public void Interact()
    {
        if (!locked)
        {
            if (GameController.Instance.isHoldingBar)
            {
                GameController.Instance.isHoldingBar = false; 
                open.SetActive(false);
                close.SetActive(true); 
                fixChain.SetActive(true);
                isOpen = false;
                locked = true;
                GetUpToFullDurability(); 
            }
        }
    }

    public string GetPromptMessage()
    {
        if (!locked)
            return promptMessage;
        return ""; 
    }

    protected override int DurabilityUpdateCheck()
    {
        if (locked && currentDurability <= 0)
        {
            locked = false;
            isOpen = true;
            fixChain.SetActive(false);
            open.SetActive(true);
            close.SetActive(false);
        }

        return 0;
    }
}
