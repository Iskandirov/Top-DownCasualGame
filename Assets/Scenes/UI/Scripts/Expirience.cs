using UnityEngine;
using UnityEngine.UI;

public class Expirience : MonoBehaviour
{
    public Image expiriencepoint;
    public float level;
    public GameObject levelUp;
    public ActivateAbilities activeAbilObj;
    Health objHealth;
    float timePassed; // Виміряйте час, що пройшов з певного початкового моменту
    public Timer time;
    public float expMultiplier; // Множник, зворотно пропорційний до пройденого часу
    // Start is called before the first frame update
    void Start()
    {
        
        objHealth = GetComponent<Health>();
    }
    private void FixedUpdate()
    {
        timePassed = time.time - 0f;
        expMultiplier = Mathf.Exp(-timePassed / (10 * 20));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Expirience"))
        {
            expiriencepoint.fillAmount += collision.GetComponent<EXP>().expBuff / (level * 10) * expMultiplier;
            
            Destroy(collision.gameObject);
        }

        else if (collision.CompareTag("Health") && objHealth.playerHealthPoint < objHealth.playerHealthPointMax)
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
    public void ShowLevel()
    {
        if (expiriencepoint.fillAmount >= 1)
        {
            levelUp.SetActive(true);
            levelUp.GetComponent<LevelUpgrade>().RandomAbil();
            Time.timeScale = 0f;

            level += 1;
            expiriencepoint.fillAmount = 0;
        }
    }
}
