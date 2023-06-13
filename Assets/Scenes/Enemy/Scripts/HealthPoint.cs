using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HealthPoint : MonoBehaviour
{
    public GameObject player;
    public float stepKick;
    public float stepKickMax;
    public bool isKick;
    public float healthPoint;
    public float healthPointMax;
    public EXP expiriancePoint;
    public bool IsBobs;
    public Animator anim;
    public GameObject bodyAnim;
    public GameObject VFX_Deadarticle;
    public bool isBurn;
    public float burnTime;
    public float burnDamage;
    public float burnTick;
    public float burnTickMax;
    // Start is called before the first frame update
    void Start()
    {
        healthPointMax = healthPoint;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isBurn)
        {
            burnTime -= Time.deltaTime;
            if (burnTime <= 0)
            {
                isBurn = false;
            }
            burnTick -= Time.deltaTime;
            if (burnTick <= 0)
            {
                Debug.Log(1);
                ChangeToKick();
                healthPoint -= burnDamage;
                ChangeToNotKick();
                burnTick = burnTickMax;
            }

        }
        ChangeToNotKick();
        if (healthPoint <= 0)
        {
            if (IsBobs)
            {
                gameObject.GetComponentInParent<DropItems>().OnDestroyBoss();
            }
            EXP a = Instantiate(expiriancePoint, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1.9f), Quaternion.identity);
            a.GetComponent<EXP>().expBuff = healthPointMax / 5;

            player.GetComponent<Expirience>().expiriencepoint.fillAmount += healthPointMax / (player.GetComponent<Expirience>().level * 10);
            player.GetComponent<Expirience>().ShowLevel();
            FindObjectOfType<KillCount>().score += 1;
            Destroy(gameObject.transform.root.gameObject);
        }
    }
    
    public void ChangeToKick()
    {
        for (int i = 0; i < gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>().Length; i++)
        {
            gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>()[i].color = new Color32(255, 0, 0, 255);
        }
        isKick = true;
    } 
    public void ChangeToNotKick()
    {
        if (isKick == true)
        {
            stepKick -= Time.deltaTime;
            if (stepKick <= 0)
            {
                for (int i = 0; i < gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>().Length; i++)
                {
                    gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>()[i].color = new Color32(255, 255, 255, 255);
                }
                isKick = false;
                stepKick = stepKickMax;
            }
        }
    }
}
