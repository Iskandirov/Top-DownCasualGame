using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilChasePlayer : MonoBehaviour
{
    PlayerManager player; // Гравець
    public Transform eyePupil; // Зрачок
    public Transform eyeBounds; // Межі ока

    public float maxPupilSpeed = 5.0f; // Максимальна швидкість руху зрачка
    public float maxPupilDistance = 0.5f; // Максимальна відстань зрачка від центру ока
    private void Start()
    {
        player = PlayerManager.instance;
    }
    private void FixedUpdate()
    {
        // Визначаємо вектор від зрачка до гравця
        Vector3 directionToPlayer = player.transform.position - eyePupil.position;

        // Обчислюємо нову позицію зрачка
        Vector3 newPosition = eyePupil.position + directionToPlayer.normalized * maxPupilSpeed * Time.fixedDeltaTime;

        // Обмежуємо рух зрачка в межах області ока
        Vector3 boundedPosition = Vector3.ClampMagnitude(newPosition - eyeBounds.position, eyeBounds.localScale.x / 2 - maxPupilDistance);
        eyePupil.position = boundedPosition + eyeBounds.position;
    }
}
