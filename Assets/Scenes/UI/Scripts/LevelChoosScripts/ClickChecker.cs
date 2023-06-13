using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickChecker : MonoBehaviour
{
    public bool leftButtonPressed;
    public bool rightButtonPressed;

    private void Update()
    {
        // Перевіряємо натискання лівої кнопки миші
        if (Input.GetMouseButtonDown(0))
        {
            leftButtonPressed = true;

            // Якщо права кнопка була натиснута, відпускаємо її
            if (rightButtonPressed)
            {
                rightButtonPressed = false;
                // Додайте тут код для обробки відпускання правої кнопки миші
            }
        }

        // Перевіряємо натискання правої кнопки миші
        if (Input.GetMouseButtonDown(1))
        {
            rightButtonPressed = true;

            // Якщо ліва кнопка була натиснута, відпускаємо її
            if (leftButtonPressed)
            {
                leftButtonPressed = false;
                // Додайте тут код для обробки відпускання лівої кнопки миші
            }
        }
    }
}
