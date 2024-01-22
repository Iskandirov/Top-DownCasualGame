using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impuls : SkillBaseMono
{
    public float powerGrow;
    public DestroyBarrier barrier;
    public float wind;
    public float grass;
    

    // Start is called before the first frame update
    void Start()
    {
        wind = PlayerManager.instance.Wind;
        grass = PlayerManager.instance.Grass;
        if (basa.stats[1].isTrigger)
        {
            basa.radius += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.stepMax -= basa.stats[2].value;
            basa.skill.skillCD -= StabilizateCurrentReload(basa.skill.skillCD, basa.stats[2].value);
            basa.stats[2].isTrigger = false;
        }
        StartCoroutine(TimerSpell());
    }
    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(basa.lifeTime);

        if (basa.stats[3].isTrigger)
        {
            GameManager.Instance.FindStatName("barierSpawned", 1);
            DestroyBarrier a = Instantiate(barrier, transform.position, Quaternion.identity);
            a.Grass = grass;
            if (basa.stats[4].isTrigger)
            {
                a.isFiveLevel = basa.stats[4].isTrigger;
            }
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = PlayerManager.instance.transform.position;
        transform.localScale = new Vector3((transform.localScale.x + basa.radius) + Time.fixedDeltaTime * powerGrow * wind,
            (transform.localScale.y + basa.radius) + Time.fixedDeltaTime * powerGrow * wind, (transform.localScale.z + basa.radius) + Time.fixedDeltaTime * powerGrow * wind);
    }
}
