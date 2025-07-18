using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject minionPrefab;
    public GameObject turretPrefab;
    public GameObject CoreA;
    public GameObject CoreB;

    [SerializeField] private GameObject[] FireWallsA;
    [SerializeField] private GameObject[] FireWallsB;

    private bool finish = false;
    private GameObject[] turrets = new GameObject[12];
    public GameObject[] turretsColocation;

    Vector3 A_SpawnLocation = new Vector3(-260, 2, 260);
    Vector3 B_SpawnLocation = new Vector3(260, 2, -260);
    Vector3 Top_SpawnLocation = new Vector3(260, 2, 260);
    Vector3 Bottom_SpawnLocation = new Vector3(-260, 2, -260);

    bool spawnFlag = true;
    float raidTimer = 0f;

    [SerializeField] private GameObject shopPanel;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [SerializeField] private TMP_Text coinsText;

    Animator anim;
    PlayerController playerController;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        anim = playerController.anim;

        for (int i = 0; i < turretsColocation.Length; i++)
        {
            turrets[i] = Instantiate(
                turretPrefab,
                turretsColocation[i].transform.position,
                turretsColocation[i].transform.rotation
            );

            turrets[i].GetComponent<TurretManager>().team = (i < 6) ? 'B' : 'A';
        }

        CoreA.GetComponent<CoreManager>().team = 'A';
        CoreB.GetComponent<CoreManager>().team = 'B';
    }

    void Update()
    {
        if (CoreA == null && !finish) Win('A');
        if (CoreB == null && !finish) Win('B');

        raidTimer += Time.deltaTime;
        if (raidTimer >= 60f)
        {
            raidTimer = 0;
            spawnFlag = true;
        }

        coinsText.text = GameData.Instance.coins.ToString();

        if (spawnFlag)
        {
            GameObject minionSpawned;

            // TEAM A
            for (int i = 0; i < 3; i++)
            {
                minionSpawned = Instantiate(minionPrefab, A_SpawnLocation, Quaternion.identity);
                minionSpawned.GetComponent<MinionAiScript>().team = 'A';
                minionSpawned.GetComponent<MinionAiScript>().destination = B_SpawnLocation;

                minionSpawned = Instantiate(minionPrefab, A_SpawnLocation, Quaternion.identity);
                minionSpawned.GetComponent<MinionAiScript>().team = 'A';
                minionSpawned.GetComponent<MinionAiScript>().destination = Bottom_SpawnLocation;
                minionSpawned.GetComponent<MinionAiScript>().finalDestination = B_SpawnLocation;

                minionSpawned = Instantiate(minionPrefab, A_SpawnLocation, Quaternion.identity);
                minionSpawned.GetComponent<MinionAiScript>().team = 'A';
                minionSpawned.GetComponent<MinionAiScript>().destination = Top_SpawnLocation;
                minionSpawned.GetComponent<MinionAiScript>().finalDestination = B_SpawnLocation;
            }

            // TEAM B
            for (int i = 0; i < 3; i++)
            {
                minionSpawned = Instantiate(minionPrefab, B_SpawnLocation, Quaternion.identity);
                minionSpawned.GetComponent<MinionAiScript>().team = 'B';
                minionSpawned.GetComponent<MinionAiScript>().destination = A_SpawnLocation;

                minionSpawned = Instantiate(minionPrefab, B_SpawnLocation, Quaternion.identity);
                minionSpawned.GetComponent<MinionAiScript>().team = 'B';
                minionSpawned.GetComponent<MinionAiScript>().destination = Bottom_SpawnLocation;
                minionSpawned.GetComponent<MinionAiScript>().finalDestination = A_SpawnLocation;

                minionSpawned = Instantiate(minionPrefab, B_SpawnLocation, Quaternion.identity);
                minionSpawned.GetComponent<MinionAiScript>().team = 'B';
                minionSpawned.GetComponent<MinionAiScript>().destination = Top_SpawnLocation;
                minionSpawned.GetComponent<MinionAiScript>().finalDestination = A_SpawnLocation;
            }

            spawnFlag = false;
        }
    }

    void Win(char team)
    {
        if (playerController.playerTeam != team)
        {
            finish = true;
            anim.SetBool("Win", true);
            winPanel.SetActive(true);
            GameData.Instance.money += 250;
            GameData.Instance.experience += 5f;
        }
        else
        {
            finish = true;
            anim.SetBool("Lose", true);
            losePanel.SetActive(true);
            GameData.Instance.money += 50;
            GameData.Instance.experience += 1f;
        }
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    public void BuyTopFireWall()
    {
        if(GameData.Instance.coins >= 250)
        {
            if(playerController.playerTeam == 'A') FireWallsA[0].SetActive(true);
            if(playerController.playerTeam == 'B') FireWallsB[0].SetActive(true);
        }
    }

    public void BuyMidFireWall()
    {
        if(GameData.Instance.coins >= 250)
        {
            if(playerController.playerTeam == 'A') FireWallsA[1].SetActive(true);
            if(playerController.playerTeam == 'B') FireWallsB[1].SetActive(true);
        }
    }

    public void BuyBotFireWall()
    {
        if(GameData.Instance.coins >= 250)
        {
            if(playerController.playerTeam == 'A') FireWallsA[2].SetActive(true);
            if(playerController.playerTeam == 'B') FireWallsB[2].SetActive(true);
        }
    }

    public void BuyItemSlot_1()
    {
        if(GameData.Instance.coins >= 30)
        {
            playerController.damage += 3f;
        }
    }

    public void BuyItemSlot_2()
    {
        if(GameData.Instance.coins >= 50)
        {
            playerController.health += 10f;
        }
    }

    public void BuyItemSlot_3()
    {
        if(GameData.Instance.coins >= 45)
        {
            playerController.realAttackTimer -= 0.5f;
        }
    }

    public void BuyItemSlot_4()
    {
        if(GameData.Instance.coins >= 25)
        {
            playerController.myNavMeshAgent.speed += 8f;
        }
    }

    public void BuyItemSlot_5()
    {
        if(GameData.Instance.coins >= 35)
        {
            playerController.range += 2f;
        }
    }

    public void BuyItemSlot_6()
    {
        if(GameData.Instance.coins >= 32)
        {
            playerController.myNavMeshAgent.acceleration += 5f;
        }
    }

    public void BuyItemSlot_7()
    {
        if(GameData.Instance.coins >= 55)
        {
            playerController.characterSkill_3.cooldown -= 5;
        }
    }

    public void BuyItemSlot_8()
    {
        if(GameData.Instance.coins >= 60)
        {
            playerController.characterSkill_1.boost += 0.5f;
        }
    }

    public void BuyItemSlot_9()
    {
        if(GameData.Instance.coins >= 39)
        {
            playerController.minionKillValue += 0.5f;
        }
    }
}

