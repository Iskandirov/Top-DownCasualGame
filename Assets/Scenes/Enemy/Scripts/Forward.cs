using Pathfinding;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Forward : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public Rigidbody2D Body;
    public float speed;
    public float speedMax;
    public bool isStunned;
    public float stunnTime;
    public float stunnTimeMax;
    public bool isFly;
    public bool isShooted;

    public bool isSummoned;
    public bool enemyFinded;
    public bool protectPlayer;
    public float summonTime;
    public bool isThree;
    public GameObject bomb;
    public Animator anim;
    public bool IsOutOfCamera;

    public bool isChaising;

    public float acceleration = 0.1f; // збільшення швидкості
    public float angle;

    private Vector2 velocity;
    AIPath path;
    AIDestinationSetter destination;
    private void Awake()
    {
        if (FindObjectOfType<KillCount>().LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) > 0)
            speedMax *= FindObjectOfType<KillCount>().LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) * 0.2f;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isChaising)
        {
            // встановлюємо вектор швидкості в початкове значення (нульовий вектор)
            velocity = Vector2.zero;
            gameObject.AddComponent<Rigidbody2D>();
            Body = GetComponent<Rigidbody2D>();
            Body.gravityScale = 0;
            Body.angularDrag = 0;
            Body.mass = 5000;
            Body.freezeRotation = true;
        }
        else
        {
            // Код для режиму редактора Unity
            isStunned = false;
            player = GameObject.FindWithTag("Player");
        }
        path = GetComponent<AIPath>();
        destination = GetComponent<AIDestinationSetter>();
        destination.target = player.transform;
        path.maxSpeed = speedMax;
    }

    public void FindEnemy()
    {
        // Отримуємо всі колайдери, які перетинають колайдер об'єкта
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 16f);

        // По замовчуванню ми не знаходимо ворога
        enemyFinded = false;

        foreach (Collider2D collider in colliders)
        {
            // Перевіряємо, чи колайдер належить ворогові, а не цьому ж об'єкту або гравцеві
            if (collider.CompareTag("Enemy") && collider.gameObject != gameObject && collider.gameObject != player)
            {
                HealthPoint healthPoint = collider.GetComponent<HealthPoint>();
                if (healthPoint != null)
                {
                    enemy = collider.gameObject;
                    enemyFinded = true;
                    protectPlayer = false;
                    // Виходимо з циклу, оскільки знайшли ворога
                    break;
                }
            }
        }

        // Якщо не знайдено ворога, встановлюємо protectPlayer в true
        if (!enemyFinded)
        {
            protectPlayer = true;
        }
    }
    bool IsObjectOutsideCameraBounds(GameObject obj)
    {
        Camera mainCamera = Camera.main;

        Vector3 objectViewportPosition = mainCamera.WorldToViewportPoint(obj.transform.position);

        bool isOutsideBounds = objectViewportPosition.x < -0.5f || objectViewportPosition.x > 1.45f ||
                               objectViewportPosition.y < -0.5f || objectViewportPosition.y > 1.45f;

        IsOutOfCamera = isOutsideBounds;
        return isOutsideBounds;
    }
    public void MoveEnd()
    {
        path.maxSpeed = speedMax;
        isShooted = false;
    }

    public void StopMove()
    {
        path.maxSpeed = 0;
    }

    public void StartMove()
    {
        path.maxSpeed = speedMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isChaising)
        {
            if (IsObjectOutsideCameraBounds(gameObject))
            {
                anim.enabled = false;
            }
            else
            {
                anim.enabled = true;
            }
            if (isShooted)
            {
                Invoke("MoveEnd", 0.1f);
            }

            if (isSummoned)
            {
                summonTime -= Time.deltaTime;

                if (!enemyFinded || enemy == null)
                {
                    FindEnemy();
                }
                
                if (summonTime <= 0)
                {
                    isSummoned = false;
                    enemyFinded = false;

                    if (isThree)
                    {
                        Instantiate(bomb, transform.position, Quaternion.identity);
                        Destroy(gameObject);
                    }
                }
            }

           

            if (isStunned)
            {
                Stuned();
            }

            if (!isFly && !isShooted && !isSummoned)
            {
                destination.target = player.transform;
            }
            else if(isSummoned && !enemyFinded)
            {
                destination.target = null;
            }

            if (Body.position.x < player.transform.position.x)
            {
                Body.transform.rotation = Quaternion.identity;
            }
            else
            {
                Body.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

           
        }
        else
        {
            // визначаємо напрямок до гравця
            Vector2 direction = player.transform.position - transform.position;
            direction.Normalize();

            // визначаємо кут між напрямком до гравця і поточним напрямком руху об'єкту
            angle = Vector2.Angle(velocity, direction);

            // якщо кут між векторами більший за 100 градусів - зменшуємо швидкість
            if (angle > 20f)
            {
                // зменшуємо швидкість з поступовим нарощуванням
                velocity = Vector2.Lerp(velocity, direction * speedMax, Time.deltaTime * acceleration);
            }
            else // якщо кут менший - збільшуємо швидкість
            {
                // збільшуємо швидкість з поступовим нарощуванням
                velocity = Vector2.Lerp(velocity, direction * speedMax, Time.deltaTime * acceleration);
            }

            // обмежуємо максимальну швидкість
            velocity = Vector2.ClampMagnitude(velocity, speedMax);

            // зміщуємо об'єкт на відстань, що дорівнює швидкості, помноженій на час оновлення
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    public void Stuned()
    {
        if (stunnTime <= 0)
        {
            // Код для режиму редактора Unity
            path.maxSpeed = speedMax;
            isStunned = false;
            stunnTime = stunnTimeMax;
        }
        else
        {
            path.maxSpeed = 0;
            stunnTime -= Time.deltaTime;
        }
    }

    public void Relocate(Transform pos)
    {
        gameObject.transform.position = new Vector2(pos.position.x, pos.position.y + 10);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.isTrigger && !protectPlayer && isSummoned && collision.GetComponent<Expirience>().isEnemyInZone > 0)
        {
            //Debug.Log("Attact enemy");
            destination.target = enemy.transform;
        }
        else if (collision.CompareTag("Player") && collision.isTrigger && !protectPlayer && isSummoned && collision.GetComponent<Expirience>().isEnemyInZone <= 0)
        {
            destination.target = player.transform;
            //destination.target = null;

            //float distance = 10f; // Відстань від початкової позиції
            //Vector3 offset = new Vector3(Mathf.Cos(Time.time * 2) * distance, Mathf.Sin(Time.time * 2) * distance, 0f);
            //// Визначте нову позицію, додавши зміщення до початкової позиції
            //Vector3 newPosition = initialPosition + offset;
            //transform.position = newPosition;

            //Debug.Log("Protect player");
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.isTrigger && protectPlayer && isSummoned)
        {
            //Debug.Log("Go to player");
            destination.target = player.transform;
        }
    }
}