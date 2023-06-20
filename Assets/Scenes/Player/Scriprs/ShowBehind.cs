using UnityEngine;

public class ShowBehind : MonoBehaviour
{
    public SpriteRenderer tree; // ��������� ��� ���� ���������
    public int isSomeoneHere;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            isSomeoneHere++;
            // ���� ���� ������� ��'���� ��������, ������� ��������� �� �����������
            if (isSomeoneHere == 1)
            {
                Color c = tree.color;
                c.a = 0.5f;
                tree.color = c;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            isSomeoneHere--;
            // ���� ����� ���� ��'���� ��������, ������� ��������� ����� �� �����
            if (isSomeoneHere == 0)
            {
                Color c = tree.color;
                c.a = 1f;
                tree.color = c;
            }
        }
    }
}
