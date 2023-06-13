using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DestroyVFX : MonoBehaviour
{
    public float delay;
    public bool isGoingAttack;
    public GameObject Bullet;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("CreateObject", delay);// Запускаємо таймер на створення об'єкта
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void CreateObject()
    {
        if (isGoingAttack)
        {
            // Створюємо новий об'єкт у місці тригера
            Instantiate(Bullet, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
