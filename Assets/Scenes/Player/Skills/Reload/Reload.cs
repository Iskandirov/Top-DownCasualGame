using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reload : MonoBehaviour
{
    public float lifeTime;
    public int count;
    public List<CDSkillObject> spells;
    // Start is called before the first frame update
    void Start()
    {
        int i = Random.Range(0, spells.Count);
        if (spells != null)
        {
            for (int y = 0; y < count; y++)
            {
                Debug.Log(spells[i]);
                spells[i].valueFieldStep.SetValue(spells[i].monoStep, 0.01f + y / 100);
            }
            GameManager.Instance.FindStatName("skillsReloaded", 1);
        }
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
