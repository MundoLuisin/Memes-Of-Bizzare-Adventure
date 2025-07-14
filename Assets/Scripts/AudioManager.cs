using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

[System.Serializable]
public class MusicalClipsGenre
{
    public string nameGenre;
   public List<AudioClip> clips;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;

    [SerializeField] private List<MusicalClipsGenre> audioClipsJukebox;
    [SerializeField] private AudioSource jukeboxAudioSource;
    private Dictionary<string, List<AudioClip>> genreClipDict;
    private AudioClip currentClip;
    string currentGenre;

    void Awake()
    {
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    void Start()
    {
        Initializejukebox();
        InitializeVolumeSettings();
        InitializeSliders();
    }

    
    void Update()
    {
        if(!jukeboxAudioSource.isPlaying)
        {
            PlayNextTrack();
        }   
    }

    void Initializejukebox()
    {
        currentGenre = GameData.Instance.currentCharacter.musicGenre;
        genreClipDict = new Dictionary<string, List<AudioClip>>();
        foreach (var genre in audioClipsJukebox)
        {
            genreClipDict[genre.nameGenre] = genre.clips;
        }

        PlayNextTrack();
    }

    void PlayNextTrack()
    {
        if (genreClipDict.ContainsKey(currentGenre) && genreClipDict[currentGenre].Count > 0)
        {
            List<AudioClip> clips = genreClipDict[currentGenre];
            currentClip = clips[Random.Range(0, clips.Count)];
            jukeboxAudioSource.clip = currentClip;
            jukeboxAudioSource.Play();
        }
    }

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
}
