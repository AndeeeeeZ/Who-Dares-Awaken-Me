using UnityEngine;

public class Wall : HasDurability, IInteractable
{
    [SerializeField]
    private string promptMessage;

    [SerializeField]
    private ItemTypes requiredItemType;

    [SerializeField]
    private float[] statePercentage;

    [SerializeField]
    private Mesh[] modelMeshes;

    [SerializeField]
    private GameObject fixBoard;

    [SerializeField]
    private bool isBroken;

    [SerializeField]
    private float fixInterval;

    [SerializeField]
    private int amountFixedPerInterval, amountPerClick;

    private float timer;
    private bool isHolding, boardPlaced; 
    private void Start()
    {
        base.OnStart();
        isHolding = false;
        boardPlaced = false; 
        fixBoard.SetActive(false);
    }

    private void Update()
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
    }
    public void Interact()
    {
        if (isBroken)
        {
            if (GameController.Instance.isHoldingBoard)
            {
                fixBoard.SetActive(true);
            }
            isHolding = true;
            
            AddCurrentDurability(amountPerClick);
        }
    }

    public string GetPromptMessage()
    {
        return promptMessage;
    }
}
