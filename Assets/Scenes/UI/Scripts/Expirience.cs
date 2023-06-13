using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expirience : MonoBehaviour
{
    public Image expiriencepoint;
    public float level;
    public GameObject levelUp;
    public ActivateAbilities activeAbilObj;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Expirience"))
        {
            expiriencepoint.fillAmount += collision.GetComponent<EXP>().expBuff / (level * 10);
            
            Destroy(collision.gameObject);
        }

        else if (collision.CompareTag("Health") && GetComponent<Health>().playerHealthPoint < GetComponent<Health>().playerHealthPointMax)
        {
            if (GetComponent<Health>().playerHealthPoint + (GetComponent<Health>().playerHealthPointMax / 100) * 10 >= GetComponent<Health>().playerHealthPointMax)
            {
                GetComponent<Health>().playerHealthPoint = GetComponent<Health>().playerHealthPointMax;
                Destroy(collision.gameObject);
            }
            else
            {
                GetComponent<Health>().playerHealthPoint += (GetComponent<Health>().playerHealthPointMax / 100) * 10; //Can be baffed by some thing
                GetComponent<Health>().playerHealthPointImg.fillAmount = GetComponent<Health>().playerHealthPoint / GetComponent<Health>().playerHealthPointMax;
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
