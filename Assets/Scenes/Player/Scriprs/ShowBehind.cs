using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShowBehind : MonoBehaviour
{
    public SpriteRenderer tree; // ��������� ��� ���� ���������

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ���� ��'��� � ����������� �������� ������-��������,
            // ������� ��������� ������-��'���� �� �����������
            Color c = tree.color;
            c.a = 0.5f;
            tree.color = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ���� ��'��� � ����������� ����� �� �������� ������-��������,
            // ������� ��������� ����� �� �����
            Color c = tree.color;
            c.a = 1f;
            tree.color = c;
        }
    }
}
