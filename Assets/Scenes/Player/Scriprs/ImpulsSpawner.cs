using UnityEngine;

public class ImpulsSpawner : MonoBehaviour
{
    public Impuls impuls;
    public float step;
    public float stepMax;
    public float powerGrow;
    public bool isFour;
    public bool isFive;
    ElementsCoeficients WindElement;
    // Start is called before the first frame update
    void Start()
    {
        WindElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
    }

    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
        {

            Impuls a = Instantiate(impuls, transform.position, Quaternion.identity);
            a.powerGrow = powerGrow;
            a.isFour = isFour;
            a.isFive = isFive;
            a.wind = WindElement.Wind;
            a.grass = WindElement.Grass;
            step = stepMax;
        }
    }
}
