using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vortex : SkillBaseMono
{
    public List<Transform> movingObjects = new List<Transform>(); // Список рухомих об'єктів
    public float bump;
    public bool isClone;
    Vector3 centerPosition;
    Collider2D circleCollider2D;
    // Start is called before the first frame update
    void Start()
    {
        movingObjects.Clear();
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1.9f; // Задаємо Z-координату для об'єкта
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (!isClone)
        {
            transform.position = worldPosition;
            if (basa.stats[4].isTrigger)
            {
                Vortex b = Instantiate(this, new Vector3(transform.position.x + UnityEngine.Random.Range(-30, 30), transform.position.y + UnityEngine.Random.Range(-30, 30), 1.9f), Quaternion.identity);
                b.basa.lifeTime = basa.lifeTime;
                b.isClone = true;
            }
        }
        if (basa.stats[1].isTrigger)
        {
            basa.lifeTime += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.radius += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.stepMax -= basa.stats[3].value;
            basa.skill.skillCD -= StabilizateCurrentReload(basa.skill.skillCD, basa.stats[2].value);
            basa.stats[3].isTrigger = false;
        }

        transform.localScale = new Vector2(basa.radius * PlayerManager.instance.Steam, basa.radius * PlayerManager.instance.Steam);
        centerPosition = transform.position; // Визначте потрібну позицію центрального об'єкта тут


        StartCoroutine(Destroy());
        circleCollider2D = GetComponent<CircleCollider2D>();
        Debug.Log(centerPosition);
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        //DamageDeal();
        Destroy(gameObject);
    }
    //void DamageDeal()
    //{
    //    foreach (Transform movingObject in movingObjects)
    //    {
    //        if (movingObject.GetComponent<EnemyState>() != null)
    //        {
    //            EnemyState health = movingObject.GetComponent<EnemyState>();

    //            ElementActiveDebuff debuff = movingObject.GetComponent<ElementActiveDebuff>();
    //            debuff.StartCoroutine(debuff.EffectTime(Elements.status.Wind, 5));

    //            EnemyController.instance.TakeDamage(health, basa.damage * PlayerManager.instance.Wind * PlayerManager.instance.Steam
    //                / (debuff.elements.CurrentStatusValue(Elements.status.Wind) * debuff.elements.CurrentStatusValue(Elements.status.Steam)));
    //        }
    //    }
    //}
    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = movingObjects.Count - 1; i >= 0; i--)
        {
            Transform movingObject = movingObjects[i];
            Collider2D objectCollider = movingObject.GetComponent<Collider2D>();

            if (movingObject == null || objectCollider == null)
            {
                movingObjects.RemoveAt(i);
                continue;
            }

            if (Physics2D.OverlapCollider(circleCollider2D, new ContactFilter2D(), new Collider2D[1]) > 0)
            {
                Vector3 offset = centerPosition - movingObject.position; 
                float distance = offset.magnitude;
                Vector3 direction = offset.normalized;
                float force = distance * bump; 

                movingObject.position += direction * force * Time.fixedDeltaTime;
            }
            else
            {
                movingObjects.RemoveAt(i);
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            if (collision.CompareTag("Enemy") && !movingObjects.Contains(collision.transform))
            {
                movingObjects.Add(collision.transform);
                collision.GetComponent<ElementActiveDebuff>().ApplyEffect(Elements.status.Wind, 5);
            }
            else if (collision.CompareTag("Player"))
            {
                PlayerManager move = collision.GetComponent<PlayerManager>();
                if (move != null && !movingObjects.Contains(move.objTransform))
                {
                    movingObjects.Add(move.objTransform);
                }
            }
        }
    }
}
