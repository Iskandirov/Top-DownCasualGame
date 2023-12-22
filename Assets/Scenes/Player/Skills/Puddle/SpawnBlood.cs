using System.Collections;
using UnityEngine;

public class SpawnBlood : MonoBehaviour
{
    public puddle puddle;
    public float step;
    public float stepMax;
    public float radius;
    public float damage;
    public float numOfChair;
    public float damageTickMax;
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
            for (int i = 0; i < numOfChair; i++)
            {
                puddle a = Instantiate(puddle, new Vector3(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20), 1.9f), Quaternion.identity);
                a.damage = damage * player.Water;
                a.radius += radius * player.Dirt;
                a.gameObject.transform.localScale = new Vector2(a.gameObject.transform.localScale.x + radius, a.gameObject.transform.localScale.y + radius);
                a.damageTickMax = damageTickMax;
            }
            step = stepMax;
        }
    }
}
