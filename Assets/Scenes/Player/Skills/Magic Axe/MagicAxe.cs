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
    
    // Start is called before the first frame update
    void Start()
    {
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
        transform.localScale = new Vector2(basa.radius * PlayerManager.instance.Steam, basa.radius * PlayerManager.instance.Steam);
        //a.cold = cold * coldElement.Cold;
        transform.position = PlayerManager.instance.transform.position;
        // Нормування напрямку курсора
        directionToCursor = PlayerManager.instance.GetMousDirection(PlayerManager.instance.transform.position);

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
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotationSpeed);
        if (!isBack)
        {
            if (!isCreated)
            {
                // Рух об'єкта в напрямку курсора
                transform.position += new Vector3(directionToCursor.x * speed * 0.1f, directionToCursor.y * speed * 0.1f, 0f);
            }
            else
            {
                if (isFirst)
                {
                    // Рух об'єкта в напрямку курсора
                    //transform.position -= Quaternion.AngleAxis(-80f, Vector3.down) * directionToCursor * speed * 0.03f;
                    transform.position += new Vector3(directionToCursor.x * directionToCursorForFirstAxe.x * speed * 0.6f, 
                        directionToCursor.y * directionToCursorForFirstAxe.y * speed * 0.4f, 0f);

                }
                else
                {
                    // Рух об'єкта в напрямку курсора
                    transform.position += new Vector3(directionToCursor.x * directionToCursorForSecondAxe.x * speed * 0.6f,
                        directionToCursor.y * directionToCursorForSecondAxe.y * speed * 0.4f, 0f);
                }
            }
           
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
        playerDirection = PlayerManager.instance.transform.position;

        Vector3 currentPosition = transform.position;

        Vector3 forwardVelocity = currentPosition - playerDirection;

        transform.position = Vector3.MoveTowards(currentPosition, playerDirection, 1.5f);

        // Якщо об'єкт повернувся в початкову позицію, то знищи його
        if (forwardVelocity.magnitude < 5f)
        {
            if (basa.stats[4].isTrigger && !isCreated)
            {
                MagicAxe a = Instantiate(this, transform.position, Quaternion.identity);
                a.isCreated = true; 
                MagicAxe b = Instantiate(this, transform.position, Quaternion.identity);
                b.isCreated = true;
                b.isFirst = true;
            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            collision.GetComponent<HealthPoint>().TakeDamage(basa.damage);
        }
    }
}
