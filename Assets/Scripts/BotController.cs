using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using System.IO;
using UnityEngine.Audio;

public class BotController : MonoBehaviour
{
    #region Essential
    private GameObject characterModelInstance;
    private DecisionBotController decisionController;
    public CharacterState currentState;
    int layerTeam_A = 9;
    int layerTeam_B = 10;
    int layerPlayerTeam_A = 11;
    int layerPlayerTeam_B = 12;
    int jungleLayer = 15;
    int layerEnemyPlayer;
    int enemyLayer;
    int teamLayer;
    string playerName;
    [HideInInspector] public char playerTeam;
    int playerLevel = 1;
    Vector3[] MinionJungle_SpawnLocations = 
    {
        new Vector3(-180, 0, -135),
        new Vector3(-75, 0, -125),
        new Vector3(65, 0, -200),
        new Vector3(180, 0, 0),
        new Vector3(25, 0, 175),
        new Vector3(-45, 0, 165),
        new Vector3(-65, 0, -200)
    };

    [SerializeField] private GameObject[] characterPrefab;
    [SerializeField] private Avatar[] characterAvatarMask;

    Vector3 playerPositionTeamA = new Vector3(-245f, 2.5f, 245f);
    Vector3 playerPositionTeamB = new Vector3(245f, 2.5f, -245f);
    Vector3 playerSpawnPosition;

    [SerializeField] private GameObject playerSkinObject;

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerLevelText;

    [HideInInspector] public NavMeshAgent myNavMeshAgent;
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
    #endregion

    #region Animations & Effects
    [HideInInspector] public Animator anim;
    [SerializeField] private GameObject punchEffectPrefab;
    #endregion

    #region Skills
    [HideInInspector] public Skill characterSkill_1;
    [HideInInspector] public Skill characterSkill_2;
    [HideInInspector] public Skill characterSkill_3;
    #endregion

    #region AI
    private bool hasDecision = true;
    private float decisionTime = 30f;
    private float decisionTimer = 0f;
    Character character;
    private List<GameObject> validTargets = new List<GameObject>();
    #endregion

    string[] randomNames = {
    "KoiBuff", "WispMeme", "LyraReal", "KojiSpin", "MochiDrama", "YenoToast", "NyaaCry", "SuzuWin", "XDSpinny", "ZoroLOL",
    "IroXD", "MinuMood", "RokuUwU", "TakaCake", "RioBae", "PingChan", "YukiGhost", "NekoSimp", "MisoLurk", "TomoToast",
    "KaiCry", "TaviLOL", "ChiiSoft", "MemeDrama", "YeetZoom", "HanaUwU", "TheoSpin", "ZeroGhost", "KiraFlex", "JunXD",
    "NoScopePapi", "Error404Life", "ToiletSniper", "GigaLagger", "AltF4Daddy", "KDAOver9000", "PingOfDeath", "CringeLord98",
    "RAMnDIE", "69FPSOnly", "CryptoFarmer", "BuggedSoul", "Trollencio", "ShrekMeta", "ZoomerOnCrack", "TacoGamer", "XDterminator",
    "RageQuitChan", "BoomerLOL", "MemeMage", "KFCCleric", "GPUburner", "SpaghettiCoder", "HardstuckIron", "404Wins",
    "SnaccDealer", "DripDemon", "LoreInvented", "NoSkillAllKill", "NPCvibes", "TikTokOverlord", "AltTabber", "Cringe.exe",
    "CtrlAltDelux", "SleepyDDOS", "RAMSnack", "DracoManco", "TeemoTax", "SkibidiElite", "McLagFace", "RTXBrokeMe",
    "TrueDamageIRL", "ErrorOnBoot", "FrameDropper", "VolatileMeme", "LagGod", "YassifiedDoom", "ASCIIThug", "InputDelayPro",
    "ZuckerSmurf", "GPUlessBoy", "SniperInBathroom", "DeadByPing", "TrollBuffed", "DegenOverdrive", "VapeMage", "TikTokProphet",
    "GosuOnPotato", "BugCollector", "PatchAbuser", "InfiniteDesync", "ShaderDealer", "JankGod", "CursedKDA", "MetaReject",
    "HardReset", "Shrekcore", "SkillIssue", "TrashHero69", "UnpatchedGlitch", "SteamRefund", "BetaTester666", "SSDOverheat",
    "LootboxLover", "AnimeTaxEvader", "LowIQClutch", "NoAimNoShame", "SussyMage", "MalwareBae", "HackermanXD", "DegenerateMain",
    "BasedAF", "LagSpikeSama", "BugSnack", "FrameDropKing", "WiFiSummoner", "BrokenCombo", "PatchVictim", "NoPatchOnlyBugs",
    "RAMstealer", "JankTamer", "AIOverlord", "CooldownClown", "ZeroPingLiar", "VisualGlitcher", "ToxicMetaBoy"
    };

