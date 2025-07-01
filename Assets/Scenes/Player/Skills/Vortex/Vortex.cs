using Pathfinding;
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
        //Debug.Log(centerPosition);
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(basa.lifeTime);

        // Повертаємо керування AIPath для всіх ворогів, які ще залишились у списку
        foreach (var movingObject in movingObjects)
        {
            if (movingObject == null) continue;
            var aiPath = movingObject.GetComponent<Pathfinding.AIPath>();
            if (aiPath != null)
            {
                aiPath.enabled = true;
                aiPath.canMove = true;
            }
        }

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
        // Оновлюємо центр вихору кожен кадр
        centerPosition = transform.position;

        for (int i = movingObjects.Count - 1; i >= 0; i--)
        {
            Transform movingObject = movingObjects[i];
            if (movingObject == null)
            {
                movingObjects.RemoveAt(i);
                continue;
            }

            // Перевірка чи ворог ще в зоні вихору
            if (Physics2D.OverlapPoint(movingObject.position, LayerMask.GetMask("Enemy", "Player")))
            {
                Vector3 offset = centerPosition - movingObject.position;
                float distance = offset.magnitude;
                if (distance < 0.01f) continue; // вже в центрі

                Vector3 direction = offset.normalized;
                float force = distance * bump;
                // Пріоритет: Rigidbody2D -> AIPath -> Transform
                Rigidbody2D rb = movingObject.GetComponent<Rigidbody2D>();
                //Debug.Log($"Vortex force: {force}, direction: {direction}, bump: {bump}, distance: {distance}, mass: {rb.mass}");

                var aiPath = movingObject.GetComponent<AIPath>();

                if (rb != null)
                {
                    
                    if (aiPath != null)
                    {
                        aiPath.canMove = false;
                        aiPath.enabled = false; // Повністю вимикаємо AIPath
                    }
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rb.AddForce(direction * force, ForceMode2D.Impulse);
                }
                else if (aiPath != null)
                {
                    // Вимикаємо рух AIPath і рухаємо вручну
                    aiPath.canMove = false;
                    movingObject.position += direction * force * Time.fixedDeltaTime;
                }
                else
                {
                    // Якщо нічого немає — рухаємо трансформ
                    movingObject.position += direction * force * Time.fixedDeltaTime;
                }
            }
            else
            {
                // Ворог вийшов із зони — повертаємо керування AIPath
                var aiPath = movingObject.GetComponent<Pathfinding.AIPath>();
                if (aiPath != null) aiPath.canMove = true;
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
