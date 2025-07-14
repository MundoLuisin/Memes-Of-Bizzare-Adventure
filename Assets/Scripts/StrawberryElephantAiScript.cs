using UnityEngine;
using UnityEngine.AI;

public class StrawberryElephantAiScript : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;

    private float timer = 0f;
    private NavMeshAgent agent;
    private Vector3 origin;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        origin = transform.position;
        SetNewDestination();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderInterval && agent.remainingDistance < 0.5f)
        {
            SetNewDestination();
            timer = 0f;
        }
    }

    void LateUpdate()
    {
        Vector3 rot = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(-90f, rot.y, rot.z); // fija X en 90
    }


    void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += origin;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
