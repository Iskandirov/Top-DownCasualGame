using System.Collections;
using UnityEngine;

public class HealSpawner : MonoBehaviour
{
    public HealActive healObj;
    public float step;
    public float stepMax;
    public float heal;
    public bool isLevelTwo;
    int buttonActivateSkill;
    PlayerManager player;
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

            HealActive a = Instantiate(healObj, transform.position, Quaternion.identity);
            a.heal = heal;
            a.isLevelTwo = isLevelTwo;
            a.Grass = player.Grass;
            step = stepMax;
        }
    }
}
