using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePuddle : SkillBaseMono
{
    public puddle puddle;
    public Transform puddlePos;
    private void Start()
    {
        if (basa.stats[3].isTrigger)
        {
            basa.countObjects += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
            StartCoroutine(WaitToAnotherObject(2, basa.spawnDelay));
        }
    }
    private IEnumerator WaitToAnotherObject(int count, float delay)
    {
        for (int i = 0; i < count - 1; i++)
        {
            yield return new WaitForSeconds(delay);
            GameObject a = Instantiate(gameObject, new Vector3(player.objTransform.position.x + Random.Range(-20, 20), player.objTransform.position.y + Random.Range(-20, 20), 1.9f), Quaternion.identity);
            a.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    private void OnDestroy()
    {
        puddle a = Instantiate(puddle, puddlePos.position, Quaternion.identity);
        a.basa = basa;
        Destroy(gameObject); 
    }
}
