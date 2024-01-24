using System.Collections.Generic;
using UnityEngine;

public class Boss_Destroy : MonoBehaviour
{
    public List<string> parts;
    public CutThePart partOff;

    public EXP expiriancePoint;
    public GameObject VFX_Deadarticle;
    public float healthMax;

    Animator objAnim;
    Forward objMove;
    Transform objTransform;
    public void Start()
    {
        //parts = new List<string>() { "Head", "Body", "Left_Shoulder", "Left_Arm", "Left_Fist", "Right_Shoulder", "Right_Arm", "Right_Fist" };

        objAnim = GetComponent<Animator>();
        objMove = GetComponent<Forward>();
        objTransform = transform;
    }
    public void DestroyEnd()
    {
        partOff.CutTheParts();
        objAnim.SetBool("Body", true);
        ChangeChildTags(objTransform, "Enemy",5f);
        Destroy(gameObject);
    }
    public void DestroyStart()
    {
        objAnim.SetBool("Body", true);
        ChangeChildTags(objTransform, "Untagged",0f);
    }
    public void ChangeChildTags(Transform parent, string newTag, float speed_move)
    {
        foreach (Transform child in parent)
        {
            child.tag = newTag;
            ChangeChildTags(child, newTag, speed_move);
        }
        objMove.path.maxSpeed = speed_move;
    }
    public void GetParts(CutThePart part ,float maxHealth)
    {
        partOff =  part;
        healthMax = maxHealth;
    }
}
