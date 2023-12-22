using System.Collections;
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    public Shield shield;
    public float step;
    public float stepMax;
    public float ShieldHP;
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
    // Update is called once per frame
    void FixedUpdate()
    {
        step -= Time.fixedDeltaTime;
       
    }
    private void Update()
    {
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {
            Shield a = Instantiate(shield, transform.position, Quaternion.identity);
            a.healthShield = ShieldHP;
            a.isThreeLevel = isThree;
            a.isFourLevel = isFour;
            a.isFiveLevel = isFive;
            a.dirtElement = player.Dirt;
            step = stepMax;
        }
    }
}
