using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MinionJungleAi : MonoBehaviour
{
    public float health = 100;
    public float attackTimer = 2f;
    public GameObject target;

    public float wanderRadius = 30f;
    public float wanderTimer = 30f;
    private float wanderCounter;

    NavMeshAgent agent;
    Animator anim;

    private List<GameObject> validTargets = new List<GameObject>();

    void Start()
    {
        anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        wanderCounter = wanderTimer;
    }
    
    void Update()
    {
        CleanTargetList(); 
        if (validTargets.Count > 0)
        {
            target = validTargets[0]; 
            Attack();
            agent.SetDestination(target.transform.position);
        }
        else
        {
            target = null;
            Wander();
        }

        anim.SetBool("isWalking", agent.velocity.magnitude > 0.1f);

        if(health <= 0) Death();
    }

    void Death()
    {
        GameData.Instance.coins += 5;
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 11 || collider.gameObject.layer == 12)
        {
            validTargets.Add(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (validTargets.Contains(collider.gameObject))
        {
            validTargets.Remove(collider.gameObject);
        }
    }

    void CleanTargetList()
    {
        validTargets.RemoveAll(obj => obj == null);
    }

    void Attack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            attackTimer = 2f;

            if (target.TryGetComponent(out PlayerController playerTargetScript))
            {
                if (playerTargetScript.health > 0 && !playerTargetScript.isDead && !playerTargetScript.immunity)
                {
                    playerTargetScript.health -= 25;
                }
            }

            if (target.TryGetComponent(out BotController playerBotTargetScript))
            {
                if (playerBotTargetScript.health > 0 && !playerBotTargetScript.isDead && !playerBotTargetScript.immunity)
                {
                    playerBotTargetScript.health -= 25;
                }
            }

            anim.SetTrigger("Attack");
        }
    }

    void Wander()
    {
        wanderCounter -= Time.deltaTime;

        if (wanderCounter <= 0f)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            wanderCounter = wanderTimer;
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
