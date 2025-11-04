using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class Enemy : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string promptMessage;
    private EnemySpawner spawner;
    private EnemyPathfinding pathfinding;
    [SerializeField]
    private int maxHealth, damageToDurability;

    private int currentHealth;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool haveDeathAnimation, haveExplosionEffect;

    private bool dead;


    private void Awake()
    {
        pathfinding = GetComponent<EnemyPathfinding>();
        currentHealth = maxHealth;
        dead = false;
    }

    public void Interact()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {

            if (!dead)
            {
                spawner.CloneDestroyed();
                dead = true;
                if (haveDeathAnimation)
                {
                    animator.Play("Dead");
                    // Play sound clip
                }
                Destroy(gameObject, 1f);
            }
        }
    }
    private void OnTriggerEnter(Collider otherC)
    {
        GameObject other = otherC.gameObject;
        if (other.GetComponent<HasDurability>() != null)
        {
            if (other.GetComponent<Wall>() != null)
            {
                if (!other.GetComponent<Wall>().boardPlaced)
                    return;
            }
            if (other.GetComponent<Door>() != null)
            {
                if (!other.GetComponent<Door>().locked)
                    return;
            }
            if (!dead)
            {
                spawner.CloneDestroyed();
                other.GetComponent<HasDurability>().AddCurrentDurability(damageToDurability);
                dead = true;
                if (haveExplosionEffect)
                    animator.Play("Explode");
                Destroy(gameObject, 1f);
            }
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
