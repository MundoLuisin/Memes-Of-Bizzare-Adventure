using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class MinionTarget : MonoBehaviour
{
    public List<GameObject> targetList = new List<GameObject>();
    public MinionAiScript minionScript;
    public NavMeshAgent minionAgent;
    public char team;
    public GameObject closestTarget;

    void Start()
    {
        minionScript = this.GetComponentInParent<MinionAiScript>();
        minionAgent = this.GetComponentInParent<NavMeshAgent>();
        team = minionScript.team;
    }

    void Update()
    {
        targetList.RemoveAll(item => item == null); 

        if(targetList.Count > 0 && minionScript.hasTarget == false)
        {
            foreach(GameObject target in targetList)
            {
                if (target == null) continue; 

                float closestDistance = Mathf.Infinity;
                float distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }
            minionScript.target = closestTarget;
            minionScript.hasTarget = true;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(team == 'A')
        {
            if(!targetList.Contains(collider.gameObject) && (collider.gameObject.layer == 10 || collider.gameObject.layer == 12))
            {
                targetList.Add(collider.gameObject);
            }
        }
        else if(team == 'B')
        {
            if(!targetList.Contains(collider.gameObject) && (collider.gameObject.layer == 9 || collider.gameObject.layer == 11))
            {
                targetList.Add(collider.gameObject);
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if(targetList.Contains(collider.gameObject))
        {
            targetList.Remove(collider.gameObject);
        }
    }
}

