using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Attack_Spawn_Tornadics : MonoBehaviour
{
    public Tornadic_Move tornadic;
    public float tornadicCount = 10;
    public float interval = 10;
    Transform objTransofrm;
    // Start is called before the first frame update
    void Start()
    {
        objTransofrm = transform;
        StartCoroutine(SpawnTornadics());
    }

   public IEnumerator SpawnTornadics()
    {
        while(true)
        {
            yield return new WaitForSeconds(interval);
            Spawn();
        }
    }
    public void Spawn()
    {
        for (int i = 0; i <= tornadicCount; i++)
        {
            Tornadic_Move a = Instantiate(tornadic, objTransofrm.position, Quaternion.identity);
            a.mainDirection = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        }
    }
}
