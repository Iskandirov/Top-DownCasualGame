using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vortex : SkillBaseMono
{
    public List<Transform> movingObjects = new List<Transform>(); // Список рухомих об'єктів
    public float bump;
    public float Steam;
    public float Wind;
    public bool isFive;
    
    // Start is called before the first frame update
    void Start()
    {
        //basa = SetToSkillID(gameObject);
        
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1.9f; // Задаємо Z-координату для об'єкта
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = worldPosition;
        basa.damage = basa.damage * PlayerManager.instance.Wind;
        transform.localScale = new Vector2(basa.radius * PlayerManager.instance.Steam, basa.radius * PlayerManager.instance.Steam);
        if (isFive)
        {
            Vortex b = Instantiate(this, new Vector3(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20), 1.9f), Quaternion.identity);
            b.basa.damage = basa.damage * PlayerManager.instance.Wind;
            b.basa.lifeTime = basa.lifeTime;
            b.transform.localScale = new Vector2(basa.radius * PlayerManager.instance.Steam, basa.radius * PlayerManager.instance.Steam);
        }
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        DamageDeal();
        Destroy(gameObject);
    }
    void DamageDeal()
    {
        foreach (Transform movingObject in movingObjects)
        {
            HealthPoint health = movingObject.GetComponentInChildren<HealthPoint>();
            health.healthPoint -= (basa.damage * Wind * Steam) / (health.Wind * health.Steam);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Transform movingObject in movingObjects)
        {
            Vector3 centerPosition = transform.position; // Визначте потрібну позицію центрального об'єкта тут
            if (movingObject != null)
            {
                Vector3 offset = movingObject.position - centerPosition;

                float force = Mathf.Abs(offset.magnitude) * bump;

                // Обчислення кута між вектором від предмета до центру і віссю X
                float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

                // Обчислення горизонтальної складової сили притягання
                float horizontalForce = force * Mathf.Cos(angle * Mathf.Deg2Rad);

                // Обчислення вертикальної складової сили притягання
                float verticalForce = force * Mathf.Sin(angle * Mathf.Deg2Rad);

                // Оновлення положення предмета
                movingObject.position -= new Vector3(horizontalForce * Time.fixedDeltaTime, verticalForce * Time.fixedDeltaTime, 0);
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (collision.GetComponentInParent<Forward>() != null)
            {
                if (!movingObjects.Contains(collision.GetComponentInParent<Forward>().transform))
                {
                    movingObjects.Add(collision.GetComponentInParent<Forward>().transform);
                }
            }
            
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (collision.GetComponentInParent<Forward>() != null)
            {
                if (!movingObjects.Contains(collision.GetComponentInParent<Forward>().transform))
                {
                    int index = movingObjects.IndexOf(collision.GetComponentInParent<Forward>().transform);
                    movingObjects.RemoveAt(index);
                }
            }
        }
    }
}
