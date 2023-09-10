using System.Collections.Generic;
using UnityEngine;

public class ShowBehind : MonoBehaviour
{
    public List<SpriteRenderer> tree; // ��������� ��� ���� ���������
    public int isSomeoneHere;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Enemy")) && !collision.isTrigger)
        {
            isSomeoneHere++;
            // ���� ���� ������� ��'���� ��������, ������� ��������� �� �����������
            if (isSomeoneHere == 1)
            {
                foreach (var item in tree)
                {
                    Color c = item.color;
                    c.a = 0.5f;
                    item.color = c;
                }
               
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Enemy")) && !collision.isTrigger)
        {
            isSomeoneHere--;
            // ���� ����� ���� ��'���� ��������, ������� ��������� ����� �� �����
            if (isSomeoneHere == 0)
            {
                foreach (var item in tree)
                {
                    Color c = item.color;
                    c.a = 1f;
                    item.color = c;
                }
            }
        }
    }
}
