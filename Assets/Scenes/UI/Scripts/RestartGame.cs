using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    PlayerManager player;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        gameManager = GameManager.Instance;
        Time.timeScale = 0f;
    }
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && player.playerHealthPoint > 0 && !gameManager.isPanelOpen)
        {
            gameManager.ClosePanel(gameManager.losePanel);
        }
    }
    public void Restart()
    {
        AudioManager.instance.PlayMusic("GameTheme");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OpenOtherScene(string scenename)
    {
        if (scenename == "Menu")
        {
            AudioManager.instance.PlayMusic("Theme");
        }
        SceneManager.LoadScene(scenename);
    }
}
