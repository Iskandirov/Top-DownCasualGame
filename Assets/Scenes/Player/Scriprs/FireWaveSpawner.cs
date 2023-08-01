using System.Collections;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class FireWaveSpawner : MonoBehaviour
{
    public FireWave wave;
    public float step;
    public float stepMax;
    public float damage;
    public bool isLevelThree;
    public float burnDamage;
    ElementsCoeficients FireElement;
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        FireElement = transform.root.GetComponent<ElementsCoeficients>();
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
            CreateWave();
            if (isLevelThree)
            {
                Invoke("CreateWave", 0.5f);
            }
            step = stepMax;
        }
    }
    public void CreateWave()
    {
        FireWave a = Instantiate(wave, transform.position, Quaternion.identity);
        a.damage = damage;
        a.fireElement = FireElement.Fire;
        a.burnDamage = burnDamage;
    }
}