    void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    void Start()
    {
        playerName = randomNames[Random.Range(0, randomNames.Length)];
        playerNameText.text = playerName;
        playerLevelText.text = playerLevel.ToString();

        healthSlider.maxValue = health;
        easeHealthSlider.maxValue = health;

        anim = GetComponent<Animator>();

        Character[] characters = new Character[]
        {
            // string Name, int Index, int Level, int Health, int AttackPower, int AttackTimer, int Speed, bool IsHumanoid, Vector3 ModelScale, Vector3 ModelPosition, bool IsUnlocked, GameObject CharacterModel, Avatar Avatar, string MusicGenre, Texture2D CharacterIcon
            new Character("Sr. Pollo", 1, 1, 100, 25, 2, 15, true, new Vector3(1, 1, 1), new Vector3(0, 0, 0), true, characterPrefab[0], characterAvatarMask[0], "phonk", null),
            new Character("Tung tung tung Sahur", 2, 1, 75, 30, 3, 15, true, new Vector3(1, 1, 1), new Vector3(0, 0, 0), true, characterPrefab[1], characterAvatarMask[1], "hardstyle", null),
            new Character("EA 68", 3, 1, 125, 35, 8, 30, false, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, -1), true, characterPrefab[2], /*null*/ characterAvatarMask[2], "eurobeat", null),
            new Character("Quandle", 4, 1, 100, 20, 1, 20, true, new Vector3(1, 1, 1), new Vector3(0, 0, 0), true, characterPrefab[3], characterAvatarMask[3], "trap", null),
            new Character("Stonks Man", 5, 1, 150, 25, 2, 20, true, new Vector3(1, 1, 1), new Vector3(0, 0, 0), true, characterPrefab[4], characterAvatarMask[4], "dubstep", null) // Stonks Man hasn't the mesh vertex reduction in Blender
        };

        character = characters[Random.Range(0, characters.Length)];

        characterModelInstance = Instantiate(character.characterModel, playerSkinObject.transform);

        health = character.health;
        damage = character.attackPower;
        attackTimer = character.attackTimer;
        realAttackTimer = attackTimer;
        myNavMeshAgent.speed = character.speed;
        anim.avatar = character.avatar;

        decisionController = new DecisionBotController();
        decisionController.Initialize();

        decisionController.currentState = CharacterState.Base;
        currentState = decisionController.currentState;

