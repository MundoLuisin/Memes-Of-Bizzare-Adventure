using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class PlayerController : MonoBehaviour
{

    #region Essential
    public Camera mainCam;
    private GameObject characterModelInstance;
    int layerGround = 8;
    int layerTeam_A = 9;
    int layerTeam_B = 10;
    int layerPlayerTeam_A = 11;
    int layerPlayerTeam_B = 12;
    int bossLayer = 15;
    int enemyLayer;
    int teamLayer;
    string playerName;
    [HideInInspector] public char playerTeam;
    int playerLevel = 1;

    float returnToBaseCooldownTimer = 0;
    [SerializeField] private Image returnToBaseCooldownImage;
    [SerializeField] private GameObject shopCanvasButton;

    [HideInInspector] public CinemachineCamera vCam;
    Vector3 vCamOffsetTeamA = new Vector3(-98.15f, 196.3f, -170f);
    Vector3 vCamOffsetTeamB = new Vector3(98.15f, 196.3f, 170f);

    Vector3 playerPositionTeamA = new Vector3(-250f, 2.5f, 250f);
    Vector3 playerPositionTeamB = new Vector3(250f, 2.5f, -250f);
    Vector3 playerSpawnPosition;

    [SerializeField] private GameObject playerSkinObject;

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerLevelText;

    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TextMeshProUGUI respawnTimer;
    public GameObject targetPrefab;
    public GameObject linePrefab;
    private LineRenderer lineRenderer;
    private GameObject currentLineObject;
    [HideInInspector] public NavMeshAgent myNavMeshAgent;

    [SerializeField] private Volume globalVolume;
    private Vignette vignette;
    private ChromaticAberration chromatic;
    #endregion

    #region Main Stats
    public float attackTimer;
    public float realAttackTimer;
    public float health;
    public float damage;
    public float speed;
    public float minionKillValue = 1;
    bool healthRecuperationFlag = true;
    public bool  isDead = false;
    public Slider healthSlider;
    public Slider easeHealthSlider;
    [HideInInspector] public bool immunity = false;
    #endregion

    #region Target System
    [HideInInspector] public float range = 15;
    bool hasTarget;
    GameObject target;
    GameObject moveIconReference;
    private bool combatModeActive = false;
    private bool finishCombatEffects = true;
    #endregion

    #region Animations & Effects
    [HideInInspector] public Animator anim;
    [SerializeField] private GameObject punchEffectPrefab;
    [SerializeField] private GameObject prefabSpellEffect;
    [SerializeField] private GameObject minionKillEffectPrefab;
    #endregion

    #region Audio
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClipAnnouncerSingleKill;
    [SerializeField] private AudioClip audioClipDeath;
    [SerializeField] private AudioClip audioClipPunch;
    [SerializeField] private AudioClip audioClipMagic;
    private AudioSource walkAudioSource;
    #endregion

    #region Skills
    [HideInInspector] public Skill characterSkill_1;
    [HideInInspector] public Skill characterSkill_2;
    [HideInInspector] public Skill characterSkill_3;
    [SerializeField] private Image imageSkill_1;
    [SerializeField] private Image imageSkill_2;
    [SerializeField] private Image imageSkill_3;
    [SerializeField] private Image cooldownImageSkill_1;
    [SerializeField] private Image cooldownImageSkill_2;
    [SerializeField] private Image cooldownImageSkill_3;
    #endregion
    
    void Start()
    {
        playerName = GameData.Instance.playerName;
        playerNameText.text = playerName;
        playerLevelText.text = playerLevel.ToString();

        healthSlider.maxValue = health;
        easeHealthSlider.maxValue = health;

        vCam = GetComponentInChildren<CinemachineCamera>();
        var componentBase = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        var followComponent = componentBase as CinemachineFollow;

        audioSource = GetComponent<AudioSource>();
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        characterModelInstance = Instantiate(GameData.Instance.currentCharacter.characterModel, playerSkinObject.transform);
        walkAudioSource = characterModelInstance.GetComponent<AudioSource>();

        health = GameData.Instance.currentCharacter.health;
        damage = GameData.Instance.currentCharacter.attackPower;
        attackTimer = GameData.Instance.currentCharacter.attackTimer;
        realAttackTimer = attackTimer;
        myNavMeshAgent.speed = GameData.Instance.currentCharacter.speed;
        anim.avatar = GameData.Instance.currentCharacter.avatar;

        SkillInitialization();

        myNavMeshAgent.enabled = false;

        int coin = UnityEngine.Random.Range(0, 1); // This needs to be changed very important !! The Correct => Range(0, 2) 
        if (coin == 0)
        {
            playerTeam = 'A';
            this.gameObject.layer = layerPlayerTeam_A;
            enemyLayer = layerTeam_B;
            teamLayer = layerTeam_A;
            playerSpawnPosition = playerPositionTeamA;
            this.gameObject.transform.position = playerSpawnPosition;
            followComponent.FollowOffset = vCamOffsetTeamA;
        }
        else if(coin == 1)
        {
            playerTeam = 'B';
            this.gameObject.layer = layerPlayerTeam_B;
            enemyLayer = layerTeam_A;
            teamLayer = layerTeam_B;
            playerSpawnPosition = playerPositionTeamB;
            this.gameObject.transform.position = playerSpawnPosition;
           followComponent.FollowOffset = vCamOffsetTeamB;
        }

        myNavMeshAgent.enabled = true;

        if (globalVolume.profile.TryGet(out Vignette vg)) vignette = vg;
        if (globalVolume.profile.TryGet(out ChromaticAberration ca)) chromatic = ca;

        StartCoroutine(ReturnToBaseCooldown());

    }

    void SkillInitialization()
    {
        List<Skill> skills = GameData.Instance.characterSkills;
        int startIndex = (GameData.Instance.currentCharacter.index - 1) * 3;

        characterSkill_1 = skills[startIndex];
        characterSkill_2 = skills[startIndex + 1];
        characterSkill_3 = skills[startIndex + 2];

        imageSkill_1.sprite = characterSkill_1.sprite;
        imageSkill_2.sprite = characterSkill_2.sprite;
        imageSkill_3.sprite = characterSkill_3.sprite;

        characterSkill_1.image = cooldownImageSkill_1;
        characterSkill_2.image = cooldownImageSkill_2;
        characterSkill_3.image = cooldownImageSkill_3;

        StartCoroutine(Cooldown(characterSkill_1));
        StartCoroutine(Cooldown(characterSkill_2));
        StartCoroutine(Cooldown(characterSkill_3));
    }


    void Update()
    {
        if(health < 100 && healthRecuperationFlag)
        {
            healthRecuperationFlag = false;
            StartCoroutine(ReHealth());
        }

        if(healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if(healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, 0.05f);
        }

        if (isDead) return;

        if(health <= 0 && !isDead)
        {
            isDead = true;  
            StartCoroutine(Death());
            return;     
        }

        // NavMeshAgent movement  
        if(Input.GetMouseButton(1) && myNavMeshAgent.enabled)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            int layerToIgnore = LayerMask.GetMask("Building");
            int layerMask = ~layerToIgnore;

            {
                if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                {
                    if(hit.collider.gameObject.layer == layerGround)
                    {
                        if(Input.GetMouseButtonDown(1))
                        {
                            Destroy(moveIconReference);
                            Destroy(currentLineObject);
                            Vector3 offset = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
                            moveIconReference = Instantiate(targetPrefab, offset, Quaternion.identity);
                            currentLineObject = Instantiate(linePrefab);
                            lineRenderer = currentLineObject.GetComponent<LineRenderer>();
                            lineRenderer.positionCount = 2;
                            StartCoroutine(UpdateLine());
                            Destroy(moveIconReference, 1.5f);
                            Destroy(currentLineObject, 1.5f);
                        }

                        myNavMeshAgent.SetDestination(hit.point);
                        myNavMeshAgent.stoppingDistance = 0;
                        hasTarget = false;
                    }

                    if(hit.collider.gameObject.layer == enemyLayer || hit.collider.gameObject.layer == bossLayer)
                    {
                        target = hit.collider.gameObject;
                        hasTarget = true;
                    }
                }
            }
        }

        if(myNavMeshAgent.velocity.sqrMagnitude > 0.01f)
        {
            if (!walkAudioSource.isPlaying) walkAudioSource.Play();
        }
        else
        {
           if (walkAudioSource.isPlaying) walkAudioSource.Stop();
        }

        // Animations  
        bool isMoving = myNavMeshAgent.hasPath && myNavMeshAgent.remainingDistance > myNavMeshAgent.stoppingDistance;
        anim.SetBool("Move", isMoving);

        // Target destination
        if(hasTarget && target != null && myNavMeshAgent.enabled)
        {
            myNavMeshAgent.SetDestination(target.transform.position);
            myNavMeshAgent.stoppingDistance = range;
        }

        if(target != null)
        {
            if(Vector3.Distance(transform.position, target.transform.position) <= range)
            {
                if (combatModeActive == false && finishCombatEffects)
                {
                    StartCoroutine(CombatMode(true));
                    combatModeActive = true;
                    finishCombatEffects = false;
                }

                Vector3 lookDirection = (target.transform.position - transform.position).normalized;
                lookDirection.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 10f);
                attackTimer -= Time.deltaTime;

                if(attackTimer <= 0)
                {
                    attackTimer = realAttackTimer;

                    if(target.TryGetComponent<MinionAiScript>(out MinionAiScript minion))
                    {
                        minion.health -= damage;

                        if(minion.health < 1 || minion.gameObject == null)
                        {
                            audioSource.PlayOneShot(randomKillAudioClip());  
                            GameObject minionKillEffectObj = Instantiate(minionKillEffectPrefab);
                            if(minion.health <= 0) GameData.Instance.coins += minionKillValue;
                            Destroy(minionKillEffectObj, 3f);
                        } 
                    }

                    if(target.TryGetComponent<JungleBossAI>(out JungleBossAI strawberryElephant))
                    {
                        strawberryElephant.health -= damage;
                    }

                    if(target.TryGetComponent<TurretManager>(out TurretManager turret))
                    {
                        turret.health -= damage;
                        if(turret.health <= 0) GameData.Instance.coins += 25;
                    }

                    if(target.TryGetComponent<CoreManager>(out CoreManager core))
                    {
                        core.health -= damage;
                    }

                    GameObject punchEffectObj = Instantiate(punchEffectPrefab, target.transform.position, Quaternion.identity);
                    audioSource.PlayOneShot(audioClipPunch);
                    anim.SetTrigger("Fight");
                    Destroy(punchEffectObj, 1.5f);
                }
            }
        }

        if (target == null && combatModeActive && finishCombatEffects)
        {
            StartCoroutine(CombatMode(false));
            combatModeActive = false;
            finishCombatEffects = false;
        }

        if (Input.GetKeyDown(KeyCode.Q)) Skill_1();
        if (Input.GetKeyDown(KeyCode.E)) Skill_2();
        if (Input.GetKeyDown(KeyCode.R)) Skill_3();
    }

    AudioClip randomKillAudioClip()
    {
        int i = Random.Range(0, audioClipAnnouncerSingleKill.Length);
        return audioClipAnnouncerSingleKill[i];
    }

    public IEnumerator CombatMode(bool on)
    {
        float duration = 2f;
        float time = 0f;

        bool activateEffect = on;

        while(time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            if(activateEffect)
            {
                vCam.Lens.OrthographicSize = Mathf.Lerp(40, 35, t);
                vignette.intensity.value = Mathf.Lerp(0.25f, 0.45f, t);
                chromatic.intensity.value = Mathf.Lerp(0, 0.25f, t);
            }
            else
            {
               vCam.Lens.OrthographicSize = Mathf.Lerp(35, 40, t);
               vignette.intensity.value = Mathf.Lerp(0.45f, 0.25f, t);
               chromatic.intensity.value = Mathf.Lerp(0.25f, 0, t);
            }

            yield return null;
        }

        finishCombatEffects = true;
    }

    public void Skill_1()
    {
        characterSkill_1.Consume(this);
    }

    public void Skill_2()
    {
        characterSkill_2.Consume(this);
    }

    public void Skill_3()
    {
        characterSkill_3.Consume(this);
       /* if (!isOnCooldownSkill3 && hasTarget)
        {
            isOnCooldownSkill3 = true;
            cooldownTimerSkill3 = cooldownTimeSkill3;
            myNavMeshAgent.enabled = false;
            anim.SetTrigger("Spell");
            Vector3 effectPosition = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
            GameObject spellEffect = Instantiate(prefabSpellEffect, effectPosition, Quaternion.identity);
            spellEffect.transform.SetParent(this.transform, true);
            spellEffect.transform.localScale = Vector3.one;
            damage += 200; // Parchear el como la skill hace daño en un futuro porque esto esta mal hecho hahahaha 
            audioSource.PlayOneShot(audioClipMagic);
            Destroy(spellEffect, 8f);
        }*/
    }

    public IEnumerator Cooldown(Skill skill)
    {
        float timer = skill.cooldown;
        skill.image.fillAmount = 1f;
        skill.currentCooldown = skill.cooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            skill.currentCooldown = Mathf.CeilToInt(timer);
            skill.image.fillAmount = timer / skill.cooldown;
            yield return null;
        }

        skill.currentCooldown = 0;
        skill.image.fillAmount = 0f;
    }

    public IEnumerator RemoveBoostAfterTime(PlayerController player, int timeSkill, string typeOfBoost, float originalHealth, float originalDamage, float originalRange, float originalAttackTimer, float originalSpeed, float originalAcceleration, bool originalImmunity)
    {
        yield return new WaitForSeconds(timeSkill);

        switch (typeOfBoost)
        {
            case "health":
                player.health = originalHealth;
            break;
            case "damage":
                player.damage = originalDamage;
            break;
            case "range":
                player.range = originalRange;
            break;
            case "rangeAttack":
                player.range = originalRange;
                player.damage = originalDamage;
            break;
            case "attackRate":
                player.realAttackTimer = originalAttackTimer;
            break;
            case "immunity":
                player.immunity = originalImmunity;
            break;
            case "speed":
                player.myNavMeshAgent.speed = originalSpeed;
            break;
            case "acceleration":
                player.myNavMeshAgent.acceleration = originalAcceleration;
            break;
            case "fastCombo":
                player.myNavMeshAgent.speed = originalSpeed;
                player.myNavMeshAgent.acceleration = originalAcceleration;
            break;
            case "insanity":
                player.health = originalHealth;
                player.damage = originalDamage;
                player.range = originalRange;
            break;
        }
    }

    public IEnumerator ReturnToBaseCooldown()
    {
        returnToBaseCooldownTimer = 0f;
        returnToBaseCooldownImage.fillAmount = 1f; 

        while (returnToBaseCooldownTimer < 5f)
        {
            returnToBaseCooldownTimer += Time.deltaTime;
            float progress = returnToBaseCooldownTimer / 5f;
            returnToBaseCooldownImage.fillAmount = Mathf.Lerp(1f, 0f, progress);
            yield return null;
        }

        returnToBaseCooldownImage.fillAmount = 0f;
        returnToBaseCooldownTimer = 0f;
    }
 

    public void ReturnToBase()
    {
        if(returnToBaseCooldownTimer <= 0)
        {  
            myNavMeshAgent.ResetPath();
            myNavMeshAgent.enabled = false;
            hasTarget = false; 
            this.gameObject.transform.position = playerSpawnPosition;
            myNavMeshAgent.enabled = true;

            StartCoroutine(ReturnToBaseCooldown());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BaseZone") && other.gameObject.layer == teamLayer)
        {
            shopCanvasButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BaseZone") && other.gameObject.layer == teamLayer)
        {
            shopCanvasButton.SetActive(false);
        }
    }

    IEnumerator UpdateLine()
    {
        while (currentLineObject != null && moveIconReference != null)
        {
            Vector3 dir = (moveIconReference.transform.position - transform.position).normalized;
            Vector3 originOffset = transform.position + dir * 3f;
            Vector3 targetOffset = moveIconReference.transform.position - dir * 5f;
            lineRenderer.SetPosition(0, originOffset);
            lineRenderer.SetPosition(1, targetOffset);
            yield return null;
        } 
    }


    IEnumerator ReHealth()
    {
        yield return new WaitForSeconds(5f);
        float lastHealth = health;

        while (health < 100)
        {
            if (lastHealth == health)
            {
                health += 1;
                lastHealth = health;
                yield return new WaitForSeconds(1f);
            }
            else
            {
                healthRecuperationFlag = true;
                yield break;
            }
        }

        healthRecuperationFlag = true;
    }


    IEnumerator Death()
    {
        float time = 5;
        anim.SetBool("Death", true);
        audioSource.PlayOneShot(audioClipDeath);
        deathPanel.SetActive(true);

        myNavMeshAgent.enabled = false;

        while(time > 0)
        {
            respawnTimer.text = "Respawn in: " + time.ToString("F2");
            time -= Time.deltaTime;
            yield return null;
        }

        this.transform.position = playerSpawnPosition;

        health = 100;
        anim.SetBool("Death", false);
        anim.ResetTrigger("Fight");
        deathPanel.SetActive(false);

        myNavMeshAgent.enabled = true;
        hasTarget = false;
        myNavMeshAgent.ResetPath();  
        isDead = false;

        target = null;

        yield return new WaitForSeconds(0.5f);
    }

}
