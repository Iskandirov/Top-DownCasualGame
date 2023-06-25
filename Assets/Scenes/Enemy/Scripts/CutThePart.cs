using System.Collections.Generic;
using UnityEngine;

public class CutThePart : MonoBehaviour
{
    public List<CutThePart> parts;
    public GameObject vfxAttack;

    public void CutTheParts()
    {
        GameObject player = GameObject.FindWithTag("Player");

        for (int i = parts.Count - 1; i >= 0; i--)
        {
            CutThePart part = parts[i];
            CutThePart obj = Instantiate(part, part.transform.position, Quaternion.identity);
            obj.gameObject.tag = "Enemy";
            obj.GetComponent<Collider2D>().isTrigger = false;
            obj.transform.localScale = new Vector3(2, 2, 2);

            Forward moveTowardsObject = obj.gameObject.AddComponent<Forward>();
            moveTowardsObject.isChaising = true;
            moveTowardsObject.speedMax = 30;
            moveTowardsObject.acceleration = 1;
            moveTowardsObject.player = player;

            Attack attack = obj.gameObject.AddComponent<Attack>();
            attack.damage = 2;
            attack.attackVFX = vfxAttack;
            attack.stepAttack = 0.3f;
            attack.stepAttackMax = 0.3f;

            Animator anim = obj.gameObject.AddComponent<Animator>();
            anim.transform.root.GetComponent<Animator>();
            anim.runtimeAnimatorController = transform.root.GetComponent<Animator>().runtimeAnimatorController;

            obj.GetComponent<CutThePart>().parts.Clear();

            Destroy(part.gameObject);
            parts.RemoveAt(i);
        }

        if (gameObject.name == "Body")
        {
            Transform parentTransform = transform.root;
            Destroy(parentTransform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

