using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHat : MonoBehaviour
{
    public List<Sprite> fireSprites;
    public SpriteRenderer currentSprite;
    public float delay = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimateFire());
    }

    public IEnumerator AnimateFire()
    {
        int i = 0;
        while (true)
        {
            currentSprite.sprite = fireSprites[i];
            i++;
            if (i >= fireSprites.Count)
            {
                i = 0;
            }

            yield return new WaitForSeconds(delay); // Затримка між кадрами
        }
    }
}
