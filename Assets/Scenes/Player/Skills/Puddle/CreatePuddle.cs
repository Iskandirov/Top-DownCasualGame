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
            StartCoroutine(WaitToAnotherObject((int)basa.countObjects - 1, basa.spawnDelay));
        }
    }
    private IEnumerator WaitToAnotherObject(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(delay);
            Instantiate(gameObject, new Vector3(player.objTransform.position.x + Random.Range(-20, 20), player.objTransform.position.y + Random.Range(-20, 20), 1.9f), Quaternion.identity);
        }
    }
    public void CreateActualPuddle()
    {
        puddle a = Instantiate(puddle, puddlePos.position, Quaternion.identity);
        a.basa = basa;
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
