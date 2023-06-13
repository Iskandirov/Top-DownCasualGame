using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss_Destroy : MonoBehaviour
{

    public List<string> parts;
    public CutThePart partOff;

    public GameObject expiriancePoint;
    public GameObject VFX_Deadarticle;
    public float healthMax;

    public void Start()
    {
        parts = new List<string>() { "Head", "Body", "Left_Shoulder", "Left_Arm", "Left_Fist", "Right_Shoulder", "Right_Arm", "Right_Fist" };
    }
    public void DestroyEnd()
    {
        GameObject a = Instantiate(expiriancePoint, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1.9f), Quaternion.identity);
        a.GetComponent<EXP>().expBuff = healthMax / 5;

        gameObject.GetComponentInParent<DropItems>().OnDestroyBoss();
        Instantiate(VFX_Deadarticle, gameObject.transform.position, Quaternion.identity);
        partOff.CutTheParts();
        gameObject.GetComponent<Animator>().SetBool("Body", true);
        ChangeChildTags(gameObject.transform, "Enemy",5f);
        Destroy(gameObject);
    }
    public void DestroyStart()
    {
        gameObject.GetComponent<Animator>().SetBool("Body", true);
        ChangeChildTags(gameObject.transform, "Untagged",0f);
    }
    public void ChangeChildTags(Transform parent, string newTag, float speed_move)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.tag = newTag;
            ChangeChildTags(child, newTag, speed_move);
        }
        gameObject.GetComponent<Forward>().speed = speed_move;
    }
    public void GetParts(CutThePart part ,float maxHealth)
    {
        partOff =  part;
        healthMax = maxHealth;
    }
}
