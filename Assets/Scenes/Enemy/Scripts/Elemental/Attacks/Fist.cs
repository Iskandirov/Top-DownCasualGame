using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour
{
    public GameObject fistPrefab;     // Префаб руки з анімаціями
    public Transform bossTransform;   // Автоматично підставимо трансформ боса
    public float moveSpeed = 5f;
    public float maxChaseTime = 2f;
    public float dragDuration = 0.5f;
    public float grabRange = 1f;

    public void ExecuteAttack()
    {
        // Створюємо руку в сцені
        GameObject fistObj = Instantiate(fistPrefab, transform.position, Quaternion.identity);

        // Налаштовуємо руку (Init)
        var controller = fistObj.GetComponent<FistController>();
        controller.Init(bossTransform, moveSpeed, maxChaseTime, dragDuration, grabRange);
    }
}
