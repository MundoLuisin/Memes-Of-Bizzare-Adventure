using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private Sprite desktopImage;
    [SerializeField] private Sprite tiktakIntroImage;
    [SerializeField] private Sprite callImage;
    [SerializeField] private GameObject tiktakUI;
    [SerializeField] private GameObject desktopUI;
    [SerializeField] private GameObject reproductorImg;
    [SerializeField] private GameObject reproductorVideo;
    [HideInInspector] private AudioSource audioSource;
    [SerializeField] private AudioClip callRingtone;
    [HideInInspector] public Animator anim;
    bool Appear = false;
    bool AppearInitialize = false;
    bool installCall = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        StartCoroutine(RandomCall());
    }

    void Update()
    {
        Time();

        if(Input.GetKeyDown(KeyCode.P))
        {
            Appear = !Appear;
            anim.SetBool("Appear", Appear);

            if(!AppearInitialize)
            {
                anim.SetTrigger("Initialize");
                AppearInitialize = true;
            }
        }
    }

    void Time()
    {
        DateTime dateTime = DateTime.Now;
        string time = dateTime.ToString("h:mm tt");
        timeTxt.text = time;
    }

    public void OpenTikTak()
    {
        StartCoroutine(TikTak());
    }

    public void OpenInstallCall()
    {
        installCall = true;
    }

    public void ExitApp()
    {
        desktopUI.SetActive(true);
        tiktakUI.SetActive(false);
        reproductorImg.SetActive(true);
        reproductorImg.GetComponent<Image>().sprite = desktopImage;
        reproductorVideo.SetActive(false);
    }

    IEnumerator TikTak()
    {
        desktopUI.SetActive(false);
        reproductorImg.GetComponent<Image>().sprite = tiktakIntroImage;
        yield return new WaitForSeconds(1f);
        tiktakUI.SetActive(true);
        reproductorVideo.SetActive(true);
        reproductorImg.SetActive(false);
    }

    

   IEnumerator RandomCall()
{
    while (true)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 20f)); 

        int randomNumber = UnityEngine.Random.Range(1, 26);

        if (randomNumber == 1 || installCall)
        {
            if (!AppearInitialize)
            {
                anim.SetTrigger("Initialize");
                AppearInitialize = true;
            }

            if (!Appear) Appear = true;

            installCall = false;
            anim.SetBool("Appear", Appear);
            desktopUI.SetActive(false);
            tiktakUI.SetActive(false);
            reproductorImg.GetComponent<Image>().sprite = callImage;
            audioSource.PlayOneShot(callRingtone);
        }
    }
}

}
