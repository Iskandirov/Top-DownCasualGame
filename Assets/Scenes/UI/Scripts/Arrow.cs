using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform arrow;             // Об'єкт, на який показує стрілка
    public Transform target;            // Об'єкт, на який показує стрілка
    private Transform player;           // Гравець
    public Camera mainCamera;
    public float showArrowDistance = 5f; // Мінімальна відстань для показу стрілки
    public float edgePadding = 0.1f;     // Відступи від країв екрану
    public float arrowSmoothness = 15f;  // Чим більше, тим швидше реагує (10-20 оптимально)

    private Vector3 currentArrowPos;
    public List<Vector3> blockSpawnPos;
    private System.Random rand = new System.Random();
    [SerializeField] private bool blockTrap;
    private void Start()
    {
        if (blockTrap)
        {
            target.transform.position = blockSpawnPos[rand.Next(blockSpawnPos.Count)];
        }
        mainCamera = Camera.main;
        player = PlayerManager.instance.transform;
        if (arrow != null)
            currentArrowPos = arrow.position;
    }

    void Update()
    {
        if (target == null || player == null || mainCamera == null)
            return;

        // Розрахунок відстані між гравцем і ціллю
        float distanceToTarget = Vector3.Distance(player.position, target.position);

        // Ввімкнути стрілку, якщо об'єкт далеко
        arrow.gameObject.SetActive(distanceToTarget > showArrowDistance);

        if (distanceToTarget <= showArrowDistance)
            return;

        // Центр екрану
        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0f) / 2f;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
        Vector3 dirFromCenter = screenPos - screenCenter;
        dirFromCenter.z = 0;

        // Обмеження країв екрану
        float maxX = Screen.width * (1f - edgePadding);
        float minX = Screen.width * edgePadding;
        float maxY = Screen.height * (1f - edgePadding);
        float minY = Screen.height * edgePadding;

        // Знаходимо точку на краю екрана в напрямку до цілі
        Vector3 cappedScreenPos = screenCenter + dirFromCenter.normalized * 1000f;
        cappedScreenPos.x = Mathf.Clamp(cappedScreenPos.x, minX, maxX);
        cappedScreenPos.y = Mathf.Clamp(cappedScreenPos.y, minY, maxY);

        // Перетворюємо в world-space позицію
        Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(cappedScreenPos.x, cappedScreenPos.y, 10f));
        targetWorldPos.z = 0;

        // Плавно рухаємо стрілку до цільової позиції
        currentArrowPos = Vector3.Lerp(currentArrowPos, targetWorldPos, arrowSmoothness * Time.deltaTime);
        arrow.position = currentArrowPos;

        // Повертаємо стрілку в напрямку до цілі
        Vector3 dirToTarget = (target.position - arrow.position).normalized;
        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
