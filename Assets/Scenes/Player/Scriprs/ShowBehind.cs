using System.Collections.Generic;
using UnityEngine;

public class ShowBehind : MonoBehaviour
{
    public List<SpriteRenderer> tree; // компонент дл€ зм≥ни прозорост≥
    public int isSomeoneHere;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Enemy")) && !collision.isTrigger)
        {
            isSomeoneHere++;
            // якщо немаЇ жодного об'Їкту всередин≥, зм≥нюЇмо прозор≥сть на нап≥впрозору
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
            // якщо б≥льше немаЇ об'Їкт≥в всередин≥, зм≥нюЇмо прозор≥сть назад на повну
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
