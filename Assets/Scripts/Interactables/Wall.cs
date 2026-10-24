using UnityEngine;

public class Wall : HasDurability, IInteractable
{
    [SerializeField]
    private string promptMessage;

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
    public bool IsDefending => boardPlaced && GetCurrentDurability() > 0;
    private void Start()
    {
        base.OnStart();
        isHolding = false;
        boardPlaced = false;
        fixBoard.SetActive(false);
        currentDurability = 0;
        UpdateDurabilityDisplay();
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

            AddCurrentDurability(Mathf.Max(1, amountPerClick));
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
        if (boardPlaced && currentDurability <= 0)
        {
            boardPlaced = false;
            fixBoard.SetActive(false);
            wallTimer = 0f;
        }

        isBroken = currentDurability < maxDurability;
        return 0; 
    }
}
