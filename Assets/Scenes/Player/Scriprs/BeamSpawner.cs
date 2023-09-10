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
    ElementsCoeficients SteamElement;
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        SteamElement = transform.root.GetComponent<ElementsCoeficients>();
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
    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {
            Beam a = Instantiate(beam, new Vector2(transform.position.x + 5, transform.position.y), Quaternion.identity);
            a.damage = damage;
            a.lifeTime = lifeTime;
            a.Steam = SteamElement.Steam;
            if (isTwo)
            {
                Beam b = Instantiate(beam, new Vector2(transform.position.x, transform.position.y - 5), Quaternion.Euler(0, 0, 90));
                b.damage = damage;
                b.img.transform.rotation = Quaternion.AngleAxis(-90f, Vector3.forward);
                b.lifeTime = lifeTime;
                b.Steam = SteamElement.Steam;
                Beam c = Instantiate(beam, new Vector2(transform.position.x, transform.position.y + 5), Quaternion.Euler(0, 0, -90));
                c.damage = damage;
                c.img.transform.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
                c.lifeTime = lifeTime;
                c.Steam = SteamElement.Steam;
            }
            step = stepMax;
        }
    }
}
