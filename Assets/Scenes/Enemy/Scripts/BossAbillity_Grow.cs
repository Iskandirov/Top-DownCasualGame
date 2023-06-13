using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAbillity_Grow : MonoBehaviour
{
    public Vector3 speedGrow;
    GameObject line;
    public float changeColorSpeed;
    public float timeToExplode;
    public float timeToExplodeMax;
    GameObject playerHealth;

    public Color32 startColor;
    public Color32 endColor;
    // Start is called before the first frame update
    void Start()
    {
        line = GameObject.Find("Player/UI/Parent_Line/Time_Bobs");
        line.SetActive(true);
        line.GetComponent<Image>().fillAmount = 1;
        playerHealth = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.localScale += speedGrow;

        //Лінія часу для вбивства боса
        line.GetComponent<Image>().fillAmount = timeToExplode / timeToExplodeMax;
        timeToExplode -= Time.deltaTime;
        line.transform.GetComponent<Image>().color = Color.Lerp(line.transform.GetComponent<Image>().color, endColor, changeColorSpeed);
        line.GetComponent<Animator>().speed = (-timeToExplode + timeToExplodeMax) / 5;

        //Таймер для вбивства боса і якщо він рівний 0 то гравець програє
        if (timeToExplode <= 0)
        {
            playerHealth.GetComponent<Health>().playerHealthPoint = 0;
            playerHealth.GetComponent<Health>().playerHealthPointImg.fillAmount = 0;
            playerHealth.GetComponent<Animator>().SetBool("IsHit", true);
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        line.SetActive(false);
        line.GetComponent<Image>().fillAmount = 1;
        if (timeToExplode > 0)
        {

            timeToExplode = timeToExplodeMax;
        }
    }
}
