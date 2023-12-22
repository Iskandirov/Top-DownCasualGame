using System.Collections;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{

    public Tower tower;
    public float step;
    public float stepMax;
    PlayerManager player;
    public float lifeTime;
    public bool isThree;
    public float attackSpeed;
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
    // Update is called once per frame
    void Update()
    {
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {
            Tower a = Instantiate(tower, new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10)), Quaternion.identity);
            a.lifeTime = lifeTime;
            a.waterElement = player.Water;
            a.isThree = isThree;
            a.spawnTickMax = attackSpeed;
            a.fireElement = player.Fire;
            a.isFive = isFive;
            step = stepMax;
        }
    }
}
