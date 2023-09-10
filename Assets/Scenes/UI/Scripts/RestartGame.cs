using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    Move player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Move>();
        Time.timeScale = 0f;
    }
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && player.GetComponent<Health>().playerHealthPoint > 0)
        {
            player.otherPanelOpened = false;
            player.escPanelIsShowed = false;
            Destroy(gameObject);

            OnDestroy();
        }
    }
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OpenOtherScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }
}
