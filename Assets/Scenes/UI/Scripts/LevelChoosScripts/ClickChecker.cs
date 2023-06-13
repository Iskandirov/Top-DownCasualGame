using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickChecker : MonoBehaviour
{
    public bool leftButtonPressed;
    public bool rightButtonPressed;

    private void Update()
    {
        // ���������� ���������� ��� ������ ����
        if (Input.GetMouseButtonDown(0))
        {
            leftButtonPressed = true;

            // ���� ����� ������ ���� ���������, ��������� ��
            if (rightButtonPressed)
            {
                rightButtonPressed = false;
                // ������� ��� ��� ��� ������� ���������� ����� ������ ����
            }
        }

        // ���������� ���������� ����� ������ ����
        if (Input.GetMouseButtonDown(1))
        {
            rightButtonPressed = true;

            // ���� ��� ������ ���� ���������, ��������� ��
            if (leftButtonPressed)
            {
                leftButtonPressed = false;
                // ������� ��� ��� ��� ������� ���������� ��� ������ ����
            }
        }
    }
}
