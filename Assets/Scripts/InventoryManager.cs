using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

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
        skills.Add("Fidget spinner", new Skill("Fidget spinner", 0, 30, 15, objectSpriteSkills[0], null, 1.25f, "damage"));
        skills.Add("Pop it", new Skill("Pop it", 0, 20, 10, objectSpriteSkills[1], null, 1.5f, "life"));
        skills.Add("Squish Ball", new Skill("Squish Ball", 0, 15, 30, objectSpriteSkills[2], null, 0.5f, "attackRate"));
        skills.Add("Slime", new Skill("Slime", 0, 1, 60, objectSpriteSkills[3], null, 0.75f, "skillCooldown"));
        skills.Add("Baby Angel", new Skill("Baby Angel", 0, 5, 120, objectSpriteSkills[4], null, 0f, "immunity"));
        skills.Add("Reversible Octopus", new Skill("Reversible Octopus", 0, 35, 100, objectSpriteSkills[5], null, 1.75f, "range"));
        skills.Add("Squishy", new Skill("Squishy", 0, 30, 240, objectSpriteSkills[6], null, 1.5f, "acceleration"));
        skills.Add("Sharklas", new Skill("Sharklas", 0, 60, 180, objectSpriteSkills[7], null, 2f, "speed"));
        skills.Add("Labubu", new Skill("Labubu", 0, 15, 300, objectSpriteSkills[8], null, 2f, "insanity"));
    }

    void UpgradeItemInitialization()
    {
        upgradeItems.Add("XP Bottle", new UpgradeItem("XP Bottle", 0, objectSpriteUpgradeItem[0], null, "XP"));
        upgradeItems.Add("StarDust Bottle", new UpgradeItem("StarDust Bottle", 0, objectSpriteUpgradeItem[1], null, "StarDust"));        
    }

    void CharacterGemsInitialization()
    {
        characterGems.Add("Sr. Pollo Gem", new UpgradeItem("Sr. Pollo Gem", 0, objectSpriteUpgradeItem[0], null, "UpgradeGem"));
        characterGems.Add("Tung tung tung Sahur Gem", new UpgradeItem("Tung tung tung Sahur Gem", 0, objectSpriteUpgradeItem[1], null, "UpgradeGem"));
        characterGems.Add("EA 68 Gem", new UpgradeItem("EA 68 Gem", 0, objectSpriteUpgradeItem[0], null, "UpgradeGem"));
        characterGems.Add("Quandle Gem", new UpgradeItem("Quandle Gem", 0, objectSpriteUpgradeItem[1], null, "UpgradeGem"));
        characterGems.Add("Stonks Man Gem", new UpgradeItem("Stonks Man Gem", 0, objectSpriteUpgradeItem[0], null, "UpgradeGem"));
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
    public virtual  void Consume(PlayerController player){} 

    //Only for objects of statistical use or to improve things.
    public virtual  void Use(Character character){}
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

    public Skill(string name, int amount, int cooldown, int timeSkill, Sprite sprite, Image image, float boost, string typeOfBoost)
        : base(name, amount, sprite, image)
    {
        this.cooldown = cooldown;
        this.timeSkill = timeSkill;
        this.boost = boost;
        this.typeOfBoost = typeOfBoost;
    }

    public override void Consume(PlayerController player)
    {
        if (cooldown > 0) return;

        switch (typeOfBoost)
        {
            case "life":
                player.health *= boost;
                break;
            case "damage":
                player.damage *= boost;
                break;
            case "range":
                player.range *= boost;
                break;
            case "attackRate":
                player.attackTimer *= boost;
                break;
            case "skillCooldown":
                player.cooldownTimeSkill1 *= boost;
                player.cooldownTimeSkill2 *= boost;
                player.cooldownTimeSkill3 *= boost;
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
            case "insanity":
                player.health *= boost;
                player.damage *= boost;
                player.range *= boost;
                break;
            default:
                Debug.LogWarning("Boost desconocido: " + typeOfBoost);
                break;
        }
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
