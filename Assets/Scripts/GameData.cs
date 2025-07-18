using UnityEngine;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public string playerName;
    public int playerLevel;
    public float experience;
    public int money;

    public bool battlePassPurchased;
    public bool isOfferPackPurchased;

    public Character currentCharacter;

    public int pity;

    public float bgmVolume;
    public float sfxVolume;
    public float masterVolume;

    public bool buildingsDisappear;
    public bool isPhoneActive = true;

    public Dictionary<string, Item> item = new Dictionary<string, Item>();
    public Dictionary<string, Inventory> inventory = new Dictionary<string, Inventory>();
    public Dictionary<string, Character> charactersInventory = new Dictionary<string, Character>();

    // NOT SAVING BUT STATIC DATA //
    public List<Skill> characterSkills = new List<Skill>();
    public float coins;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetAllData()
    {
        playerName = "";
        playerLevel = 0;
        experience = 0;
        money = 0;
        pity = 0;
        battlePassPurchased = false;
        isOfferPackPurchased = false;
        currentCharacter = null;
        item.Clear();
        inventory.Clear();
        charactersInventory.Clear();
        bgmVolume = 1;
        sfxVolume = 1;
        masterVolume = 1;
    }
}
