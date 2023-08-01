using System.Collections;
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
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        WindElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
        StartCoroutine(SetBumberToSkill());
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
        buttonActivateSkill = gameObject.GetComponent<CDSkillObject>().num + 1;
        keyCode = (KeyCode)((int)KeyCode.Alpha0 + buttonActivateSkill);
    }
    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0 && Input.GetKeyDown(keyCode))
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
