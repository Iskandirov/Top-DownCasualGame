using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vortex : MonoBehaviour
{
    public List<Transform> movingObjects = new List<Transform>(); // Список рухомих об'єктів
    public float bump;
    public float lifeTime;
    public float damage;
    public float Steam;
    public float Wind;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(lifeTime);
        DamageDeal();
        Destroy(gameObject);
    }
    void DamageDeal()
    {
        foreach (Transform movingObject in movingObjects)
        {
            movingObject.GetComponentInChildren<HealthPoint>().healthPoint -= (damage * Wind * Steam) / (movingObject.GetComponentInChildren<HealthPoint>().Wind * movingObject.GetComponentInChildren<HealthPoint>().Steam);
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
