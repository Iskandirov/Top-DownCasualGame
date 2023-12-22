using System.Collections;
using UnityEngine;

public class IllusionSpawner : MonoBehaviour
{
    public Illusion illusion;
    public float step;
    public float stepMax;
    public float lifeTime;
    public bool isTwo;
    public bool isFour;
    public bool isFive;
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
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
            CreateClone(-10, 10, 2, -10, 0);
            if (isTwo)
            {
                CreateClone(-5, -5, 10, 0, 112);
                if (isFour)
                {
                    CreateClone(10, 0, -10, 5, 240);
                }
            }
            step = stepMax;
        }
    }
    private void CreateClone(float x, float y, float xZzap, float yZzap,float angle)
    {
        Illusion a = Instantiate(illusion, transform.position, Quaternion.identity);
        a.x = x;
        a.y = y;
        a.xZzap = xZzap;
        a.yZzap = yZzap;
        a.angle = angle;
        a.lifeTime = lifeTime;
        a.isFive = isFive;
    }
}
