using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAxe : SkillBaseMono
{
    public float rotationSpeed = 500f;
    public float speed = 15f;
    public float speedBack = 15f;
    Vector3 playerDirection;
    bool isBack;
    public bool isCreated;
    public bool isFirst;
    Vector3 directionToCursor;
    Vector3 directionToCursorForFirstAxe;
    Vector3 directionToCursorForSecondAxe;
    public float timeToBack = 1f;
    Transform objTransform;
    public EnemyController enemy;
    public Sphere sphereAxe;
    private void Start()
    {
        enemy = EnemyController.instance;
        objTransform = transform;
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
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
            basa.skill.skillCD -= StabilizateCurrentReload(basa.skill.skillCD, basa.stats[3].value);
            basa.stats[3].isTrigger = false;
        }
        //basa = SetToSkillID(gameObject);
        objTransform.localScale = new Vector2(basa.radius * PlayerManager.instance.Steam, basa.radius * PlayerManager.instance.Steam);
        //a.cold = cold * coldElement.Cold;
        objTransform.position = PlayerManager.instance.objTransform.position;
        // Нормування напрямку курсора
        directionToCursor = PlayerManager.instance.GetMousDirection(PlayerManager.instance.objTransform.position);

        directionToCursorForFirstAxe = new Vector2(Mathf.Cos(-165f * Mathf.Deg2Rad), Mathf.Sin(-165f * Mathf.Deg2Rad));
        directionToCursorForSecondAxe = new Vector2(Mathf.Cos(-15f * Mathf.Deg2Rad), Mathf.Sin(-15f  * Mathf.Deg2Rad));
        // Запуск coroutine
        StartCoroutine(MoveToAndBack());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rotationSpeed += speed;
        // Оновлення кута обертання об'єкта
        objTransform.rotation = Quaternion.Euler(objTransform.rotation.x, objTransform.rotation.y, rotationSpeed);
        if (!isBack)
        {
            if (!isCreated)
            {
                // Рух об'єкта в напрямку курсора
                objTransform.position += new Vector3(directionToCursor.x * speed * 0.1f, directionToCursor.y * speed * 0.1f, 0f);
            }
            //else
            //{
            //    if (isFirst)
            //    {
            //        objTransform.position += new Vector3(directionToCursor.x * directionToCursorForFirstAxe.x * speed * 0.6f,
            //            directionToCursor.y + directionToCursorForFirstAxe.y * speed * 0.4f, 0f);

            //    }
            //    else
            //    {
            //        objTransform.position += new Vector3(directionToCursor.x * directionToCursorForSecondAxe.x * speed * 0.6f,
            //            directionToCursor.y * directionToCursorForSecondAxe.y * speed * 0.4f, 0f);
            //    }
            //}
           
        }
        else
        {
            MoveBack();
        }
    }

    public IEnumerator MoveToAndBack()
    {
        yield return new WaitForSeconds(timeToBack);
        // Запуск таймера
        isBack = true;
    }

    private void MoveBack()
    {
        playerDirection = PlayerManager.instance.objTransform.position;

        Vector3 currentPosition = objTransform.position;

        Vector3 forwardVelocity = currentPosition - playerDirection;

        objTransform.position = Vector3.MoveTowards(currentPosition, playerDirection, 1.5f);

        // Якщо об'єкт повернувся в початкову позицію, то знищи його
        if (forwardVelocity.magnitude < 5f)
        {
            if (basa.stats[4].isTrigger && !isCreated)
            {
                Instantiate(sphereAxe, objTransform.position, Quaternion.identity);
                Sphere a = Instantiate(sphereAxe, objTransform.position, Quaternion.identity);
                a.angle = 180;
            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemy.TakeDamage(collision.GetComponent<EnemyState>(), (basa.damage));
            ElementActiveDebuff debuff = collision.GetComponent<ElementActiveDebuff>();
            debuff.StartCoroutine(debuff.EffectTime(Elements.status.Cold, 5));
        }
    }
}
