using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Attack_Spawn_Tornadics : MonoBehaviour
{
    public Tornadic_Move tornadic;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnTornadics());
    }

   public IEnumerator SpawnTornadics()
    {
        while(true)
        {
            yield return new WaitForSeconds(10f);
            Spawn();
        }
    }
    public void Spawn()
    {
        for (int i = 0; i <= 10; i++)
        {
            Tornadic_Move a = Instantiate(tornadic, transform.position, Quaternion.identity);
            a.mainDirection = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        }
    }
}
