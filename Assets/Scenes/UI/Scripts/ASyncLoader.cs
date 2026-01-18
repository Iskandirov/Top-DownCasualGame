using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ASyncLoader : MonoBehaviour
{
    public static ASyncLoader Instance;
    private float target;
    [Header("Screens To Swap")]
    [SerializeField] private List<GameObject> objectsToHide;

    [Header("UI Elements")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Image loader;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float progressSmoothSpeed = 1.5f;

    private float displayedProgress = 0f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public async void LoadLevel(string levelToLoad)
    {
        target = 0;
        loadingSlider.value = 0;

        var scene = SceneManager.LoadSceneAsync(levelToLoad);
        scene.allowSceneActivation = false;

        canvas.SetActive(true);
        do
        {
           
            await Task.Delay(100);

            target = scene.progress + .1f;

        } while (scene.progress < 0.9f);
        scene.allowSceneActivation = true;
    }
    void Update()
    {
       // GameManager.Instance.TimeScale(0);
        loader.transform.Rotate(Vector3.forward * -rotationSpeed * Time.deltaTime);
        loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, target, progressSmoothSpeed * Time.deltaTime);
        if (loadingSlider.value >= 1f)
        {
            GameManager.Instance.TimeScale(1);
            canvas.SetActive(false);
        }
    }
}
