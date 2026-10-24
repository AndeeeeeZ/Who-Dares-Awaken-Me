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
    // [SerializeField]
    // private Material[] modelMaterials;

    [SerializeField]
    private ParticleSystem particle;

    [SerializeField]
    private SoundEffectPlayer soundEffectPlayer; 
    private MeshFilter meshFilter;
    // private MeshRenderer meshRenderer;
    private int currentModel = -1;

    private void Start()
    {
        durabilityDeclines = false;
        base.OnStart(); 
        meshFilter = GetComponent<MeshFilter>();
        // meshRenderer = GetComponent<MeshRenderer>();

        if (progressBar != null)
        {
            progressBar.SetFillColorGradient(emptyRingColor, fullRingColor);
            progressBar.FaceMainCamera();
        }

        if (statePercentage.Length != modelMeshes.Length)
        {
            Debug.LogError("Tombstone has unmatched number of states, meshes, and materials");
        }
        currentDurability = startingDurability;
        currentModel = DurabilityUpdateCheck();
        UpdateDurabilityDisplay();
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
        if (currentDurability == maxDurability)
        {
            durabilityDeclines = false;
        }
        
        float currPercentage = GetCurrentDurabilityPercentage();
        // Debug.Log(currentDurability);
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

        meshFilter.mesh = modelMeshes[index];
        //meshRenderer.material = modelMaterials[index];

        currentModel = index;
    }
}
