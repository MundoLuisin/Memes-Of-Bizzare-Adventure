using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class JungleBossAI : MonoBehaviour
{
    public float wanderRadius = 25f;
    public float wanderInterval = 30f;

    public float health = 500f;

    private float timer = 0f;
    private NavMeshAgent agent;
    private Vector3 origin;

    public Slider healthSlider;
    public Slider easeHealthSlider;

    bool hasTarget;
    bool canAttack = true;

    GameObject target;
    public List<GameObject> targetsList = new List<GameObject>();

    public ParticleSystem particleSystem;
    private AudioSource audioSource;
    public AudioClip audioClipStartAttack;
    public AudioClip audioClipFinishAttack;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = this.GetComponent<AudioSource>();
        origin = transform.position;
        if(!hasTarget) SetNewDestination();
        if(hasTarget) Attack();
    }

    void Update()
    {
        if(health <= 0) Death();

        timer += Time.deltaTime;

        if(healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if(healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, 0.05f);
        }

        if (timer >= wanderInterval && agent.remainingDistance < 0.5f)
        {
            SetNewDestination();
            timer = 0f;
        }

        UpdateTargets();

        if(!hasTarget) SetNewDestination();
        if (hasTarget && target != null && canAttack) StartCoroutine(Attack());
    }

    void Death()
    {
        GameData.Instance.coins += 100;
        Destroy(this.gameObject);
    }

    void LateUpdate()
    {
        Vector3 rot = transform.eulerAngles;

        if(CompareTag("StrawberryElephant")) transform.rotation = Quaternion.Euler(-90f, rot.y, rot.z);
        if(CompareTag("TrenostruzzoTurbo3000")) transform.rotation = Quaternion.Euler(rot.x, rot.y, 0f);
    }


    void UpdateTargets()
    {
        if (targetsList.Count > 0)
        {
            target = targetsList[0];
            hasTarget = true;
        }
        else
        {
            target = null;
            hasTarget = false;
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        particleSystem.Play();
        audioSource.PlayOneShot(audioClipStartAttack);
        yield return new WaitForSeconds(10f);
        audioSource.PlayOneShot(audioClipFinishAttack);
        if(target != null) target.GetComponent<PlayerController>().health -= 100;
        yield return new WaitForSeconds(5f);
        canAttack = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!targetsList.Contains(other.gameObject)) targetsList.Add(other.gameObject);
            target = other.gameObject; 
            hasTarget = true;
    }   }

    private void OnTriggerExit(Collider other)
    {
        targetsList.Remove(other.gameObject);
        if(targetsList == null || targetsList.Count == 0) hasTarget = false;
    }
}
