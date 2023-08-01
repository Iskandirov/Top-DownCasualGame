using UnityEngine;
using UnityEngine.UI;

public class CDSkills : MonoBehaviour
{
    public float skillCD;
    public float skillCDMax;
    public Image spriteCD;
    public int number;
    public GameObject text;
    public int abilityId;

    // Update is called once per frame
    void Update()
    {
        spriteCD.fillAmount = skillCD / skillCDMax;
    }
}
