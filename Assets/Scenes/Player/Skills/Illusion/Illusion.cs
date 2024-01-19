using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illusion : SkillBaseMono
{
    public Zzap zzap;
    public float x;
    public float y;
    public float xZzap;
    public float yZzap;
    public bool isFive;
    public float angle;

    bool isTriggeredTwo;
    public float attackSpeed;
    public float attackSpeedMax;

    PlayerManager player;
    

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        //basa = SetToSkillID(gameObject);
        x = -10;
        y = 10;
        xZzap = 2;
        yZzap = -10;
        angle = 0;
        if (basa.isReadyToDo)
        {
            CreateClone(-5, -5, 10, 0, 112);
            if (isTriggeredTwo)
            {
                CreateClone(10, 0, -10, 5, 240);
            }
        }
       
        attackSpeed = player.attackSpeed / player.Wind;
        attackSpeedMax = attackSpeed;
        if (isFive)
        {
            Zzap a = Instantiate(zzap, transform.position, Quaternion.Euler(0, 0, angle));
            a.copie = gameObject;
            a.x = xZzap;
            a.y = yZzap;
            a.electicElement = player.Electricity;
            a.lifeTime = basa.lifeTime;
        }
        StartCoroutine(TimerSpell());
    }
    private void CreateClone(float x, float y, float xZzap, float yZzap, float angle)
    {
        Illusion a = Instantiate(this, transform.position, Quaternion.identity);
        a.x = x;
        a.y = y;
        a.xZzap = xZzap;
        a.yZzap = yZzap;
        a.angle = angle;
        a.basa.lifeTime = basa.lifeTime;
        a.isFive = isFive;
    }
    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(basa.lifeTime);

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(player.transform.position.x + x, player.transform.position.y + y);
        player.ShootBullet();
    }
}
