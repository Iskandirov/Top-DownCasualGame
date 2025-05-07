using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    PlayerManager player;
    GameManager gameManager;
    [SerializeField] ASyncLoader loader;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        gameManager = GameManager.Instance;
        gameManager.TimeScale(0);
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
        loader.LoadLevelBtn(SceneManager.GetActiveScene().buildIndex);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OpenOtherScene(string scenename)
    {
        if (scenename == "Menu")
        {
            AudioManager.instance.PlayMusic("Theme");
        }
        loader.LoadLevelBtn(scenename);
        //SceneManager.LoadScene(scenename);
    }
}
