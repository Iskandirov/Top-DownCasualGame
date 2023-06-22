using UnityEngine;

public class BeamSpawner : MonoBehaviour
{
    public Beam beam;
    public float step;
    public float stepMax;
    public bool isTwo;
    public float damage;
    public float lifeTime;
    ElementsCoeficients BloodElement;
    // Start is called before the first frame update
    void Start()
    {
        BloodElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
    }

    // Update is called once per frame
    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
        {
            Beam a = Instantiate(beam, new Vector2(transform.position.x + 5, transform.position.y), Quaternion.identity);
            a.damage = damage;
            a.lifeTime = lifeTime;
            a.blood = BloodElement.Blood;
            if (isTwo)
            {
                Beam b = Instantiate(beam, new Vector2(transform.position.x, transform.position.y - 5), Quaternion.Euler(0, 0, 90));
                b.damage = damage;
                b.lifeTime = lifeTime;
                b.blood = BloodElement.Blood;
                Beam c = Instantiate(beam, new Vector2(transform.position.x, transform.position.y + 5), Quaternion.Euler(0, 0, -90));
                c.damage = damage;
                c.lifeTime = lifeTime;
                c.blood = BloodElement.Blood;
            }
            step = stepMax;
        }
    }
}
