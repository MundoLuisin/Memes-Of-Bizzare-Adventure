using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadSceneManager : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text progressText;
    public string sceneToLoad = "MainScene";

    [SerializeField] private Animator transitionAnimator;

    void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (fakeProgress < 1f)
        {
            float target = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            fakeProgress = Mathf.MoveTowards(fakeProgress, target, Time.deltaTime);
        
            if (progressBar != null) progressBar.value = fakeProgress;
            if (progressText != null) progressText.text = (fakeProgress * 100f).ToString("F0") + "%";

            if (asyncLoad.progress >= 0.9f && fakeProgress >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                transitionAnimator.SetTrigger("Start");
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
