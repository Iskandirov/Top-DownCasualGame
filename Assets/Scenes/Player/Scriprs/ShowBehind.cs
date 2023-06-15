using UnityEngine;

public class ShowBehind : MonoBehaviour
{
    public SpriteRenderer tree; // ��������� ��� ���� ���������
    public int isSomeoneHere;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSomeoneHere < 0)
        {
            isSomeoneHere = 0;
        }
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            // ���� ��'��� � ����������� �������� ������-��������,
            // ������� ��������� ������-��'���� �� �����������
            Color c = tree.color;
            c.a = 0.5f;
            tree.color = c;
            isSomeoneHere += 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isSomeoneHere -= 1;
        if (collision.CompareTag("Player") && isSomeoneHere <= 0 || collision.CompareTag("Enemy") && isSomeoneHere == 0)
        {
            // ���� ��'��� � ����������� ����� �� �������� ������-��������,
            // ������� ��������� ����� �� �����
            Color c = tree.color;
            c.a = 1f;
            tree.color = c;
        }
        
    }
}
