using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

// ========================
//   INVENTORY MANAGER
// ========================
public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, Item> skills = new Dictionary<string, Item>();
    public Dictionary<string, Item> upgradeItems = new Dictionary<string, Item>();
    public Dictionary<string, Item> characterGems = new Dictionary<string, Item>(); 
    public Dictionary<string, Inventory> inventory = new Dictionary<string, Inventory>();
    public Dictionary<string, Character> charactersInventory = new Dictionary<string, Character>();

    public Sprite[] objectSpriteSkills;
    public Sprite[] objectSpriteUpgradeItem;

    public PlayerController player; 

    void Start()
    {
        SkillsInitialization();
        UpgradeItemInitialization();
        CharacterGemsInitialization();
    }

    void SkillsInitialization()
    {
        // Characters Exclusive Skills
        skills.Add("Linganguli", new Skill("Linganguli", 0, 6, 3, objectSpriteSkills[0], null, 1.25f, "damage", true));
        skills.Add("Sauce", new Skill("Sauce", 0, 30, 15, objectSpriteSkills[1], null, 1.25f, "speed", true));
        skills.Add("Phone", new Skill("Phone", 0, 60, 5, objectSpriteSkills[2], null, 1.2f, "insanity", true));

        skills.Add("Bat", new Skill("Bat", 0, 9, 3, objectSpriteSkills[3], null, 1.3f, "damage", true));
        skills.Add("Fire", new Skill("Fire", 0, 25, 10, objectSpriteSkills[4], null, 1.2f, "acceleration", true));
        skills.Add("WoodShield", new Skill("WoodShield", 0, 60, 5, objectSpriteSkills[5], null, 0f, "immunity", true));

        skills.Add("Motor", new Skill("Motor", 0, 10, 1, objectSpriteSkills[6], null, 1.75f, "damage", true));
        skills.Add("Wheels", new Skill("Wheels", 0, 60, 15, objectSpriteSkills[7], null, 1.35f, "acceleration", true));
        skills.Add("Super Fuel", new Skill("Super Fuel", 0, 60, 5, objectSpriteSkills[8], null, 1.1f, "insanity", true));

        skills.Add("Gun", new Skill("Gun", 0, 12, 2, objectSpriteSkills[9], null, 1.5f, "rangeAttack", true));
        skills.Add("Gold Necklace", new Skill("Gold Necklace", 0, 45, 15, objectSpriteSkills[10], null, 1.1f, "fastCombo", true));
        skills.Add("TattoMachine", new Skill("TattoMachine", 0, 60, 15, objectSpriteSkills[11], null, 1.5f, "health", true));

        skills.Add("Stocks", new Skill("Stocks", 0, 12, 2, objectSpriteSkills[12], null, 1.5f, "damage", true));
        skills.Add("Money", new Skill("Money", 0, 45, 10, objectSpriteSkills[1], null, 1.5f, "speed", true));
        skills.Add("Stock Market", new Skill("Stock Market", 0, 45, 5, objectSpriteSkills[14], null, 0f, "attackRate", true));

        GameData.Instance.characterSkills = skills.Values.OfType<Skill>().Take(15).ToList();

        // Normal Skills
        skills.Add("AI", new Skill("AI", 0, 0, 0, objectSpriteSkills[15], null, 0f, "", false));
        skills.Add("Energy Drink", new Skill("Energy Drink", 0, 0, 0, objectSpriteSkills[16], null, 0f, "", false));
        skills.Add("Hotdog", new Skill("Hotdog", 0, 0, 0, objectSpriteSkills[17], null, 0f, "", false));
        skills.Add("Looksmaxing", new Skill("Looksmaxing", 0, 0, 0, objectSpriteSkills[18], null, 0f, "", false));
        skills.Add("Mayonnasie", new Skill("Mayonnasie", 0, 0, 0, objectSpriteSkills[19], null, 0f, "", false));
        skills.Add("Ohio", new Skill("Ohio", 0, 0, 0, objectSpriteSkills[20], null, 0f, "", false));
        skills.Add("Ramen", new Skill("Ramen", 0, 0, 0, objectSpriteSkills[21], null, 0f, "", false));
        skills.Add("Rizz", new Skill("Rizz", 0, 0, 0, objectSpriteSkills[22], null, 0f, "", false));
        skills.Add("Screwdriver", new Skill("Screwdriver", 0, 0, 0, objectSpriteSkills[23], null, 0f, "", false));

        // Special Skill
        skills.Add("Sigma", new Skill("Sigma", 0, 0, 0, objectSpriteSkills[24], null, 0f, "", true));
    }

    void UpgradeItemInitialization()
    {
        upgradeItems.Add("XP Bottle", new UpgradeItem("XP Bottle", 0, objectSpriteUpgradeItem[0], null, "XP"));
        upgradeItems.Add("StarDust Bottle", new UpgradeItem("StarDust Bottle", 0, objectSpriteUpgradeItem[1], null, "StarDust"));        
    }

    void CharacterGemsInitialization()
    {
        characterGems.Add("Sr. Pollo Gem", new UpgradeItem("Sr. Pollo Gem", 0, objectSpriteUpgradeItem[2], null, "UpgradeGem"));
        characterGems.Add("Tung tung tung Sahur Gem", new UpgradeItem("Tung tung tung Sahur Gem", 0, objectSpriteUpgradeItem[3], null, "UpgradeGem"));
        characterGems.Add("EA 68 Gem", new UpgradeItem("EA 68 Gem", 0, objectSpriteUpgradeItem[4], null, "UpgradeGem"));
        characterGems.Add("Quandle Gem", new UpgradeItem("Quandle Gem", 0, objectSpriteUpgradeItem[5], null, "UpgradeGem"));
        characterGems.Add("Stonks Man Gem", new UpgradeItem("Stonks Man Gem", 0, objectSpriteUpgradeItem[6], null, "UpgradeGem"));
    }

    // Ejemplo: Llamar desde botón o evento
    public void UseSkill(string itemName)
    {
        if (skills.ContainsKey(itemName))
        {
            skills[itemName].Consume(player);
            Debug.Log($"Consumido {itemName}");
        }
        else
        {
            Debug.LogWarning($"No existe el item {itemName}");
        }
    }
}


