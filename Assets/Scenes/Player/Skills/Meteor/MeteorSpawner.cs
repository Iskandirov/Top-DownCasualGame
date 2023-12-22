using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public Meteor meteor;
    public float step;
    public float stepMax;
    public float damage;
    public bool isThree;
    public bool isFour;
    public bool isFive;
    PlayerManager player;
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        step = gameObject.GetComponent<CDSkillObject>().CD;
        StartCoroutine(SetBumberToSkill());
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
        buttonActivateSkill = gameObject.GetComponent<CDSkillObject>().num + 1;
        keyCode = (KeyCode)((int)KeyCode.Alpha0 + buttonActivateSkill);
    }

    void FixedUpdate()
    {
        step -= Time.fixedDeltaTime;
    }
    private void Update()
    {
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {
            Meteor a = Instantiate(meteor, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
            a.damage = damage;
            a.isFour = isFour;
            a.fireDirt = player.Dirt + player.Fire - 1;
            if (isThree)
            {
                Meteor b = Instantiate(meteor, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
                b.damage = damage;
                b.isFour = isFour;
                b.fireDirt = player.Dirt + player.Fire - 1;
                Meteor c = Instantiate(meteor, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
                c.damage = damage;
                c.isFour = isFour;
                c.fireDirt = player.Dirt + player.Fire - 1;
                if (isFive)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Meteor x = Instantiate(meteor, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
                        x.damage = damage;
                        x.isFour = isFour;
                        x.fireDirt = player.Dirt + player.Fire - 1;
                    }
                }
            }
            step = stepMax;
        }
    }
}
