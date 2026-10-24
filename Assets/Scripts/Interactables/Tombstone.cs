using System;
using UnityEngine;

public class Tombstone : HasDurability, IInteractable
{
    public event Action<Tombstone> DurabilityChanged;

    [SerializeField]
    private string promptMessage;
    [SerializeField]
    private int fixPerInteraction; 
    [SerializeField]
    private float[] statePercentage;
    [SerializeField]
    private Mesh[] modelMeshes;
    [SerializeField]
    private Color emptyRingColor = Color.red;
    [SerializeField]
    private Color fullRingColor = Color.green;

    [SerializeField]
    private ParticleSystem particle;

    [SerializeField]
    private SoundEffectPlayer soundEffectPlayer; 
    private MeshFilter meshFilter;
    private int currentModel = -1;
    private bool isInitialized;

    private void Start()
    {
        durabilityDeclines = false;
        base.OnStart(); 
        meshFilter = GetComponent<MeshFilter>();

        if (progressBar != null)
        {
            progressBar.SetFillColorGradient(emptyRingColor, fullRingColor);
            progressBar.FaceMainCamera();
        }

        if (statePercentage.Length != modelMeshes.Length)
        {
            Debug.LogError("Tombstone has unmatched numbers of state percentages and meshes", this);
        }
        isInitialized = true;
        DurabilityUpdateCheck();
        UpdateDurabilityDisplay();
    }

    public void InitializeDurability(int value)
    {
        currentDurability = Mathf.Clamp(value, 0, maxDurability);
        if (!isInitialized)
            return;

        UpdateDurabilityDisplay();
        DurabilityUpdateCheck();
    }

    public void Interact()
    {
        AddCurrentDurability(fixPerInteraction);
        particle.Play();
        soundEffectPlayer.PlayOneShot(); 
    }

    public string GetPromptMessage()
    {
        return promptMessage;
    }

    protected override int DurabilityUpdateCheck()
    {
        float currPercentage = GetCurrentDurabilityPercentage();
        int modelIndex = statePercentage.Length - 1;
        for (int i = 0; i < statePercentage.Length; i++)
        {
            if (currPercentage >= statePercentage[i])
            {
                modelIndex = i;
                break;
            }
        }
        SwitchModelTo(modelIndex);
        DurabilityChanged?.Invoke(this);
        return modelIndex;
    }

    private void SwitchModelTo(int index)
    {
        if (index == currentModel)
            return;

        meshFilter.sharedMesh = modelMeshes[index];

        currentModel = index;
    }
}
