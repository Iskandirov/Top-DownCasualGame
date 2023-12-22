using System.Collections;
using UnityEngine;

public class BeamSpawner : MonoBehaviour
{
    public Beam beam;
    public float step;
    public float stepMax;
    public bool isTwo;
    public float damage;
    public float lifeTime;
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
            CreateBeam(0, 5, 0);
            if (isTwo)
            {
                CreateBeam(-90, 0, -5);
                CreateBeam(90, 0, 5);
            }
            step = stepMax;
        }
    }
    private void CreateBeam(float angle,float x,float y)
    {
        Beam a = Instantiate(beam, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity);
        a.damage = damage;
        a.addToAndle = angle;
        a.lifeTime = lifeTime;
        a.Steam = player.Steam;
    }
}
