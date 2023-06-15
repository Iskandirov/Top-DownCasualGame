using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultDestroy : MonoBehaviour
{
    public float lifeTime;
    void Start()
    {
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);

    }
}
