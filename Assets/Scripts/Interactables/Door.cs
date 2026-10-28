using UnityEngine;
using UnityEngine.Serialization;

public class Door : HasDurability, IInteractable
{
    [Header("Interaction Messages")]
    [FormerlySerializedAs("promptMessage")]
    [SerializeField] private string noChainMessage = "GET CHAIN TO LOCK DOOR";
    [SerializeField] private string holdingChainMessage = "CLICK TO ATTACH CHAIN";
    [SerializeField] private string chainAttachedMessage = "CHAIN IS ATTACHED";

    [SerializeField] private GameController gameController;

    [SerializeField]
    private GameObject fixChain;

    [SerializeField]
    private bool isOpen;

    public bool locked;
    public bool IsDefending => locked && GetCurrentDurability() > 0;

    [SerializeField]
    private GameObject open, close;

    [SerializeField]
    private Collider attackCollider;

    public Collider AttackCollider => attackCollider;

    private void Start()
    {
        if (gameController == null)
            Debug.LogError("Door needs a GameController assigned for interaction messages and chain placement.", this);

        locked = !isOpen;
        currentDurability = locked ? maxDurability : 0;
        base.OnStart();
        fixChain.SetActive(locked);
        open.SetActive(isOpen);
        close.SetActive(!isOpen);
    }

    public void SetStartingOpenState(bool shouldOpen)
    {
        isOpen = shouldOpen;
    }
    
    public void Interact()
    {
        if (!locked)
        {
            if (gameController != null && gameController.isHoldingBar)
            {
                gameController.isHoldingBar = false;
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
        if (locked)
            return chainAttachedMessage;

        if (gameController == null)
            return string.Empty;

        return gameController.isHoldingBar ? holdingChainMessage : noChainMessage;
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
