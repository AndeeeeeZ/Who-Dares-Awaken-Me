using UnityEngine;

public class Tombstone : HasDurability, IInteractable
{
    [SerializeField]
    private string promptMessage;
    [SerializeField]
    private int fixPerInteraction; 
    [SerializeField]
    private float[] statePercentage;
    [SerializeField]
    private Mesh[] modelMeshes;
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
        base.OnStart(); 
        meshFilter = GetComponent<MeshFilter>();
        // meshRenderer = GetComponent<MeshRenderer>();

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
        DurabilityUpdateCheck();
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
        // Debug.Log(currentDurability);
        for (int i = 0; i < statePercentage.Length; i++)
        {
            if (currPercentage >= statePercentage[i])
            {
                // Debug.Log("Going to switch state to " + i);
                SwitchModelTo(i);
                return i;
            }
        }
        // Debug.Log("Going to switch state to worst"); 
        SwitchModelTo(statePercentage.Length - 1);
        return statePercentage.Length - 1;
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