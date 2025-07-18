using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using System.IO;
using UnityEngine.Audio;
using SimpleFileBrowser;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyTxt;

    [SerializeField] private GameObject homeCanvas;
    [SerializeField] private GameObject charactersCanvas;
    [SerializeField] private GameObject gachaCanvas;
    [SerializeField] private GameObject battlePassCanvas;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private GameObject optionsCanvas;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip purchaseSound;
    [SerializeField] private AudioClip noMoneySound;
    private bool isPointer = false;

    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D pointerCursor;

    #region Home Zone
    [SerializeField] private TMP_Text playerLevelTxt;

    [SerializeField] private RawImage profileImageHome;
    [SerializeField] private TMP_Text playerHomeNameText;

    [SerializeField] private GameObject[] characterPrefab;
    [SerializeField] private Avatar[] characterAvatarMask;
    private Character[] characters;

    [SerializeField] private GameObject characterHomePrefab;
    [SerializeField] private GameObject characterAnimatorObject;
    private GameObject characterPrefabInstance;

    [SerializeField] private Animator transitionAnimator;
    #endregion

    #region Character Selection Zone
    [SerializeField] private Sprite[] charactersImages;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterNameTxt;
    [SerializeField] private TMP_Text characterLevelTxt;
    [SerializeField] private TMP_Text characterHealthTxt;
    [SerializeField] private TMP_Text characterAttackPowerTxt;
    [SerializeField] private TMP_Text characterAttackTimerTxt;
    [SerializeField] private TMP_Text characterSpeedTxt;
    [SerializeField] private TMP_Text characterMusicGenreTxt;
    #endregion

    #region Shop Zone
    [SerializeField] private GameObject shopOffersPanel;
    [SerializeField] private GameObject shopForYouPanel;
    [SerializeField] private GameObject shopSkillsPanel;
    [SerializeField] private GameObject shopItemsPanel;
    [SerializeField] private GameObject shopCharactersPanel;
    [SerializeField] private GameObject shopBattlePassPanel;
    [SerializeField] private GameObject shopCoinsPanel;

    [SerializeField] private Sprite purchasedOfferImage;
    [SerializeField] private GameObject OfferImage;
    #endregion

    #region Gacha Zone
    public InventoryManager inventoryManager;
    private string promotionalCharacter;
    [SerializeField] private GameObject rollPanel;
    [SerializeField] private GameObject[] slots;
    [SerializeField] private Sprite[] slotSpriteBackground;
    [SerializeField] private Image[] slotImageObj;
    [SerializeField] private Image[] slotImageBackground;
    [SerializeField] private TMP_Text[] slotText;
    #endregion

    #region Options Zone
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject profileMenu;
    [SerializeField] private GameObject keybindsMenu;
    [SerializeField] private GameObject accountMenu;
    [SerializeField] private GameObject infoMenu;

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerUserText;
    [SerializeField] private TMP_Text playerLevelOptionsTxt;
    [SerializeField] private TMP_InputField changeNameInputField;
    [SerializeField] private GameObject changeNameUIgroup;
    [SerializeField] private RawImage profileImage;
    [SerializeField] private Texture2D defaultProfileImage;
    private string savePath;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;
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
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    void Start()
    {
        characters = new Character[]
        {
            // string Name, int Index, int Level, int Health, int AttackPower, int AttackTimer, int Speed, bool IsHumanoid, Vector3 ModelScale, Vector3 ModelPosition, bool IsUnlocked, GameObject CharacterModel, Avatar Avatar, string MusicGenre, Texture2D CharacterIcon
            new Character("Sr. Pollo", 1, 1, 100, 25, 2, 15, true, new Vector3(1, 1, 1), new Vector3(0, 0, 0), true, characterPrefab[0], characterAvatarMask[0], "phonk", charactersImages[0]),
            new Character("Tung tung tung Sahur", 2, 1, 75, 30, 3, 15, true, new Vector3(1, 1, 1), new Vector3(0, 0, 0), true, characterPrefab[1], characterAvatarMask[1], "hardstyle", charactersImages[1]),
            new Character("EA 68", 3, 1, 125, 35, 8, 30, false, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, -1), true, characterPrefab[2], /*null*/ characterAvatarMask[2], "eurobeat", charactersImages[2]),
            new Character("Quandle", 4, 1, 100, 20, 1, 20, true, new Vector3(1, 1, 1), new Vector3(0, 0, 0), true, characterPrefab[3], characterAvatarMask[3], "trap", charactersImages[3]),
            new Character("Stonks Man", 5, 1, 150, 25, 2, 20, true, new Vector3(1, 1, 1), new Vector3(0, 0, 0), true, characterPrefab[4], characterAvatarMask[4], "dubstep", charactersImages[4]) // Stonks Man hasn't the mesh vertex reduction in Blender
        };

        promotionalCharacter = "Tung tung tung Sahur";

        int safeIndex = Mathf.Clamp(GameData.Instance.currentCharacter != null ? GameData.Instance.currentCharacter.index - 1 : 0, 0, characters.Length - 1);
        GameData.Instance.currentCharacter = characters[safeIndex];

        if(GameData.Instance.isOfferPackPurchased) OfferImage.GetComponent<Image>().sprite = purchasedOfferImage;

        savePath = Application.persistentDataPath + "/profile.png";
        if (File.Exists(savePath)) LoadSavedImage();
        else 
        {
            profileImage.texture = defaultProfileImage;
            profileImageHome.texture = defaultProfileImage;
        }

        if(!string.IsNullOrEmpty(GameData.Instance.playerName)) 
        {
            playerHomeNameText.text = GameData.Instance.playerName;
            playerNameText.text = GameData.Instance.playerName;
            playerUserText.text = "ACCOUNT: " + GameData.Instance.playerName;
        }
        else 
        {
            string randomUserName = randomNames[Random.Range(0, randomNames.Length)];
            playerHomeNameText.text = randomUserName;
            playerNameText.text = randomUserName;
            GameData.Instance.playerName = randomUserName;
            playerUserText.text = "ACCOUNT: " + randomUserName;
        }

        Lvl();
        InstanceCharacterPrefab();
        CharacterInfo();
        EnterHome();
        ProfileMenu();
        ShopSelectOffers();
        InitializeVolumeSettings();
        InitializeSliders();
    }

    void Update()
    {
        moneyTxt.text = GameData.Instance.money.ToString();

        bool isOnButton = IsPointerOverUIButton();

        if (isOnButton != isPointer)
        {
            isPointer = isOnButton;
            Cursor.SetCursor(isPointer ? pointerCursor : normalCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    bool IsPointerOverUIButton()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true; 
            }
        }

        return false; 
    }  

    #region HOME
    void Lvl()
    {
        int playerLevel = GameData.Instance.playerLevel;
        if (playerLevel >= 0 && playerLevel <= 19)
        {
            playerLevelTxt.text = "Lvl: " + playerLevel.ToString() + " Crook";
        }
        else if(playerLevel >= 20 && playerLevel <= 34)
        {
            playerLevelTxt.text = "Lvl: " + playerLevel.ToString() + " Underboss";
        }
        else if(playerLevel >= 35 && playerLevel <= 49)
        {
            playerLevelTxt.text = "Lvl: " + playerLevel.ToString() + " Boss";
        }
        else if(playerLevel >= 50 && playerLevel <= 99)
        {
            playerLevelTxt.text = "Lvl: " + playerLevel.ToString() + " Mafia Boss";
        }
        else
        {
            playerLevelTxt.text = "Lvl: " + "MAX";
        }

        playerLevelOptionsTxt.text = playerLevelTxt.text;
    }

    public void Play()
    {
        StartCoroutine(PlayCoroutine());
    }
    
    IEnumerator PlayCoroutine()
    {
        audioSource.PlayOneShot(clickSound);
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("LoadScene");
    }
    #endregion 

    #region CHARACTER
    void InstanceCharacterPrefab()
    {
        if (characterPrefabInstance != null)
        {
            Destroy(characterPrefabInstance);
        }

        characterPrefabInstance = Instantiate(GameData.Instance.currentCharacter.characterModel, characterHomePrefab.transform);
        characterPrefabInstance.transform.localScale = GameData.Instance.currentCharacter.modelScale;
        characterPrefabInstance.transform.localPosition = GameData.Instance.currentCharacter.modelPosition;

        if (GameData.Instance.currentCharacter.isHumanoid)
        {
            Animator anim = characterAnimatorObject.GetComponent<Animator>();
            anim.avatar = GameData.Instance.currentCharacter.avatar;
        }
    }

    void CharacterInfo()
    {
        characterImage.sprite = GameData.Instance.currentCharacter.characterIcon; 
        characterNameTxt.text = GameData.Instance.currentCharacter.name;
        characterLevelTxt.text = GameData.Instance.currentCharacter.level.ToString();
        characterHealthTxt.text = GameData.Instance.currentCharacter.health.ToString();
        characterAttackPowerTxt.text = GameData.Instance.currentCharacter.attackPower.ToString();
        characterAttackTimerTxt.text = GameData.Instance.currentCharacter.attackTimer.ToString() + "s";
        characterSpeedTxt.text =  GameData.Instance.currentCharacter.speed.ToString() + "km/h";
        characterMusicGenreTxt.text = GameData.Instance.currentCharacter.musicGenre;
    }

    public void SelectDonPollo()
    {
        GameData.Instance.currentCharacter = characters[0];
        InstanceCharacterPrefab();
        CharacterInfo();
        audioSource.PlayOneShot(clickSound);
    }

    public void SelectTungTungTungSahur()
    {
        GameData.Instance.currentCharacter = characters[1];
        InstanceCharacterPrefab();
        CharacterInfo();
        audioSource.PlayOneShot(clickSound);
    }

    public void SelectAE86()
    {
        GameData.Instance.currentCharacter = characters[2];
        InstanceCharacterPrefab();
        CharacterInfo();
        audioSource.PlayOneShot(clickSound);
    }

    public void SelectQuandle()
    {
        GameData.Instance.currentCharacter = characters[3];
        InstanceCharacterPrefab();
        CharacterInfo();
        audioSource.PlayOneShot(clickSound);
    }

    public void SelectStonksMan()
    {
        GameData.Instance.currentCharacter = characters[4];
        InstanceCharacterPrefab();
        CharacterInfo();
        audioSource.PlayOneShot(clickSound);
    }
    #endregion

    #region TOP BAR
    public void EnterHome()
    {
       homeCanvas.SetActive(true);
       charactersCanvas.SetActive(false);
       gachaCanvas.SetActive(false);
       battlePassCanvas.SetActive(false);
       shopCanvas.SetActive(false);
       optionsCanvas.SetActive(false);      
       audioSource.PlayOneShot(clickSound);
    }

    public void EnterCharacters()
    {
        homeCanvas.SetActive(false);
        charactersCanvas.SetActive(true);
        gachaCanvas.SetActive(false);
        battlePassCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void EnterGacha()
    {
        homeCanvas.SetActive(false);
        charactersCanvas.SetActive(false);
        gachaCanvas.SetActive(true);
        battlePassCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void EnterBattlePass()
    {
        homeCanvas.SetActive(false);
        charactersCanvas.SetActive(false);
        gachaCanvas.SetActive(false);
        battlePassCanvas.SetActive(true);
        shopCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void EnterShop()
    {
        homeCanvas.SetActive(false);
        charactersCanvas.SetActive(false);
        gachaCanvas.SetActive(false);
        battlePassCanvas.SetActive(false);
        shopCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void EnterOptions()
    {
        homeCanvas.SetActive(false);
        charactersCanvas.SetActive(false);
        gachaCanvas.SetActive(false);
        battlePassCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
        audioSource.PlayOneShot(clickSound);
    }

    public void ShopSelectOffers()
    {
        shopOffersPanel.SetActive(true);
        shopForYouPanel.SetActive(false);
        shopSkillsPanel.SetActive(false);
        shopItemsPanel.SetActive(false);
        shopCharactersPanel.SetActive(false);
        shopBattlePassPanel.SetActive(false);
        shopCoinsPanel.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void ShopSelectForYou()
    {
        shopOffersPanel.SetActive(false);
        shopForYouPanel.SetActive(true);
        shopSkillsPanel.SetActive(false);
        shopItemsPanel.SetActive(false);
        shopCharactersPanel.SetActive(false);
        shopBattlePassPanel.SetActive(false);
        shopCoinsPanel.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void ShopSelectSkills()
    {
        shopOffersPanel.SetActive(false);
        shopForYouPanel.SetActive(false);
        shopSkillsPanel.SetActive(true);
        shopItemsPanel.SetActive(false);
        shopCharactersPanel.SetActive(false);
        shopBattlePassPanel.SetActive(false);
        shopCoinsPanel.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void ShopSelectItems()
    {
        shopOffersPanel.SetActive(false);
        shopForYouPanel.SetActive(false);
        shopSkillsPanel.SetActive(false);
        shopItemsPanel.SetActive(true);
        shopCharactersPanel.SetActive(false);
        shopBattlePassPanel.SetActive(false);
        shopCoinsPanel.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void ShopSelectCharacters()
    {
        shopOffersPanel.SetActive(false);
        shopForYouPanel.SetActive(false);
        shopSkillsPanel.SetActive(false);
        shopItemsPanel.SetActive(false);
        shopCharactersPanel.SetActive(true);
        shopBattlePassPanel.SetActive(false);
        shopCoinsPanel.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void ShopSelectBattlePass()
    {
        shopOffersPanel.SetActive(false);
        shopForYouPanel.SetActive(false);
        shopSkillsPanel.SetActive(false);
        shopItemsPanel.SetActive(false);
        shopCharactersPanel.SetActive(false);
        shopBattlePassPanel.SetActive(true);
        shopCoinsPanel.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void ShopSelectCoins()
    {
        shopOffersPanel.SetActive(false);
        shopForYouPanel.SetActive(false);
        shopSkillsPanel.SetActive(false);
        shopItemsPanel.SetActive(false);
        shopCharactersPanel.SetActive(false);
        shopBattlePassPanel.SetActive(false);
        shopCoinsPanel.SetActive(true);
        audioSource.PlayOneShot(clickSound);
    }
    #endregion

    #region SHOP
    // Offer Pack no real money system, just for fun UWU
    // OFFER PACK //
    public void BuyOfferPack()
    {
        if (!GameData.Instance.isOfferPackPurchased)
        {
            GameData.Instance.isOfferPackPurchased = true;
            OfferImage.GetComponent<Image>().sprite = purchasedOfferImage;
            GameData.Instance.money += 1200;
            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    // FOR YOU //
    public void SelectForYouBanner1()
    {
        audioSource.PlayOneShot(clickSound);
        ShopSelectBattlePass();
    }

    public void SelectForYouBanner2()
    {
        audioSource.PlayOneShot(clickSound);
        ShopSelectCharacters();
    }

    public void SelectForYouBanner3()
    {
        audioSource.PlayOneShot(clickSound);
        EnterGacha();
    }

    // This needs to be implemented with skills system
    // SKILLS //
    public void BuySkillPack1()
    {
        if (GameData.Instance.money >= 100)
        {
            GameData.Instance.money -= 100;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuySkillPack2()
    {
        if (GameData.Instance.money >= 100)
        {
            GameData.Instance.money -= 100;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuySkillPack3()
    {
        if (GameData.Instance.money >= 100)
        {
            GameData.Instance.money -= 100;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuySkillPack4()
    {
        if (GameData.Instance.money >= 100)
        {
            GameData.Instance.money -= 100;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuySkillPack5()
    {
        if (GameData.Instance.money >= 100)
        {
            GameData.Instance.money -= 100;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuySkillPack6()
    {
        if (GameData.Instance.money >= 100)
        {
            GameData.Instance.money -= 100;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuySkillPack7()
    {
        if (GameData.Instance.money >= 100)
        {
            GameData.Instance.money -= 100;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    // ITEMS //
    // This is unimplemented and needs to be implemented with ascension items and XP bottles
    public void BuyItemsPack1()
    {
        if (GameData.Instance.money >= 1)
        {
            GameData.Instance.money -= 1;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuyItemsPack2()
    {
        if (GameData.Instance.money >= 5)
        {
            GameData.Instance.money -= 5;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    // This needs to be implemented with ascension items and XP bottles
    // CHARACTERS //
    public void BuyCharacterPack1()
    {
        if (GameData.Instance.money >= 50)
        {
            GameData.Instance.money -= 50;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuyCharacterPack2()
    {
        if (GameData.Instance.money >= 50)
        {
            GameData.Instance.money -= 50;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuyCharacterPack3()
    {
        if (GameData.Instance.money >= 75)
        {
            GameData.Instance.money -= 75;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuyCharacterPack4()
    {
        if (GameData.Instance.money >= 75)
        {
            GameData.Instance.money -= 75;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuyCharacterPack5()
    {
        if (GameData.Instance.money >= 75)
        {
            GameData.Instance.money -= 75;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    public void BuyCharacterPack6()
    {
        if (GameData.Instance.money >= 25)
        {
            GameData.Instance.money -= 25;

            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    // Battle Pass system, not implemented yet
    // BATTLE PASS //
    public void BuyBattlePass()
    {
        if (GameData.Instance.money >= 500 && !GameData.Instance.battlePassPurchased)
        {
            GameData.Instance.money -= 500;
            GameData.Instance.battlePassPurchased = true;
            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            audioSource.PlayOneShot(noMoneySound);
        }
    }

    // Coins Packs no real money system, just for fun UWU
    // COINS //
    public void BuyPack1Coins()
    {
        GameData.Instance.money += 50;
        audioSource.PlayOneShot(purchaseSound);
    }

    public void BuyPack2Coins()
    {
        GameData.Instance.money += 200;
        audioSource.PlayOneShot(purchaseSound);
    }

    public void BuyPack3Coins()
    {
        GameData.Instance.money += 500;
        audioSource.PlayOneShot(purchaseSound);
    }

    public void BuyPack4Coins()
    {
        GameData.Instance.money += 1750;
        audioSource.PlayOneShot(purchaseSound);
    }

    public void BuyPack5Coins()
    {
        GameData.Instance.money += 4000;
        audioSource.PlayOneShot(purchaseSound);
    }

    public void BuyPack6Coins()
    {
        GameData.Instance.money += 9500;
        audioSource.PlayOneShot(purchaseSound);
    }
    #endregion

    #region GACHA
    (Sprite, string, int) RollGacha()
    {
        float roll = Random.Range(0, 100 - GameData.Instance.pity);

        if(roll < 1)
        {
            //✩✩✩ 2
            GameData.Instance.pity = 0;
            if(!inventoryManager.charactersInventory.ContainsKey(promotionalCharacter))
            {
                var newChar = characters.FirstOrDefault(i => i.name == promotionalCharacter);
                inventoryManager.charactersInventory.Add(promotionalCharacter, newChar);

                return (newChar.characterIcon, promotionalCharacter, 2);
            }
            else
            {
                string promotionalCharacterGem = promotionalCharacter + " Gem";

                if(inventoryManager.inventory.ContainsKey(promotionalCharacterGem))
                {
                    inventoryManager.inventory[promotionalCharacterGem].amount++;
                }
                else
                {
                    Item item = inventoryManager.characterGems[promotionalCharacterGem];
                    inventoryManager.inventory.Add(promotionalCharacterGem, new Inventory(promotionalCharacterGem, 1, item));
                }

                return (inventoryManager.characterGems[promotionalCharacterGem].objectSprite, promotionalCharacterGem, 2);
            }
        }
        else
        {
            float rareRoll = Random.Range(0f, 100f);
            if(rareRoll < 10f)
            {
                //✩✩ 1
                GameData.Instance.pity++;

                var values = new List<Item>(inventoryManager.skills.Values);
                Skill randomSkill = (Skill)values[UnityEngine.Random.Range(0, values.Count)];

                bool allAcquired = inventoryManager.skills.Values.OfType<Skill>().Where(skill => !skill.isCharacterExclusive).All(skill => skill.isAcquired);
                if (allAcquired)
                {
                    //✩ 0
                    var valuesUpgrade = new List<Item>(inventoryManager.upgradeItems.Values);
                    UpgradeItem fallbackItem = (UpgradeItem)valuesUpgrade[UnityEngine.Random.Range(0, valuesUpgrade.Count)];

                    string itemName = fallbackItem.name;
                    if (inventoryManager.inventory.ContainsKey(itemName))
                    {
                        inventoryManager.inventory[itemName].amount++;
                    }
                    else
                    {
                        inventoryManager.inventory.Add(itemName, new Inventory(itemName, 1, fallbackItem));
                    }

                    return (fallbackItem.objectSprite, fallbackItem.name, 0);
                }
                else
                {
                    while (randomSkill.isAcquired || randomSkill.isCharacterExclusive)
                    {
                        randomSkill = (Skill)values[UnityEngine.Random.Range(0, values.Count)];
                    }

                    randomSkill.isAcquired = true;
                    inventoryManager.inventory.Add(randomSkill.name, new Inventory(randomSkill.name, 1, randomSkill));

                    return (randomSkill.objectSprite, randomSkill.name, 1);
                }
            }
            else
            {
                //✩ 0
                GameData.Instance.pity++;

                var values = new List<Item>(inventoryManager.upgradeItems.Values);
                UpgradeItem randomUpgradeItem = (UpgradeItem)values[UnityEngine.Random.Range(0, values.Count)];

                string itemName = randomUpgradeItem.name;
                if (inventoryManager.inventory.ContainsKey(itemName))
                {
                    inventoryManager.inventory[itemName].amount++;
                }
                else
                {
                    inventoryManager.inventory.Add(itemName, new Inventory(itemName, 1, randomUpgradeItem));
                }

                return (randomUpgradeItem.objectSprite, randomUpgradeItem.name, 0);
            }
        }
    }

    public void Roll1()
    {
        var result = RollGacha();

       slotImageObj[0].sprite = result.Item1;
       slotText[0].text = result.Item2;
       slotImageBackground[0].sprite = slotSpriteBackground[result.Item3]; 

       StartCoroutine(RollAnimation(true));
    }

    public void Roll10()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var result = RollGacha();
            slotImageObj[i].sprite = result.Item1;
            slotText[i].text = result.Item2;
            slotImageBackground[i].sprite = slotSpriteBackground[result.Item3]; 
        }

        StartCoroutine(RollAnimation(false));
    }

    IEnumerator RollAnimation(bool isSingleRoll)
    {
        rollPanel.SetActive(true);

        if(!isSingleRoll)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetActive(true);
                yield return new WaitForSeconds(0.5f);
            }
        }
        else if(isSingleRoll)
        {
            slots[0].SetActive(true);
            yield return new WaitForSeconds(1f);
        }
    }

    public void ExitRollPanel()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].SetActive(false);
        }

        rollPanel.SetActive(false);
    }
    #endregion

    #region OPTIONS
    void InitializeVolumeSettings()
    {

        if (GameData.Instance.bgmVolume == 0)
        {
            GameData.Instance.bgmVolume = 1f; 
        }
        if (GameData.Instance.sfxVolume == 0)
        {
            GameData.Instance.sfxVolume = 1f; 
        }
        if (GameData.Instance.masterVolume == 0)
        {
            GameData.Instance.masterVolume = 1f;
        }

        SetBGMVolume(GameData.Instance.bgmVolume);
        SetSFXVolume(GameData.Instance.sfxVolume);
        SetMasterVolume(GameData.Instance.masterVolume);
    }

    public void Apply()
    {
        GameData.Instance.bgmVolume = bgmSlider.value;
        GameData.Instance.sfxVolume = sfxSlider.value;
        GameData.Instance.masterVolume = masterSlider.value;
    }

    void InitializeSliders()
    {
        float bgmVolume;
        audioMixer.GetFloat("BGMVolume", out bgmVolume);
        bgmSlider.value = Mathf.Pow(10, bgmVolume / 20);

        float sfxVolume;
        audioMixer.GetFloat("SFXVolume", out sfxVolume);
        sfxSlider.value = Mathf.Pow(10, sfxVolume / 20);

        float masterVolume;
        audioMixer.GetFloat("MasterVolume", out masterVolume);
        masterSlider.value = Mathf.Pow(10, masterVolume / 20);
    }

    void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }

    void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void OnClick_SelectImage()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public void ActivatePhone()
    {
       GameData.Instance.isPhoneActive = !GameData.Instance.isPhoneActive;
    }

    public void BuildingsDisappear()
    {
       GameData.Instance.buildingsDisappear = !GameData.Instance.buildingsDisappear;
    }

    private IEnumerator ShowLoadDialogCoroutine()
    {
        string[] filters = { "Image Files", ".png,.jpg,.jpeg" };

       yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, "Select your profile IMG", "Select");

        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result[0];
            byte[] imageBytes = File.ReadAllBytes(path);
            File.WriteAllBytes(savePath, imageBytes); 
            LoadImageFromBytes(imageBytes);
        }
    }

    private void LoadSavedImage()
    {
        byte[] bytes = File.ReadAllBytes(savePath);
        LoadImageFromBytes(bytes);
    }

    private void LoadImageFromBytes(byte[] bytes)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        profileImage.texture = tex;
        profileImageHome.texture = tex;
    }

    public void EnterChangeName()
    {
        changeNameUIgroup.SetActive(true);
        audioSource.PlayOneShot(clickSound);
    }

    public void ApplyChangeNameAndExit()
    {
        if(!string.IsNullOrEmpty(changeNameInputField.text))
        {
            GameData.Instance.playerName = changeNameInputField.text;
            playerHomeNameText.text = changeNameInputField.text;
            playerNameText.text = changeNameInputField.text;
            playerUserText.text = "ACCOUNT: " + changeNameInputField.text;
            changeNameUIgroup.SetActive(false);
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void ProfileMenu()
    {
        settingsMenu.SetActive(false);
        profileMenu.SetActive(true);
        keybindsMenu.SetActive(false);
        accountMenu.SetActive(false);
        infoMenu.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void SettingsMenu()
    {
        settingsMenu.SetActive(true);
        profileMenu.SetActive(false);
        keybindsMenu.SetActive(false);
        accountMenu.SetActive(false);
        infoMenu.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void KeybindsMenu()
    {
        settingsMenu.SetActive(false);
        profileMenu.SetActive(false);
        keybindsMenu.SetActive(true);
        accountMenu.SetActive(false);
        infoMenu.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void AccountMenu()
    {
        settingsMenu.SetActive(false);
        profileMenu.SetActive(false);
        keybindsMenu.SetActive(false);
        accountMenu.SetActive(true);
        infoMenu.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void InfoMenu()
    {
        settingsMenu.SetActive(false);
        profileMenu.SetActive(false);
        keybindsMenu.SetActive(false);
        accountMenu.SetActive(false);
        infoMenu.SetActive(true);
        audioSource.PlayOneShot(clickSound);
    }

    public void ManualSaveData()
    {
        DataManager.Instance.SaveData();
    }

    public void Wipedata()
    {
        DataManager.Instance.WipeAllData();
    }
    #endregion 

    #region BATTLE PASS

    // REWARDS
    public void RewardMoney_50()
    {
        GameData.Instance.money += 50;
    }

    public void RewardXp_500()
    {
       RewardItem("XP Bottle", 500);
    }

    public void RewardXp_50()
    {
        RewardItem("XP Bottle", 50);
    }

    public void RewardStardust_75()
    {
        RewardItem("StarDust Bottle", 75);
    }

    public void RewardStardust_5()
    {
        RewardItem("StarDust Bottle", 5);
    }

    void RewardItem(string itemName, int amount)
    {
        Item fallbackItem = null;
        if (inventoryManager.upgradeItems.ContainsKey(itemName)) 
        {
            fallbackItem = inventoryManager.upgradeItems[itemName];
        }

        if (inventoryManager.inventory.ContainsKey(itemName))
        {
            inventoryManager.inventory[itemName].amount += amount;
        }
        else
        {
            inventoryManager.inventory.Add(itemName, new Inventory(itemName, amount, fallbackItem));
        }
    }

    public void RewardSigmaSkill()
    {
        inventoryManager.inventory.Add("Sigma", new Inventory("Sigma", 1, inventoryManager.upgradeItems["Sigma"]));
    }
    #endregion
}