/*
                                                                      :-                            
                                                                    .*##*                           
                                                                   +#####*                          
                          =**+:                                 .=*#######+                         
                         =*****+=.                            :+###%%%%%%%%+                        
                        +*#####****=:                       -*##%%%%%%%%%%%#-                       
                       -###%%######***:                   =###%%%%%%%%%%####*                       
                       *##%%%%%#%%%%##*+.               =###%%%%%%%%%%#######.                      
                      =#%####%%%%%%%%%##*=            =###%%%%%%%%%%#########:                      
                      **#***###%%%%%%%%%##*:        -*##%%%%@@@%%%###########=                      
                     .**********##%%%%%%@%##*:    .*##%%%%@@@%%####*#########*                      
                     -#************##%%%%@@%%#*...###%%@@@@%###****#####%%%%%#.                     
                     -###########*****##%%%%%%#***#%%@@%%*****#####%%%%%%%%%%%-                     
                     -##%%%%%%%%%%%%%######*########***#######%%%%%%@@@@@%%%%%-                     
                     :#%%%%%%%%@@@@@@@@@@@@@@%##%%%%@@@@@@@@@@@@@@@@@@@@%%%%%%:                     
                      *###%%%%%%%%%###########*+++**#####%%%%%@@@@@@@%%%%%####                      
                      -*###****************+=..........:=+########%%%%%%%%%%#=                      
                       +*****#****####*=:..................-*################.                      
                       .*******#####=:.......................:+%@%##########=                       
                         **####*#+:............................-#@%%%%######                        
                         .*###*+:...............................:+%%%%@*...                         
                           =#+-:.................................:=%%+.                             
                           -#:.........:................=:..........                                
                            +........:=-...............:::=:.......                                 
                             .......-=.................:...--.......                                
                            .......-=.................:....:=:.......                               
                           .......:+=-:...............----==-=.......                               
                           .......-=:..:::-:........:=-:..::-=-......                               
                           .......=##++=+==---....:=***+*#%#===.......                              
                            ......==%=%###%@=:-====@##%###%---=.......                              
                           .......=-*####*#*...:--:=%####%%---=......-:                             
                          ........---%###*%:.......:*%###%+---=..... ...                            
                            .:...:--::=++=:..........=+*+=----+....                                 
                             =-..--:::::::...........:::---::=*..-:                                 
                              =--+*-::::::::::::::::::---:--=*-  :=                                 
                                .**=+--:::::-++++=--------------.                                   
                                 .---++*+++==--==---===+#===----.                                   
                                  -:--=#%##**##****##*%+:::...-:                                    
                               ..::..:=****##*###**#**#.::. ........                                
                           .....:::.:::=#####%@%%%####%=.....:..........                            
                         ......::-=---+*****%%#####*####==-.==--:::......                           
                         ...::-=++==+*#***%%#######%#####%#*+++::---::::.                           
                            :=+*****#####%%########%%%%%#####**=:.                                  
                          :+*******##############***#####**##******=-:                              
                          ...=*****############****######*******+=====--.                           
                            -***********######*******#####******.  .   ..                           
                           :******************#**********####***:                                   
                           .......     :+**********+--**###*#*#*-                                   
                                         -******=:.    .:-=++***-  .                                
                               ....       .=**+:               ...:::.                              
                              ......:.      ::                .:......:.                            
                             ......:::--:                   :--...:..:::                            
                             .......::.                       .::::::::.                            
                             .:.::::::                          .::::.                              
                               ....                                                                 
*/
