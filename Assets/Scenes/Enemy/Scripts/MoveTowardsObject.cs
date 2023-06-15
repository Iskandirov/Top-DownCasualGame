using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsObject : MonoBehaviour
{
    public Transform playerTransform;
    public float maxSpeed = 5f; // максимальна швидк≥сть об'Їкту
    public float acceleration = 0.1f; // зб≥льшенн€ швидкост≥
    public float angle;

    private Vector2 velocity;

    Rigidbody2D rb;
    private void Start()
    {
        // встановлюЇмо вектор швидкост≥ в початкове значенн€ (нульовий вектор)
        velocity = Vector2.zero;
        gameObject.AddComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.angularDrag = 0;
        rb.mass = 100;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // визначаЇмо напр€мок до гравц€
        Vector2 direction = playerTransform.position - transform.position;
        direction.Normalize();

        // визначаЇмо кут м≥ж напр€мком до гравц€ ≥ поточним напр€мком руху об'Їкту
        angle = Vector2.Angle(velocity, direction);

        // €кщо кут м≥ж векторами б≥льший за 100 градус≥в - зменшуЇмо швидк≥сть
        if (angle > 20f)
        {
            // зменшуЇмо швидк≥сть з поступовим нарощуванн€м
            velocity = Vector2.Lerp(velocity, direction * maxSpeed, Time.deltaTime * acceleration);
        }
        else // €кщо кут менший - зб≥льшуЇмо швидк≥сть
        {
            // зб≥льшуЇмо швидк≥сть з поступовим нарощуванн€м
            velocity = Vector2.Lerp(velocity, direction * maxSpeed, Time.deltaTime * acceleration);
        }

        // обмежуЇмо максимальну швидк≥сть
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        // зм≥щуЇмо об'Їкт на в≥дстань, що дор≥внюЇ швидкост≥, помножен≥й на час оновленн€
        transform.Translate(velocity * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // отримуЇмо вектор нормал≥ коллайдера
        Vector2 normal = collision.contacts[0].normal;

        // в≥дображенн€ вектору швидкост≥ в≥дносно нормал≥ з≥ткненн€
        velocity = Vector2.Reflect(velocity, normal) * 0.8f;

        // додаванн€ сили в залежност≥ в≥д вектору нормал≥
        rb.AddForce(normal * 5f, ForceMode2D.Impulse);
    }
}