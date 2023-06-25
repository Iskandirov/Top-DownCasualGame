using System.Collections;
using UnityEngine;

public class FireWaveSpawner : MonoBehaviour
{
    public FireWave wave;
    public float step;
    public float stepMax;
    public float damage;
    public bool isLevelThree;
    public float burnDamage;
    ElementsCoeficients FireElement;
    // Start is called before the first frame update
    void Start()
    {
        FireElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
    }

    // Update is called once per frame
    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
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
