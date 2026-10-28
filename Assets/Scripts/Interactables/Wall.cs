using UnityEngine;
using UnityEngine.Serialization;

public class Wall : HasDurability, IInteractable
{
    [Header("Interaction Messages")]
    [SerializeField] private string noWoodMessage = "GET WOOD TO SEAL THE HOLE";
    [SerializeField] private string holdingWoodMessage = "CLICK TO PLACE WOOD";
    [FormerlySerializedAs("promptMessage")]
    [SerializeField] private string aimAtWoodMessage = "KEEP AIMING AT THE WOOD TO SEAL THE HOLE";
    [SerializeField] private string woodFinishedMessage = "WOOD IS SECURED";

    [SerializeField] private GameController gameController;

    [SerializeField]
    private GameObject fixBoard;
    [SerializeField]
    private Collider attackCollider;

    public Collider AttackCollider => attackCollider;

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
        if (gameController == null)
            Debug.LogError("Wall needs a GameController assigned for interaction messages and wood placement.", this);

        if (attackCollider == null)
            Debug.LogWarning("Wall needs its attack collider assigned for enemy targeting.", this);

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
        if (isBroken && !boardPlaced && gameController != null && gameController.isHoldingBoard)
        {
            fixBoard.SetActive(true);
            boardPlaced = true;
            gameController.isHoldingBoard = false;
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
        if (boardPlaced)
            return isBroken ? aimAtWoodMessage : woodFinishedMessage;

        if (gameController == null)
            return string.Empty;

        return gameController.isHoldingBoard ? holdingWoodMessage : noWoodMessage;
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
