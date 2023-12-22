using System.Collections;
using UnityEngine;

public class Illusion : MonoBehaviour
{
    public Zzap zzap;
    public float x;
    public float y;
    public float xZzap;
    public float yZzap;
    public float lifeTime;
    public bool isFive;
    public float angle;

    public float attackSpeed;
    public float attackSpeedMax;

    PlayerManager player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        attackSpeed = player.attackSpeed / player.Wind;
        attackSpeedMax = attackSpeed;
        if (isFive)
        {
            Zzap a = Instantiate(zzap, transform.position, Quaternion.Euler(0, 0, angle));
            a.copie = gameObject;
            a.x = xZzap;
            a.y = yZzap;
            a.electicElement = player.Electricity;
            a.lifeTime = lifeTime;
        }
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(player.transform.position.x + x, player.transform.position.y + y);
        player.ShootBullet(gameObject);
    }
}
