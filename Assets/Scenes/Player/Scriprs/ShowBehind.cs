using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShowBehind : MonoBehaviour
{
    public SpriteRenderer tree; // компонент дл€ зм≥ни прозорост≥

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // якщо об'Їкт з коллайдером перетинаЇ тригер-колайдер,
            // зм≥нюЇмо прозор≥сть тригер-об'Їкту на нап≥впрозору
            Color c = tree.color;
            c.a = 0.5f;
            tree.color = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // якщо об'Їкт з коллайдером б≥льше не перетинаЇ тригер-колайдер,
            // зм≥нюЇмо прозор≥сть назад на повну
            Color c = tree.color;
            c.a = 1f;
            tree.color = c;
        }
    }
}
