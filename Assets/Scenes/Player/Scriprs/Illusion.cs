using System.Collections;
using UnityEngine;

public class Illusion : MonoBehaviour
{


    GameObject player;
    Shoot playerShoot;
    public Zzap zzap;
    public float x;
    public float y;
    public float xZzap;
    public float yZzap;
    public float lifeTime;
    public bool isFive;
    public float angle;

    public float stepShoot;
    public float attackSpeed;

    Vector2 directionBullet;
    ElementsCoeficients electicWindElement;
    // Start is called before the first frame update
    void Start()
    {
        electicWindElement = transform.root.GetComponent<ElementsCoeficients>();
        player = FindObjectOfType<Move>().gameObject;
        playerShoot = player.GetComponent<Shoot>();
        stepShoot = player.GetComponent<Shoot>().stepShoot;
        attackSpeed = player.GetComponent<Shoot>().attackSpeed / electicWindElement.Wind;
        if (isFive)
        {
            Zzap a = Instantiate(zzap, transform.position, Quaternion.Euler(0, 0, angle));
            a.copie = gameObject;
            a.x = xZzap;
            a.y = yZzap;
            a.electicElement = electicWindElement.Electricity;
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
        ShootBullet(gameObject);
    }
    public void ShootBullet(GameObject ShootPointObj)
    {
        stepShoot += Time.deltaTime;

        if (stepShoot >= attackSpeed)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButton(0))  // Перевіряємо, чи натиснута ліва кнопка миші
            {
                stepShoot = 0;
                playerShoot.shoot = ShootPointObj;
                // Отримуємо позицію курсора у світових координатах
                mousePosition.z = 0f;  // Закріплюємо координату Z на площині

                // Створюємо об'єкт з використанням префабу
                Bullet newObject = Instantiate(playerShoot.bullet, ShootPointObj.transform.position, Quaternion.identity);
                newObject.damage = playerShoot.damageToGive;
                newObject.isPiers = playerShoot.isLevelFive;
                if (playerShoot.isLevelTwo)
                {
                    for (int i = 1; i < playerShoot.secondBulletCount + 1; i++)
                    {
                        Debug.Log(1);
                        Invoke("CreateBullet", i * 0.3f);
                    }
                }
                // Визначаємо напрямок до курсора
                directionBullet = mousePosition - transform.position;

                // Запускаємо об'єкт в заданому напрямку
                Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
                rb.AddForce(directionBullet.normalized * playerShoot.launchForce, ForceMode2D.Impulse);
                float angleShot = Mathf.Atan2(directionBullet.y, directionBullet.x) * Mathf.Rad2Deg;
                newObject.transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
            }
        }
    }

    public void CreateBullet()
    {
        Bullet newObject = Instantiate(playerShoot.bullet, transform.position, Quaternion.identity);
        newObject.damage = playerShoot.damageToGive;
        newObject.isPiers = playerShoot.isLevelFive;

        // Запускаємо об'єкт в заданому напрямку
        Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
        rb.AddForce(directionBullet.normalized * playerShoot.launchForce, ForceMode2D.Impulse);
        float angleShot = Mathf.Atan2(directionBullet.y, directionBullet.x) * Mathf.Rad2Deg;
        newObject.transform.rotation = Quaternion.AngleAxis(angleShot + 0, Vector3.forward);
    }
}
