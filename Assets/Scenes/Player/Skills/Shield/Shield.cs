using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : SkillBaseMono
{
    public float healthShield;
    public float healthShieldMissed;
    PlayerManager player;
    public SlowArea slowObj;
    public float rockDamage;
    public GameObject rockObj;
    public float dirtElement;
    Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objTransform = transform;
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        objTransform.localScale = new Vector2(objTransform.localScale.x + basa.radius, objTransform.localScale.y + basa.radius);
        healthShield = basa.damage;
        player.shildActive = true;
        dirtElement = player.Dirt;
        if (basa.stats[3].isTrigger)
        {
            SlowArea a = Instantiate(slowObj, objTransform.position, Quaternion.identity, objTransform);
            a.dirtElement = dirtElement;
        }
        CoroutineToDestroy(gameObject, basa.lifeTime);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        objTransform.position = player.objTransform.position;
        if (basa.stats[4].isTrigger)
        {
            if (healthShieldMissed > 10f)
            {
                float i = Mathf.Floor(healthShieldMissed / 10f);
                for (int y = 0; y < i; y++)
                {
                    GameObject newObject = Instantiate(rockObj, new Vector2(objTransform.position.x + Random.Range(-20, 20), objTransform.position.y + Random.Range(-20, 20)), Quaternion.identity);


                    // �������� ������� � ����������� ��'������
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(newObject.transform.position, newObject.GetComponent<Collider2D>().bounds.extents.x);

                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.CompareTag("Enemy"))
                        {
                            // ��������, �� �������� �������� � ����� ��'����� (�� ����� �����)
                            if (collider.gameObject != newObject)
                            {
                                HealthPoint health = collider.GetComponent<HealthPoint>();
                                health.healthPoint -= (rockDamage * dirtElement * health.Dirt) / health.Grass;
                                // ��������� �������� 䳿 ��� ������� ��'����
                                Debug.Log("Object collided with: " + collider.name);
                            }
                        }
                    }
                }
                healthShieldMissed = 0;
            }
        }
        if (healthShield <= 0) 
        {
            Destroy(gameObject);
        }
    }
}
