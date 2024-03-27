using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tornado_PosChange : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Базова швидкість обертання
    public float bulletRotationMultiplier = 2.0f; // Множник швидкості обертання для куль

    public List<Transform> movingObjects = new List<Transform>(); // Список рухомих об'єктів
    private bool isInZone = false;
    public GameObject stump;
    public List<AudioClip> stumpSounds;
    public float bump;
    Transform objTransform;
    private void Start()
    {
        objTransform = transform;
    }
    private void FixedUpdate()
    {
        if (isInZone)
        {
            Vector3 centerPosition = objTransform.position; // Визначте потрібну позицію центрального об'єкта тут

            foreach (Transform movingObject in movingObjects)
            {
                if (movingObject != null)
                {

                    Vector3 offset = movingObject.position - centerPosition;

                    float rotationMultiplier = 1.0f; // Множник швидкості обертання

                    if (movingObject.CompareTag("Bullet"))
                    {
                        rotationMultiplier = bulletRotationMultiplier; // Встановити більший множник для куль
                    }

                    float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
                    angle += rotationSpeed * rotationMultiplier * Time.deltaTime;

                    Vector3 desiredPosition = centerPosition + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * offset.magnitude;
                    movingObject.position = desiredPosition;
                }
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tree") && !collision.isTrigger)
        {
            Instantiate(stump, collision.transform.position, Quaternion.identity);
            AudioManager.instance.PlaySFX("Stump");
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            AddToZone(collision);
        }
        if (collision.CompareTag("Bullet"))
        {
            AddToZone(collision);
            RemoveFromZone(collision);
        }
    }
    void AddToZone(Collider2D collision)
    {
        if (!movingObjects.Contains(collision.transform))
        {
            movingObjects.Add(collision.transform);
        }
        isInZone = true;
    }
    void RemoveFromZone(Collider2D collision)
    {
        if (collision == null)
        {
            movingObjects.Remove(collision.transform);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (movingObjects.Contains(collision.transform))
            {
                movingObjects.Remove(collision.transform);
            }

            if (movingObjects.Count == 0)
            {
                isInZone = false;
            }
        }
        if (collision.CompareTag("Bullet"))
        {
            if (movingObjects.Contains(collision.transform))
            {
                movingObjects.Remove(collision.transform);
            }
            if (movingObjects.Count == 0)
            {
                isInZone = false;
            }
        }
    }
}
