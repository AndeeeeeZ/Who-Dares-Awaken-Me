using System;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string promptMessage;
    private EnemyPathfinding pathfinding;

    public event Action<Enemy> Resolved;

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
        if (dead)
            return;

        spriteRenderer.color = hurtColor;
        changingBackColor = true;
        currentHealth--;
        if (currentHealth <= 0)
        {
            agent.enabled = false;
            Resolve();
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
        if (dead)
            return;

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

            Tombstone tombstone = other.GetComponent<Tombstone>();
            if (tombstone != null && tombstone.GetCurrentDurability() <= 0)
                return;

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
                if (boomEffect != null)
                    boomEffect.PlayOneShot();
                other.GetComponent<HasDurability>().AddCurrentDurability(damageToDurability);
                Resolve();
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

    public void FindTarget(Tombstone target, GameController gameController)
    {
        if (target != null)
        {
            pathfinding.SetTarget(target, gameController);
        }
    }

    private void Resolve()
    {
        if (dead)
            return;

        dead = true;
        Resolved?.Invoke(this);
    }

    private void OnDestroy()
    {
        Resolve();
    }

    public string GetPromptMessage()
    {
        return promptMessage;
    }
}
