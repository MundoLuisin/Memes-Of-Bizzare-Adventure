using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public char team;
    public float health = 250;
    public float attackTimer = 0.5f;
    public GameObject target;
    private AudioSource audioSource;
    [SerializeField] private AudioClip audioClipElectricShock;
    [SerializeField] private GameObject lightningBoltPrefab;

    private List<GameObject> validTargets = new List<GameObject>();

    void Start()
    {
        if(team == 'A') this.gameObject.layer = 9;
        if(team == 'B') this.gameObject.layer = 10;
        audioSource = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        CleanTargetList(); 
        if (validTargets.Count > 0)
        {
            target = validTargets[0]; 
            Attack();
        }
        else
        {
            target = null;
        }

        if(health <= 0) Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (team == 'A' && (collider.gameObject.layer == 10 || collider.gameObject.layer == 12))
        {
            validTargets.Add(collider.gameObject);
        }
        else if (team == 'B' && (collider.gameObject.layer == 9 || collider.gameObject.layer == 11))
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
            attackTimer = 0.5f;

            if (target.TryGetComponent(out MinionAiScript minionTargetScript))
            {
                if (minionTargetScript.health > 0)
                {
                    minionTargetScript.health -= 10;
                    audioSource.PlayOneShot(audioClipElectricShock);
                    GameObject lightningBoltReference = Instantiate(lightningBoltPrefab, target.transform.position, Quaternion.identity);
                    Destroy(lightningBoltReference, 1.5f);
                }
            }

            if (target.TryGetComponent(out PlayerController playerTargetScript))
            {
                if (playerTargetScript.health > 0 && !playerTargetScript.isDead && !playerTargetScript.immunity)
                {
                    playerTargetScript.health -= 20;
                    audioSource.PlayOneShot(audioClipElectricShock);
                    GameObject lightningBoltReference = Instantiate(lightningBoltPrefab, target.transform.position, Quaternion.identity);
                    Destroy(lightningBoltReference, 1.5f);
                }
            }
            
            if (target.TryGetComponent(out BotController playerBotTargetScript))
            {
                if (playerBotTargetScript.health > 0 && !playerBotTargetScript.isDead && !playerBotTargetScript.immunity)
                {
                    playerBotTargetScript.health -= 20;
                    audioSource.PlayOneShot(audioClipElectricShock);
                    GameObject lightningBoltReference = Instantiate(lightningBoltPrefab, target.transform.position, Quaternion.identity);
                    Destroy(lightningBoltReference, 1.5f);
                }
            }
        }
    }
}
