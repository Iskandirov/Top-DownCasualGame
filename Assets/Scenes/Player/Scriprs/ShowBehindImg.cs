using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBehindImg : MonoBehaviour
{
    public List<Image> imageToShow;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            // якщо об'Їкт з коллайдером перетинаЇ тригер-колайдер,
            // зм≥нюЇмо прозор≥сть тригер-об'Їкту на нап≥впрозору
            foreach (var img in imageToShow)
            {
                Color c = img.color;
                c.a = 0.1f;
                img.color = c;
            }
        }
    } 
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            // якщо об'Їкт з коллайдером перетинаЇ тригер-колайдер,
            // зм≥нюЇмо прозор≥сть тригер-об'Їкту на нап≥впрозору
            foreach (var img in imageToShow)
            {
                Color c = img.color;
                c.a = 0.1f;
                img.color = c;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            // якщо об'Їкт з коллайдером б≥льше не перетинаЇ тригер-колайдер,
            // зм≥нюЇмо прозор≥сть назад на повну
            foreach (var img in imageToShow)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
                Debug.Log(1);
            }
        }
    }
}
