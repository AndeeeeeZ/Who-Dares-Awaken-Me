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
    private float waitTimeBeforeDestroy;

    [SerializeField]
    private bool haveDeathAnimation, haveExplosionEffect;

    private NavMeshAgent agent;
    private bool dead;


    private void Awake()
    {
        pathfinding = GetComponent<EnemyPathfinding>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        dead = false;
    }

    public void Interact()
    {
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
                    // Play sound clip
                }
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

    private void OnTriggerEnter(Collider otherC)
    {
        Debug.Log($"OnTriggerEnter called with: {otherC.gameObject.name}");

        GameObject other = otherC.gameObject;

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
