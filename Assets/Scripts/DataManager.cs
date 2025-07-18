using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public GameSaveData gameSaveData = new GameSaveData();
    public string saveFile;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFile = Application.dataPath + "/GameData.json";
        }
        else
        {
            Destroy(gameObject);
        }

        LoadData(); 
    }

    void Start()
    {
        StartCoroutine(AutoSave());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) SaveData();
        if(Input.GetKeyDown(KeyCode.L)) LoadData();
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

    public void WipeAllData()
    {
        if (File.Exists(saveFile))
        {
            File.Delete(saveFile);
        }

        gameSaveData = new GameSaveData();

        GameData.Instance.ResetAllData();
        
        string profileImagePath = Application.persistentDataPath + "/profile.png";
        if (File.Exists(profileImagePath)) File.Delete(profileImagePath);

        SaveData();

        LoadData();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void LoadData()
    {
        if(File.Exists(saveFile))
        {
            string content = File.ReadAllText(saveFile);
            gameSaveData = JsonUtility.FromJson<GameSaveData>(content);

            ApplyLoadedData();
        }
    }

    public void SaveData()
    {
        SyncGameDataToSave();
        string jsonString = JsonUtility.ToJson(gameSaveData, true);
        File.WriteAllText(saveFile, jsonString);
    }

    private void SyncGameDataToSave()
    {
        gameSaveData.money = GameData.Instance.money;
        gameSaveData.battlePassPurchased = GameData.Instance.battlePassPurchased;
        gameSaveData.isOfferPackPurchased = GameData.Instance.isOfferPackPurchased;
        gameSaveData.playerName = GameData.Instance.playerName;
        gameSaveData.playerLevel = GameData.Instance.playerLevel;
        gameSaveData.experience = GameData.Instance.experience;
        gameSaveData.pity = GameData.Instance.pity;
        gameSaveData.bgmVolume = GameData.Instance.bgmVolume;
        gameSaveData.sfxVolume = GameData.Instance.sfxVolume;
        gameSaveData.masterVolume = GameData.Instance.masterVolume;
        gameSaveData.buildingsDisappear = GameData.Instance.buildingsDisappear;
        gameSaveData.isPhoneActive = GameData.Instance.isPhoneActive;

        gameSaveData.currentCharacter = GameData.Instance.currentCharacter;

        gameSaveData.itemList.Clear();
        foreach (var kvp in GameData.Instance.item)
        {
            gameSaveData.itemList.Add(new ItemEntry { key = kvp.Key, value = kvp.Value });
        }

        gameSaveData.inventory.Clear();
        foreach (var kvp in GameData.Instance.inventory)
        {
            gameSaveData.inventory.Add(new InventoryEntry { key = kvp.Key, value = kvp.Value });
        }

        gameSaveData.charactersInventory.Clear();
        foreach (var kvp in GameData.Instance.charactersInventory)
        {
            gameSaveData.charactersInventory.Add(new CharactersInventoryEntry { key = kvp.Key, value = kvp.Value });
        }
    }

    private void ApplyLoadedData()
    {
        GameData.Instance.money = gameSaveData.money;
        GameData.Instance.battlePassPurchased = gameSaveData.battlePassPurchased;
        GameData.Instance.isOfferPackPurchased = gameSaveData.isOfferPackPurchased;
        GameData.Instance.playerName = gameSaveData.playerName;
        GameData.Instance.playerLevel = gameSaveData.playerLevel;
        GameData.Instance.pity = gameSaveData.pity;
        GameData.Instance.experience = gameSaveData.experience;
        GameData.Instance.bgmVolume = gameSaveData.bgmVolume;
        GameData.Instance.sfxVolume = gameSaveData.sfxVolume;
        GameData.Instance.masterVolume = gameSaveData.masterVolume;
        GameData.Instance.buildingsDisappear = gameSaveData.buildingsDisappear;
        GameData.Instance.isPhoneActive = gameSaveData.isPhoneActive;

        GameData.Instance.currentCharacter = gameSaveData.currentCharacter;

        GameData.Instance.item.Clear();
        foreach (var entry in gameSaveData.itemList)
        {
            GameData.Instance.item.Add(entry.key, entry.value);
        }

        GameData.Instance.inventory.Clear();
        foreach (var entry in gameSaveData.inventory)
        {
            GameData.Instance.inventory.Add(entry.key, entry.value);
        }

        GameData.Instance.charactersInventory.Clear();
        foreach (var entry in gameSaveData.charactersInventory)
        {
            GameData.Instance.charactersInventory.Add(entry.key, entry.value);
        }
    }


    private IEnumerator AutoSave()
    {
        yield return new WaitForSeconds(5f);

        while(true)
        {
            SaveData();
            yield return new WaitForSeconds(300f);
        }
    }
}

[Serializable]
public class GameSaveData
{
    public int money;
    public bool battlePassPurchased;
    public bool isOfferPackPurchased;
    public string playerName;
    public int playerLevel;
    public float experience;
    public int pity;
    public float bgmVolume;
    public float sfxVolume;
    public float masterVolume;
    public bool buildingsDisappear;
    public bool isPhoneActive = true;
    public Character currentCharacter;
    public List<ItemEntry> itemList = new List<ItemEntry>();
    public List<InventoryEntry> inventory = new List<InventoryEntry>();
    public List<CharactersInventoryEntry> charactersInventory = new List<CharactersInventoryEntry>();
}

[Serializable]
public class ItemEntry
{
   public string key;
   public Item value;
}

[Serializable]
public class InventoryEntry
{
    public string key;
    public Inventory value;
}

[Serializable]
public class CharactersInventoryEntry
{
    public string key;
    public Character value;
}