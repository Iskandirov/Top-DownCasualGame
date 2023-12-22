using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impuls : MonoBehaviour
{
    public float lifeTime;
    public float powerGrow;
    public bool isFour;
    public bool isFive;
    public DestroyBarrier barrier;
    public float wind;
    public float grass;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerSpell());

    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        if (isFour)
        {
            GameManager.Instance.FindStatName("barierSpawned", 1);
            DestroyBarrier a = Instantiate(barrier, transform.position, Quaternion.identity);
            a.Grass = grass;
            if (isFive)
            {
                a.isFiveLevel = isFive;
            }
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = PlayerManager.instance.transform.position;
        transform.localScale = new Vector3(transform.localScale.x + Time.fixedDeltaTime * powerGrow * wind,
            transform.localScale.y + Time.fixedDeltaTime * powerGrow * wind, transform.localScale.z + Time.fixedDeltaTime * powerGrow * wind);
    }
}
