using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

            MoveTowardsObject moveTowardsObject = obj.gameObject.AddComponent<MoveTowardsObject>();
            moveTowardsObject.maxSpeed = 30;
            moveTowardsObject.acceleration = 1;
            moveTowardsObject.playerTransform = player.transform;

            Attack attack = obj.gameObject.AddComponent<Attack>();
            attack.damage = 2;
            attack.attackVFX = vfxAttack;
            attack.stepAttack = 0.3f;
            attack.stepAttackMax = 0.3f;

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

