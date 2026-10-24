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

        if (otherC.CompareTag("Floor"))
        {
            agent.enabled = true;
            rb.isKinematic = true;
        }

        if (!otherC.TryGetComponent(out HasDurability damageTarget))
            return;

        if (damageTarget is Tombstone tombstone && tombstone.GetCurrentDurability() <= 0)
            return;
        if (damageTarget is Door door && !door.IsDefending)
            return;
        if (damageTarget is Wall wall && !wall.IsDefending)
            return;

        if (boomEffect != null)
            boomEffect.PlayOneShot();
        damageTarget.AddCurrentDurability(damageToDurability);
        Resolve();
        if (haveExplosionEffect)
            animator.Play("Explode");
        Destroy(gameObject, waitTimeBeforeDestroy);
    }

    public void FindTarget(HasDurability target, GameController gameController)
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
