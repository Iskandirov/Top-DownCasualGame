using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ASyncLoader : MonoBehaviour
{
    [Header("Screens To Swap")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] List<GameObject> objectsToHide;
    [Header("Slider and loader")]
    [SerializeField] Slider loadingSlider;
    [SerializeField] Image loader;
    public void LoadLevelBtn(int levelToLoad)
    {
        if(objectsToHide != null)
        {
            foreach (var obj in objectsToHide)
            {
                obj.SetActive(false);
            }
        }
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelASync(levelToLoad));
    }
    public void LoadLevelBtn(string levelToLoad)
    {
        if(objectsToHide != null)
        {
            foreach (var obj in objectsToHide)
            {
                obj.SetActive(false);
            }
        }
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelASync(levelToLoad));
    }
    IEnumerator LoadLevelASync(int levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;

            loader.transform.Rotate(Vector3.forward * Time.deltaTime * 10);

            yield return null;
        }
    }
    IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;

            loader.transform.Rotate(Vector3.forward * Time.deltaTime * 10);

            yield return new WaitForSeconds(3f);
        }
    }
}
