using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAxeSpawner : MonoBehaviour
{
    public MagicAxe axe;
    public float step;
    public float stepMax;
    PlayerManager player;
    public float damage;
    public float cold;
    public float radius;
    public bool isFive;
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
    private void FixedUpdate()
    {
        step -= Time.deltaTime;
    }
    void Update()
    {
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {
            
            MagicAxe a = Instantiate(axe, transform.position, Quaternion.identity);
            a.transform.localScale = new Vector2(radius * player.Steam, radius * player.Steam);
            a.damage = damage;
            //a.cold = cold * coldElement.Cold;
            step = stepMax;
        }
    }
}
