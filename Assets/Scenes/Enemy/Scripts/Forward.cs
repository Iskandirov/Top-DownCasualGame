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
    public float movementSpeed = 5f;  // Швидкість руху об'єкту
    public float circleRadius = 15f;  // Радіус кола
    public float showDistance;
    public bool isSummoned;
    public bool enemyFinded;
    public float summonTime;
    public bool isThree;
    public GameObject bomb;
    public Animator anim;

    private Collider2D[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        isStunned = false;
        player = GameObject.FindWithTag("Player");
        colliders = new Collider2D[10];
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

        // Перевірка, чи об'єкт знаходиться поза межами камери за її горизонтальними і вертикальними координатами
        if (objectViewportPosition.x < -0.5f || objectViewportPosition.x > 1.45f ||
         objectViewportPosition.y < -0.5f || objectViewportPosition.y > 1.45f)
        {
            return true; // Об'єкт знаходиться поза межами камери
        }

        return false; // Об'єкт знаходиться в межах камери
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
        speed = speedMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsObjectOutsideCameraBounds(gameObject))
        {
            anim.enabled = false;
            speed = speedMax * 100;
        }
        else
        {
            anim.enabled = true;
            speed = speedMax;
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
            Body.position = Vector3.MoveTowards(Body.position, player.transform.position, speed * Time.deltaTime);
        }

        if (Body.position.x < player.transform.position.x)
        {
            Body.transform.rotation = Quaternion.identity;
        }
        else
        {
            Body.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // Визначаємо відстань від гравця до ворога
        float distanceToEnemy = Vector3.Distance(player.transform.position, transform.position);

        // Перевіряємо, чи ворог знаходиться близько
        if (distanceToEnemy < showDistance)
        {
            // Зникаємо об'єкт LocationPoint
            LocationPoint.SetActive(false);
        }
        else
        {
            // З'являємо об'єкт LocationPoint
            LocationPoint.SetActive(true);

            // Визначаємо відстань від гравця до позиції курсора
            Vector3 direction = transform.position - player.transform.position;
            direction = direction.normalized * circleRadius;

            // Визначаємо кінцеву позицію об'єкту на колі
            Vector3 targetPosition = player.transform.position + direction;

            // Рухаємо об'єкт до кінцевої позиції
            LocationPoint.transform.position = targetPosition;

            // Повертаємо коло в напрямку курсора
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            LocationPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Stuned()
    {
        if (stunnTime <= 0)
        {
            speed = speedMax;
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
