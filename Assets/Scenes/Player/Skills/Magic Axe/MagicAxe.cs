using FSMC.Runtime;
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
    public float timeToBack = 1f;
    Transform objTransform;
    public Sphere sphereAxe;
    private void Start()
    {
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
        objTransform.localScale = new Vector2(basa.radius * PlayerManager.instance.Cold, basa.radius * PlayerManager.instance.Cold);
        objTransform.position = PlayerManager.instance.ShootPoint.transform.position;

        // Нормування напрямку курсора
        directionToCursor = PlayerManager.instance.GetMousDirection(PlayerManager.instance.ShootPoint.transform.position);

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
                objTransform.position += new Vector3(directionToCursor.x * speed * 0.1f, directionToCursor.y * speed * 0.1f, 0f);
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
            ElementActiveDebuff debuff = collision.GetComponent<ElementActiveDebuff>();
            debuff.ApplyEffect(status.Cold, 5);

            collision.GetComponent<FSMC_Executer>().TakeDamage(basa.damage, damageMultiplier);
        }
    }
}
