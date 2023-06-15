using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impuls : MonoBehaviour
{
    public float lifeTime;
    public float powerGrow;
    public bool isFour;
    public bool isFive;
    GameObject player;
    public DestroyBarrier barrier;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Move>().gameObject;
        StartCoroutine(TimerSpell());

    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        if (isFour)
        {
            DestroyBarrier a = Instantiate(barrier, transform.position, Quaternion.identity);
            if (isFive)
            {
                a.isFiveLevel = isFive;

            }
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * powerGrow,
            transform.localScale.y + Time.deltaTime * powerGrow, transform.localScale.z + Time.deltaTime * powerGrow);
    }
}
