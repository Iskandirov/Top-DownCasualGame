using UnityEngine;

public class Forward : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D Body;
    public float speed;
    public float speedMax;
    public bool isStunned;
    public float stunnTime;
    public float stunnTimeMax;
    public bool isFly;
    public bool isShooted;

    public GameObject LocationPoint;
    public float movementSpeed = 5f;  
    public float circleRadius = 15f;  
    public float showDistance;
    public bool isSummoned;
    public bool enemyFinded;
    public float summonTime;
    public bool isThree;
    public GameObject bomb;
    public Animator anim;
    public bool IsOutOfCamera;

    private Collider2D[] colliders;
    public bool isChaising;

    public float acceleration = 0.1f; // збільшення швидкості
    public float angle;

    private Vector2 velocity;


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
#if UNITY_EDITOR
            // Код для режиму редактора Unity
            speed = speedMax;
#else
    speed = speedMax * 7;
#endif
            isStunned = false;
            player = GameObject.FindWithTag("Player");
            colliders = new Collider2D[10];
        }
    }

    public void FindEnemy()
    {
        int colliderCount = Physics2D.OverlapCircleNonAlloc(gameObject.transform.position, 16f, colliders);

        for (int i = 0; i < colliderCount; i++)
        {
            Collider2D collider = colliders[i];

            if (collider.isTrigger != true && collider.CompareTag("Enemy") && collider.transform.parent.gameObject != gameObject)
            {
                HealthPoint healthPoint = collider.GetComponent<HealthPoint>();

                if (healthPoint != null)
                {
                    player = collider.gameObject;
                    enemyFinded = true;
                }
            }
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
        isShooted = false;
        Body.velocity = Vector2.zero;
    }

    public void StopMove()
    {
        speed = 0;
    }

    public void StartMove()
    {
#if UNITY_EDITOR
        // Код для режиму редактора Unity
        speed = speedMax;
#else
    speed = speedMax * 7;
#endif
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
                Invoke("MoveEnd", 0.2f);
            }

            if (isSummoned)
            {
                summonTime -= Time.deltaTime;

                if (!enemyFinded || player == null)
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

            if (player == null)
            {
                player = GameObject.FindWithTag("Player");
            }

            if (isStunned)
            {
                Stuned();
            }

            if (!isFly && !isShooted)
            {
                Body.MovePosition(Vector2.MoveTowards(Body.position, player.transform.position, speed * Time.deltaTime));
            }

            if (Body.position.x < player.transform.position.x)
            {
                Body.transform.rotation = Quaternion.identity;
            }
            else
            {
                Body.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            float distanceToEnemy = Vector3.Distance(player.transform.position, transform.position);

            if (distanceToEnemy < showDistance)
            {
                LocationPoint.SetActive(false);
            }
            else
            {
                LocationPoint.SetActive(true);

                Vector3 direction = transform.position - player.transform.position;
                direction = direction.normalized * circleRadius;

                Vector3 targetPosition = player.transform.position + direction;

                LocationPoint.transform.position = targetPosition;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                LocationPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
#if UNITY_EDITOR
            // Код для режиму редактора Unity
            speed = speedMax;
#else
    speed = speedMax * 7;
#endif
            isStunned = false;
            stunnTime = stunnTimeMax;
        }
        else
        {
            speed = 0;
            stunnTime -= Time.deltaTime;
        }
    }

    public void Relocate(Transform pos)
    {
        gameObject.transform.position = new Vector2(pos.position.x, pos.position.y + 10);
    }
}