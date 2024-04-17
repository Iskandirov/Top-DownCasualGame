using System.IO;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public int sceneCount;
    [SerializeField] ASyncLoader loader;
    public void OnPlay()
    {
        loader.LoadLevelBtn(sceneCount);
    }
    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void CleanFolder()
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            foreach (string file in Directory.GetFiles(Application.persistentDataPath))
            {
                File.Delete(file);
            }
        }
        else
        {
            Debug.Log("Folder not found: " + Application.persistentDataPath);
        }
    } 
    public void CleanPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