// ========================
//          ITEM
// ========================
[Serializable]
public abstract class Item
{
    public string name;
    public int amount;
    public Sprite objectSprite;
    public Image objectImage;

    public Item(string name, int amount, Sprite sprite, Image image)
    {
        this.name = name;
        this.amount = amount;
        this.objectSprite = sprite;
        this.objectImage = image;
    }

    // Only for objects usable in game 
    public virtual void Consume(PlayerController player){} 
    public virtual void ConsumeBot(BotController player){} 

    //Only for objects of statistical use or to improve things.
    public virtual void Use(Character character){}
}

// ========================
//      SKILL ITEM
// ========================
[Serializable]
public class Skill : Item
{
    public int cooldown;
    public int timeSkill;
    public float boost;
    public string typeOfBoost;
    public bool isAcquired = false;
    public bool isCharacterExclusive;
    public Image image; 
    public Sprite sprite; 

    public int currentCooldown = 0;

    public Skill(string name, int amount, int cooldown, int timeSkill, Sprite sprite, Image image, float boost, string typeOfBoost, bool isCharacterExclusive)
        : base(name, amount, sprite, image)
    {
        this.cooldown = cooldown;
        this.timeSkill = timeSkill;
        this.boost = boost;
        this.typeOfBoost = typeOfBoost;
        this.isCharacterExclusive = isCharacterExclusive;
        this.image = image;
        this.sprite = sprite;
    }

