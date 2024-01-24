using System.Collections.Generic;
using UnityEngine;

public class ShowBehind : MonoBehaviour
{
    public List<SpriteRenderer> tree; // компонент дл€ зм≥ни прозорост≥
    public int isSomeoneHere;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HideAndShow(collision, 1, isSomeoneHere + 1, 0.5f);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        HideAndShow(collision, 0, isSomeoneHere - 1, 1f);
    }
    void HideAndShow(Collider2D collision, int someOneHere, int operation, float opacity)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Enemy")) && !collision.isTrigger)
        {
            isSomeoneHere = operation;
            // якщо б≥льше немаЇ об'Їкт≥в всередин≥, зм≥нюЇмо прозор≥сть назад на повну
            if (isSomeoneHere == someOneHere)
            {
                foreach (var item in tree)
                {
                    Color c = item.color;
                    c.a = opacity;
                    item.color = c;
                }
            }
        }
    }
}
