using UnityEngine;

public class ProductionSpot : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ProgressBar progressBar;

    [SerializeField]
    private Transform productionLocation;

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private string promptMessage;

    [SerializeField]
    private int requiredAmount;

    [SerializeField]
    private bool clickToBoost, autoProduce;

    [SerializeField]
    private int amountPerClick, amountPerAutoProduce;

    [SerializeField]
    private float timePerAutoProduce;

    [SerializeField]
    private SoundEffectPlayer autoProduceEffectPlayer, clickEffectPlayer; 

    private float timer;
    private int currentAmount;
    private bool startProducing;

    private void Start()
    {
        timer = 0f;
        currentAmount = 0;
        startProducing = false; 
    }

    public void Interact()
    {
        if (clickToBoost)
        {
            AddCurrentAmount(amountPerClick);
            clickEffectPlayer.PlayOneShot(); 
        }
        if (autoProduce)
        {
            startProducing = true;
            autoProduceEffectPlayer.Play();    
        }
    }

    private void Update()
    {
        if (startProducing)
        {
            timer += Time.deltaTime;
            if (timer >= timePerAutoProduce)
            {
                timer %= timePerAutoProduce;
                AddCurrentAmount(amountPerAutoProduce);
            }
        }
        if (currentAmount >= requiredAmount)
        {
            ProduceItem(); 
        }
    }

    public string GetPromptMessage()
    {
        return promptMessage;
    }

    private void AddCurrentAmount(int amount)
    {
        currentAmount += amount;
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        float currentPercentage = Mathf.Clamp01((float)currentAmount / requiredAmount);
        progressBar.UpdateProgressBar(currentPercentage);
    }

    private void ProduceItem()
    {
        GameObject item = Instantiate(itemPrefab, productionLocation.position, Quaternion.identity);
        currentAmount = 0;
        startProducing = false;
        UpdateProgressBar();
        autoProduceEffectPlayer.StopPlaying(); 
    }
}