    public override void Consume(PlayerController player)
    {
        if (currentCooldown > 0) return;

        currentCooldown = cooldown;
        player.StartCoroutine(player.Cooldown(this));

        float originalHealth = player.health;
        float originalDamage = player.damage;
        float originalRange = player.range;
        float originalAttackTimer = player.attackTimer;
        float originalSpeed = player.myNavMeshAgent.speed;
        float originalAcceleration = player.myNavMeshAgent.acceleration;
        bool originalImmunity = player.immunity;

        switch (typeOfBoost)
        {
            case "health":
                player.health *= boost;
                break;
            case "damage":
                player.damage *= boost;
                break;
            case "range":
                player.range *= boost;
                break;
            case "rangeAttack":
                player.range *= boost;
                player.damage *= boost;
                break;
            case "attackRate":
                player.realAttackTimer *= boost;
                break;
            case "immunity":
                player.immunity = true;
                break;
            case "speed":
                player.myNavMeshAgent.speed *= boost;
                break;
            case "acceleration":
                player.myNavMeshAgent.acceleration *= boost;
                break;
            case "fastCombo":
                player.myNavMeshAgent.speed *= boost;
                player.myNavMeshAgent.acceleration *= boost;
                break;
            case "insanity":
                player.health *= boost;
                player.damage *= boost;
                player.range *= boost;
                break;
            default:
                Debug.LogWarning("Boost desconocido: " + typeOfBoost);
                break;
        }

        player.StartCoroutine(player.RemoveBoostAfterTime(player, timeSkill, typeOfBoost, originalHealth, originalDamage, originalRange, originalAttackTimer, originalSpeed, originalAcceleration, originalImmunity));
    }

    public override void ConsumeBot(BotController player)
    {
        if (currentCooldown > 0) return;

        currentCooldown = cooldown;
        player.StartCoroutine(player.Cooldown(this));

        float originalHealth = player.health;
        float originalDamage = player.damage;
        float originalRange = player.range;
        float originalAttackTimer = player.attackTimer;
        float originalSpeed = player.myNavMeshAgent.speed;
        float originalAcceleration = player.myNavMeshAgent.acceleration;
        bool originalImmunity = player.immunity;

        switch (typeOfBoost)
        {
            case "health":
                player.health *= boost;
                break;
            case "damage":
                player.damage *= boost;
                break;
            case "range":
                player.range *= boost;
                break;
            case "rangeAttack":
                player.range *= boost;
                player.damage *= boost;
                break;
            case "attackRate":
                player.realAttackTimer *= boost;
                break;
            case "immunity":
                player.immunity = true;
                break;
            case "speed":
                player.myNavMeshAgent.speed *= boost;
                break;
            case "acceleration":
                player.myNavMeshAgent.acceleration *= boost;
                break;
            case "fastCombo":
                player.myNavMeshAgent.speed *= boost;
                player.myNavMeshAgent.acceleration *= boost;
                break;
            case "insanity":
                player.health *= boost;
                player.damage *= boost;
                player.range *= boost;
                break;
            default:
                Debug.LogWarning("Boost desconocido: " + typeOfBoost);
                break;
        }

        player.StartCoroutine(player.RemoveBoostAfterTimeBot(player, timeSkill, typeOfBoost, originalHealth, originalDamage, originalRange, originalAttackTimer, originalSpeed, originalAcceleration, originalImmunity));
    }
}

[Serializable]
public class UpgradeItem : Item
{
    public string typeOfUpgradeItem;

    public UpgradeItem(string name, int amount, Sprite sprite, Image image, string typeOfUpgradeItem)
        : base(name, amount, sprite, image)
    {
        this.typeOfUpgradeItem = typeOfUpgradeItem;
    }

    public override void Use(Character character)
    {
        switch (typeOfUpgradeItem)
        {
            case "XP":
                GameData.Instance.experience += 1;
                break;
            case "StarDust":
                // Implement 
            break;
            case "UpgradeGem":
                character.level++; 
            break;
            default:
                Debug.LogWarning("Boost desconocido: " + typeOfUpgradeItem);
                break;
        }
    }
}

// ========================
//      INVENTORY CLASS
// ========================
[Serializable]
public class Inventory
{
    public string name;
    public int amount;
    public Item item;

    public Inventory(string name, int amount, Item item)
    {
        this.name = name;
        this.amount = amount;
        this.item = item;
    }
}