        SkillInitialization();
    }

    public void Initialize(char team)
    {
        myNavMeshAgent.enabled = false;
        if (team == 'A')
        {
            playerTeam = 'A';
            playerNameText.color = new Color(1f, 0.6f, 0.6f);
            this.gameObject.layer = layerPlayerTeam_A;
            layerEnemyPlayer = layerPlayerTeam_B;
            enemyLayer = layerTeam_B;
            teamLayer = layerTeam_A;
            playerSpawnPosition = playerPositionTeamA;
            this.gameObject.transform.position = playerSpawnPosition;
        }
        else if(team == 'B')
        {
            playerTeam = 'B';
            playerNameText.color = new Color(0.6f, 1f, 1f);
            this.gameObject.layer = layerPlayerTeam_B;
            layerEnemyPlayer = layerPlayerTeam_A;
            enemyLayer = layerTeam_A;
            teamLayer = layerTeam_B;
            playerSpawnPosition = playerPositionTeamB;
            this.gameObject.transform.position = playerSpawnPosition;
        }
        myNavMeshAgent.enabled = true;
        hasDecision = false;
    }

    void SkillInitialization()
    {
        List<Skill> skills = GameData.Instance.characterSkills;
        int startIndex = (GameData.Instance.currentCharacter.index - 1) * 3;

        characterSkill_1 = skills[startIndex];
        characterSkill_2 = skills[startIndex + 1];
        characterSkill_3 = skills[startIndex + 2];

        StartCoroutine(Cooldown(characterSkill_1));
        StartCoroutine(Cooldown(characterSkill_2));
        StartCoroutine(Cooldown(characterSkill_3));
    }


    void Update()
    {
        if (isDead) return;

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0 && !hasDecision)
        {
            decisionTimer = decisionTime;
            MakeDecision();
        }

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

        // Animations  
        bool isMoving = myNavMeshAgent.hasPath && myNavMeshAgent.remainingDistance > myNavMeshAgent.stoppingDistance;
        anim.SetBool("Move", isMoving);

        if (validTargets.Count > 0)
        {
            target = validTargets[0]; 
            hasTarget = true;
        }

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

                Vector3 lookDirection = (target.transform.position - transform.position).normalized;
                lookDirection.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 10f);
                attackTimer -= Time.deltaTime;

                currentState = CharacterState.Attack;

                if(attackTimer <= 0)
                {
                    attackTimer = realAttackTimer;

                    if(target.TryGetComponent<MinionAiScript>(out MinionAiScript minion))
                    {
                        minion.health -= damage;
                    }

                    if(target.TryGetComponent<JungleBossAI>(out JungleBossAI jungleBoss))
                    {
                        jungleBoss.health -= damage;
                    }

                    if(target.TryGetComponent<MinionJungleAi>(out MinionJungleAi minionJungle))
                    {
                        minionJungle.health -= damage;
                    }

                    if(target.TryGetComponent<TurretManager>(out TurretManager turret))
                    {
                        turret.health -= damage;
                    }

                    if(target.TryGetComponent<CoreManager>(out CoreManager core))
                    {
                        core.health -= damage;
                    }

                    if(target.TryGetComponent<BotController>(out BotController enemyBotPlayer))
                    {
                        enemyBotPlayer.health -= damage;
                    }

                    if(target.TryGetComponent<BotController>(out BotController enemyPlayer))
                    {
                        enemyPlayer.health -= damage;
                    }

                    GameObject punchEffectObj = Instantiate(punchEffectPrefab, target.transform.position, Quaternion.identity);
                    anim.SetTrigger("Fight");
                    Destroy(punchEffectObj, 1.5f);
                }
            }
        }
    }

    void MakeDecision()
    {
        CharacterState nextState = decisionController.GetNextState();

        if (nextState != currentState)
        {
            currentState = nextState;
            decisionController.currentState = nextState;

            switch(currentState)
            {
                case CharacterState.Base:
                    ReturnToBase();
                break;

                case CharacterState.Farming:
                    GoFarm();
                break;

                case CharacterState.Following:
                    StartCoroutine(FollowAlly());
                break;

                case CharacterState.Attack:
                    AttackTarget();
                break;

                case CharacterState.TurretMision:
                    TurretMission();
                break;

                case CharacterState.CoreMision:
                    CoreMission();
                break;
            }
        }
    }

    void ReturnToBase()
    {
        myNavMeshAgent.ResetPath();
        myNavMeshAgent.enabled = false;
        hasTarget = false; 
        this.gameObject.transform.position = playerSpawnPosition;
        myNavMeshAgent.enabled = true;

        decisionTimer = 0f;
    }

    void GoFarm()
    {
        myNavMeshAgent.SetDestination(MinionJungle_SpawnLocations[Random.Range(0, MinionJungle_SpawnLocations.Length)]);
    }

    IEnumerator FollowAlly()
    {
        var playerObj = FindFirstObjectByType<MinionAiScript>();

        Vector3 offset = new Vector3(5f, 0, 5f);

        while(currentState == CharacterState.Following)
        {
            myNavMeshAgent.SetDestination(playerObj.gameObject.transform.position + offset);
            yield return null;
        }
    }

    void AttackTarget()
    {
        if(target == null && !hasTarget) decisionTimer = 0f;
    }

    void TurretMission()
    {
        bool exists = false;
        TurretManager[] turrets = FindObjectsOfType<TurretManager>();

        foreach (var turret in turrets)
        {
            if (turret.gameObject.layer == enemyLayer)
            {
                myNavMeshAgent.SetDestination(turret.gameObject.transform.position);

                break;
            }
        }

        if(!exists) decisionTimer = 0f;
    }

    void CoreMission()
    {
        if(playerTeam == 'A') myNavMeshAgent.SetDestination(new Vector3(280,0,-280));
        if(playerTeam == 'B') myNavMeshAgent.SetDestination(new Vector3(-280,0,280));
    }

    public void Skill_1()
    {
        characterSkill_1.ConsumeBot(this);
    }

    public void Skill_2()
    {
        characterSkill_2.ConsumeBot(this);
    }

    public void Skill_3()
    {
        characterSkill_3.ConsumeBot(this);
    }

    public IEnumerator Cooldown(Skill skill)
    {
        float timer = skill.cooldown;
        skill.currentCooldown = skill.cooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        skill.currentCooldown = 0;
    }

    public IEnumerator RemoveBoostAfterTimeBot(BotController player, int timeSkill, string typeOfBoost, float originalHealth, float originalDamage, float originalRange, float originalAttackTimer, float originalSpeed, float originalAcceleration, bool originalImmunity)
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == enemyLayer || other.gameObject.layer == jungleLayer || other.gameObject.layer == layerEnemyPlayer)
        {
            validTargets.Add(other.gameObject);
            hasTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (validTargets.Contains(other.gameObject))
        {
            validTargets.Remove(other.gameObject);
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

        myNavMeshAgent.enabled = false;

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        this.transform.position = playerSpawnPosition;

        health = 100;
        anim.SetBool("Death", false);
        anim.ResetTrigger("Fight");

        myNavMeshAgent.enabled = true;
        hasTarget = false;
        myNavMeshAgent.ResetPath();  
        isDead = false;

        target = null;

        yield return new WaitForSeconds(0.5f);
    }

}

