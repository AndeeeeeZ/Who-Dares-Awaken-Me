using UnityEngine;

public class Wall : HasDurability, IInteractable
{
    [SerializeField]
    private string promptMessage;

    [SerializeField]
    private ItemTypes requiredItemType;

    [SerializeField]
    private GameObject fixBoard;

    [SerializeField]
    private bool isBroken;

    [SerializeField]
    private float fixInterval;

    [SerializeField]
    private int amountFixedPerInterval, amountPerClick;

    private float wallTimer;
    public bool isHolding, boardPlaced;
    private void Start()
    {
        base.OnStart();
        isHolding = false;
        boardPlaced = false;
        fixBoard.SetActive(false);
    }

    private void LateUpdate()
    {
        if (isBroken && isHolding && boardPlaced)
        {
            wallTimer += Time.deltaTime;
            if (wallTimer > fixInterval)
            {
                wallTimer -= fixInterval;
                AddCurrentDurability(amountFixedPerInterval);
            }
        }
        isHolding = false;
    }
    public void Interact()
    {
        if (isBroken && !boardPlaced && GameController.Instance.isHoldingBoard)
        {
            fixBoard.SetActive(true);
            boardPlaced = true;
            GameController.Instance.isHoldingBoard = false;
            isHolding = true;

            AddCurrentDurability(amountPerClick);
        }
    }

    public void Hold()
    {
        isHolding = true;
    }

    public string GetPromptMessage()
    {
        return promptMessage;
    }

    protected override int DurabilityUpdateCheck()
    {
        if (currentDurability == maxDurability)
        {
            // isBroken = false; 
        }
        return 0; 
    }
}
