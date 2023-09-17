using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    Expirience playerExp;
    // Start is called before the first frame update
    void Start()
    {
        playerExp = FindObjectOfType<Expirience>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && playerExp.objHealth.playerHealthPoint < playerExp.objHealth.playerHealthPointMax)
        {
            Debug.Log(2);
            if (playerExp.objHealth.playerHealthPoint + (playerExp.objHealth.playerHealthPointMax / 100) * 10 >= playerExp.objHealth.playerHealthPointMax)
            {
                playerExp.objHealth.playerHealthPoint = playerExp.objHealth.playerHealthPointMax;
                playerExp.objHealth.playerHealthPointImg.fullFillImage.fillAmount = 1;
                Destroy(gameObject);
            }
            else
            {
                playerExp.objHealth.playerHealthPoint += (playerExp.objHealth.playerHealthPointMax / 100) * 10; //Can be baffed by some thing
                playerExp.objHealth.playerHealthPointImg.fullFillImage.fillAmount = playerExp.objHealth.playerHealthPoint / playerExp.objHealth.playerHealthPointMax;
                Destroy(gameObject);
            }
        }
    }
}
