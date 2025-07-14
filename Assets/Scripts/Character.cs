using UnityEngine;

[System.Serializable]
public class Character
{
    public string name;
    public int index;
    public int level;
    public int health;
    public int attackPower;
    public int attackTimer;
    public int speed;
    public bool isHumanoid;
    public Vector3 modelScale;
    public Vector3 modelPosition;
    public bool isUnlocked;
    public GameObject characterModel;
    public Avatar avatar;
    public string musicGenre;
    public Sprite characterIcon;

    public void LevelUp()
    {
        level++;
        health += 10;
        attackPower += 5;
    }

    public Character(string Name, int Index, int Level, int Health, int AttackPower, int AttackTimer, int Speed, bool IsHumanoid, Vector3 ModelScale, Vector3 ModelPosition, bool IsUnlocked, GameObject CharacterModel, Avatar Avatar, string MusicGenre, Sprite CharacterIcon)
    {
        name = Name;
        index = Index;
        level = Level;
        health = Health;
        attackPower = AttackPower;
        attackTimer = AttackTimer;
        speed = Speed;
        isHumanoid = IsHumanoid;
        modelScale = ModelScale;
        modelPosition = ModelPosition;
        isUnlocked = IsUnlocked;
        characterModel = CharacterModel;
        avatar = Avatar;
        musicGenre = MusicGenre;
        characterIcon = CharacterIcon;
    }
}
