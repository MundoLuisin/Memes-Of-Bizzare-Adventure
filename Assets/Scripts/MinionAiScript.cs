using UnityEngine;
using UnityEngine.AI;

public class MinionAiScript : MonoBehaviour
{
    public Vector3 destination;
    public Vector3 finalDestination;

    [SerializeField] private Texture[] minionsSkins;
    public char team;

    public GameObject target;
    public bool hasTarget = false;
    public bool passedHalfway = false;

    public float health = 50;
    public float attackTimer = 2;

    NavMeshAgent agent;

    void Start()
    {
        if(team == 'A')
        {
            this.GetComponentInChildren<Renderer>().material.SetTexture("_BaseMap", minionsSkins[0]);
            this.gameObject.layer = 9;
        }
        else if(team == 'B')
        {
            this.GetComponentInChildren<Renderer>().material.SetTexture("_BaseMap", minionsSkins[1]);
            this.gameObject.layer = 10;
        }  

        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(destination);
    }

    void Update()
    {
        //Redestination 
        if(target == null || ((target.TryGetComponent<MinionAiScript>(out var m) && m.health <= 0) || (target.TryGetComponent<PlayerController>(out var p) && p.health <= 0)))
        {
            hasTarget = false;
            target = null;

            if(passedHalfway)
            {
                agent.SetDestination(finalDestination);
            }
            else
            {
                agent.SetDestination(destination);
            }
            return;
        }

    // Attack
    if(hasTarget && target != null)
    {
        agent.SetDestination(target.transform.position);
        attackTimer = attackTimer - Time.deltaTime;
        if(attackTimer <= 0)
        {
            attackTimer = 2;

            if(target.TryGetComponent(out MinionAiScript minionTargetScript))
            {
                if(minionTargetScript.health > 0)
                {
                    minionTargetScript.health -= 10;
                } 
            }
            if(target.TryGetComponent(out PlayerController playerTargetScript))
            {
                if(playerTargetScript.health > 0 && !playerTargetScript.isDead && !playerTargetScript.immunity) 
                {
                    playerTargetScript.health -= 10;
                }
            }
            if(target.TryGetComponent(out CoreManager CoreTargetScript))
            {
                if(CoreTargetScript.health > 0) 
                {
                    CoreTargetScript.health -= 10;
                }
            }
        }
    }


        // Top Lane
        if(this.transform.position.x >= 260 && this.transform.position.z >= 260)
        {
            agent.SetDestination(finalDestination);
            passedHalfway = true;
        }
        // Bot Lane
        if(this.transform.position.x <= -260 && this.transform.position.z <= -260)
        {
            agent.SetDestination(finalDestination);
            passedHalfway = true;
        }

        // Death
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
