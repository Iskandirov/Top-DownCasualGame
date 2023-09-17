using UnityEngine;
using UnityEngine.UI;

public class Expirience : MonoBehaviour
{
    public Image expiriencepoint;
    public float level;
    public float expNeedToNewLevel;
    public GameObject levelUp;
    public ActivateAbilities activeAbilObj;
    public Health objHealth;
    public Timer time;
    public int isEnemyInZone;
    public int multiply;
    // Start is called before the first frame update
    void Start()
    {
        expNeedToNewLevel = 100;
        objHealth = GetComponent<Health>();
    }
    public void FixedUpdate()
    {
        ShowLevel();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            isEnemyInZone++;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            isEnemyInZone--;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Expirience"))
        {
            collision.GetComponent<EXP>().itWasInPlayerZone = true;
        }
    }
    void ShowLevel()
    {
        if (expiriencepoint.fillAmount >= 1)
        {
            levelUp.SetActive(true);
            levelUp.GetComponent<LevelUpgrade>().RandomAbil();
            Time.timeScale = 0f;

            GetComponent<Move>().otherPanelOpened = true;

            level += 1;
            expiriencepoint.fillAmount = 0;
            expNeedToNewLevel += expNeedToNewLevel * 0.3f;
        }
    }
}