public enum CharacterState
{
    Base,
    Farming,
    Following,
    Attack,
    TurretMision,
    CoreMision,
}

public class DecisionBotController
{
    public CharacterState currentState;

    private Dictionary<CharacterState, Dictionary<CharacterState, float>> stateWeights;

    public void Initialize()
    {
        stateWeights = new Dictionary<CharacterState, Dictionary<CharacterState, float>>();

        stateWeights[CharacterState.Base] = new Dictionary<CharacterState, float>()
        {
            { CharacterState.Farming, 40f },
            { CharacterState.Following, 30f },
            { CharacterState.Attack, 20f },
            { CharacterState.TurretMision, 5f },
            { CharacterState.CoreMision, 5f },
        };

        stateWeights[CharacterState.Farming] = new Dictionary<CharacterState, float>()
        {
            { CharacterState.Base, 50f },
            { CharacterState.Attack, 40f },
            { CharacterState.Following, 10f },
        };

        stateWeights[CharacterState.Following] = new Dictionary<CharacterState, float>()
        {
            { CharacterState.Attack, 50f },
            { CharacterState.Farming, 30f },
            { CharacterState.Base, 20f },
        };

        stateWeights[CharacterState.Attack] = new Dictionary<CharacterState, float>()
        {
            { CharacterState.Following, 60f },
            { CharacterState.TurretMision, 30f },
            { CharacterState.CoreMision, 10f },
        };

        stateWeights[CharacterState.TurretMision] = new Dictionary<CharacterState, float>()
        {
            { CharacterState.CoreMision, 70f },
            { CharacterState.Attack, 20f },
            { CharacterState.Base, 10f },
        };

        stateWeights[CharacterState.CoreMision] = new Dictionary<CharacterState, float>()
        {
            { CharacterState.Base, 100f },
        };
    }

    public CharacterState GetNextState()
    {
        if (!stateWeights.ContainsKey(currentState))
            return currentState; 

        var possibleTransitions = stateWeights[currentState];
        float totalWeight = 0f;

        foreach (var kvp in possibleTransitions)
            totalWeight += kvp.Value;

        float rand = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var kvp in possibleTransitions)
        {
            cumulative += kvp.Value;
            if (rand <= cumulative)
                return kvp.Key;
        }

        return currentState; 
    }
}
