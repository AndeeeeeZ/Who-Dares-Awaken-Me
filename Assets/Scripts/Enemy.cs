using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string promptMessage;
    private EnemySpawner spawner;
    private EnemyPathfinding pathfinding;

    [SerializeField]
    private SoundEffectPlayer boomEffect, dieEffect;

    [SerializeField]
    private int maxHealth, damageToDurability;

    private int currentHealth;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float waitTimeBeforeDestroy, colorChangingSpeed;

    [SerializeField]
    private bool haveDeathAnimation, haveExplosionEffect;

    [SerializeField]
    private Color hurtColor;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float pushbackForce, pushUpForce;

    private NavMeshAgent agent;
    private bool dead, changingBackColor;
    private Color originalColor;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pathfinding = GetComponent<EnemyPathfinding>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        dead = false;
        changingBackColor = false;
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (changingBackColor)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, colorChangingSpeed * Time.deltaTime);

            // Check if we're close enough to target (within a small threshold)
            if (Mathf.Abs(spriteRenderer.color.r - originalColor.r) < 0.01f &&
                Mathf.Abs(spriteRenderer.color.g - originalColor.g) < 0.01f &&
                Mathf.Abs(spriteRenderer.color.b - originalColor.b) < 0.01f &&
                Mathf.Abs(spriteRenderer.color.a - originalColor.a) < 0.01f)
            {
                spriteRenderer.color = originalColor; // Snap to exact target
                changingBackColor = false;
            }
        }
    }

    public void Interact()
    {
        spriteRenderer.color = hurtColor;
        changingBackColor = true;
        currentHealth--;
        if (currentHealth <= 0)
        {
            agent.enabled = false;
            if (!dead)
            {
                spawner.CloneDestroyed();
                dead = true;
                if (haveDeathAnimation)
                {
                    animator.Play("Dead");
                }
                else
                {
                    animator.StopPlayback();
                }
                if (dieEffect != null)
                    dieEffect.PlayOneShot();
                Destroy(gameObject, waitTimeBeforeDestroy);
            }
        }
    }
    // private void OnTriggerEnter(Collider otherC)
    // {
    //     GameObject other = otherC.gameObject;
    //     if (other.GetComponent<HasDurability>() != null)
    //     {
    //         Debug.Log("Enemy in contact with tombstone"); 
    //         if (other.GetComponent<Wall>() != null)
    //         {
    //             if (!other.GetComponent<Wall>().boardPlaced)
    //                 return;
    //         }
    //         if (other.GetComponent<Door>() != null)
    //         {
    //             if (!other.GetComponent<Door>().locked)
    //                 return;
    //         }
    //         if (!dead)
    //         {
    //             spawner.CloneDestroyed();
    //             other.GetComponent<HasDurability>().AddCurrentDurability(damageToDurability);
    //             dead = true;
    //             if (haveExplosionEffect)
    //                 animator.Play("Explode");
    //             Destroy(gameObject, waitTimeBeforeDestroy);
    //         }
    //     }
    // }

    public void HitBack(Vector3 sourcePosition)
    {
        agent.enabled = false;
        rb.isKinematic = false;
        Vector3 direction = (transform.position - sourcePosition).normalized;
        rb.AddForce(direction * pushbackForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider otherC)
    {
        Debug.Log($"OnTriggerEnter called with: {otherC.gameObject.name}");

        GameObject other = otherC.gameObject;

        if (otherC.gameObject.CompareTag("Floor"))
        {
            agent.enabled = true;
            rb.isKinematic = true;
        }

        if (other.GetComponent<HasDurability>() != null)
        {
            Debug.Log("Has HasDurability component");

            if (other.GetComponent<Wall>() != null)
            {
                Debug.Log($"Is Wall, boardPlaced: {other.GetComponent<Wall>().boardPlaced}");
                if (!other.GetComponent<Wall>().boardPlaced)
                {
                    Debug.Log("Wall board not placed, returning");
                    return;
                }
            }

            if (other.GetComponent<Door>() != null)
            {
                Debug.Log($"Is Door, locked: {other.GetComponent<Door>().locked}");
                if (!other.GetComponent<Door>().locked)
                {
                    Debug.Log("Door not locked, returning");
                    return;
                }
            }

            if (!dead)
            {
                Debug.Log("Enemy exploding!");
                boomEffect.PlayOneShot();
                spawner.CloneDestroyed();
                other.GetComponent<HasDurability>().AddCurrentDurability(damageToDurability);
                dead = true;
                if (haveExplosionEffect)
                    animator.Play("Explode");
                Destroy(gameObject, waitTimeBeforeDestroy);
            }
            else
            {
                Debug.Log("Enemy already dead, not exploding");
            }
        }
        else
        {
            Debug.Log("No HasDurability component found");
        }
    }

    public void FindTarget(GameObject target)
    {
        if (target != null)
        {
            pathfinding.SetTarget(target);
        }
    }

    public void SetSpawner(EnemySpawner s)
    {
        spawner = s;
    }

    public string GetPromptMessage()
    {
        return promptMessage;
    }
}
