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

    private float timer;
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
        if (isBroken)
        {
            if (isHolding)
            {
                timer += Time.deltaTime;
                if (timer > fixInterval)
                {
                    timer -= fixInterval;
                    AddCurrentDurability(amountFixedPerInterval);
                }
            }
        }
        isHolding = false; 
    }
    public void Interact()
    {
        if (isBroken)
        {
            if (GameController.Instance.isHoldingBoard && !boardPlaced)
            {
                fixBoard.SetActive(true);
                boardPlaced = true;
                GameController.Instance.isHoldingBoard = false;
            }
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
}
