using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class UIGameManager : MonoBehaviour
{
    private bool openCanva = false;
    [SerializeField] private GameObject canva;
    [SerializeField] private GameObject optionsCanva;
    [SerializeField] private GameObject phone;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D pointerCursor;
    private bool isPointer = false;

    [SerializeField] private Animator transitionAnimator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            openCanva = !openCanva;
            canva.SetActive(openCanva);
        }

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
            if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
                return true;
        }

        return false;
    }

    public void clickSoundPlay() // External method to play click sound
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void OpenSettings()
    {
        openCanva = !openCanva;
        canva.SetActive(openCanva);
        audioSource.PlayOneShot(clickSound);
    }

    public void Resume()
    {
        openCanva = !openCanva;
        canva.SetActive(openCanva);
        audioSource.PlayOneShot(clickSound);
    }

    public void Options()
    {
        optionsCanva.SetActive(true);
        canva.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void ExitGame()
    {
        audioSource.PlayOneShot(clickSound);
        Application.Quit();
    }

    public void ActivatePhone()
    {
       GameData.Instance.isPhoneActive = !GameData.Instance.isPhoneActive;
       phone.SetActive(GameData.Instance.isPhoneActive);
       audioSource.PlayOneShot(clickSound);
    }

    public void BuildingsDisappear()
    {
       GameData.Instance.buildingsDisappear = !GameData.Instance.buildingsDisappear;
       audioSource.PlayOneShot(clickSound);
    }

    public void Apply()
    {
        optionsCanva.SetActive(false);
        canva.SetActive(true);
        audioSource.PlayOneShot(clickSound);
    }

    public void ReturnHome()
    {
        StartCoroutine(ReturnHomeCoroutine());
    }

    IEnumerator ReturnHomeCoroutine()
    {
        audioSource.PlayOneShot(clickSound);
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Home");
    }
}
