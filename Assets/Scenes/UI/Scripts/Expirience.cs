using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Expirience : MonoBehaviour
{
    public Image expiriencepoint;
    public float level;
    public float expNeedToNewLevel;
    public GameObject levelUp;
    public ActivateAbilities activeAbilObj;
    Health objHealth;
    public Timer time;
    
    // Start is called before the first frame update
    void Start()
    {
        expNeedToNewLevel = 100;
        objHealth = GetComponent<Health>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Health") && objHealth.playerHealthPoint < objHealth.playerHealthPointMax)
        {
            if (objHealth.playerHealthPoint + (objHealth.playerHealthPointMax / 100) * 10 >= objHealth.playerHealthPointMax)
            {
                objHealth.playerHealthPoint = objHealth.playerHealthPointMax;
                Destroy(collision.gameObject);
            }
            else
            {
                objHealth.playerHealthPoint += (objHealth.playerHealthPointMax / 100) * 10; //Can be baffed by some thing
                objHealth.playerHealthPointImg.fillAmount = objHealth.playerHealthPoint / objHealth.playerHealthPointMax;
                Destroy(collision.gameObject);
            }
        }
        ShowLevel();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Expirience"))
        {
            collision.GetComponent<EXP>().itWasInPlayerZone = true;
        }
    }
    public void ShowLevel()
    {
        if (expiriencepoint.fillAmount >= 1)
        {
            levelUp.SetActive(true);
            levelUp.GetComponent<LevelUpgrade>().RandomAbil();
            Time.timeScale = 0f;

            level += 1;
            expiriencepoint.fillAmount = 0;
            expNeedToNewLevel += expNeedToNewLevel * 0.6f;
        }
    }
}
