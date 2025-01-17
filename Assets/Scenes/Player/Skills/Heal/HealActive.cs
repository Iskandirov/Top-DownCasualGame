using System.Collections;
using System.Linq;
using UnityEngine;

public class HealActive : SkillBaseMono
{
    public float Grass;
    public float offset = 1.5f;
    Transform objTransform;
    public GameObject grassVfx;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objTransform = transform;
        Grass = player.Grass;
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.stepMax -= basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            basa.damage += basa.stats[4].value;
            basa.stats[4].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            player.isInvincible = true;
        }
        player.HealHealth(basa.damage * Grass);
        StartCoroutine(TimerSpell());
        Instantiate(grassVfx);
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        //foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        //{
        //    particle.main.startColor.color = new Color(255, 255, 255, 50);
        //}
        if (basa.stats[2].isTrigger)
        {
            player.isInvincible = false;
        }
        Destroy(gameObject);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        objTransform.position = player.objTransform.position;
    }
    public void isShowed()
    {
        GetComponent<Animator>().SetBool("IsShowed", true);
    }
}
