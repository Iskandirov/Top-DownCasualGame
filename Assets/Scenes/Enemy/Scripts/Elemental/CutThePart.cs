using System.Collections.Generic;
using UnityEngine;

public class CutThePart : MonoBehaviour
{
    public List<CutThePart> parts;
    public GameObject vfxAttack;
    public int countParts;
    Transform parentTransform;
    private void Start()
    {
        parentTransform = transform.root;
    }
    public void CutTheParts()
    {
        countParts = parts.Count;

        for (int i = parts.Count - 1; i >= 0; i--)
        {
            CutThePart part = parts[i];
            CutThePart obj = Instantiate(part, part.transform.position, Quaternion.identity);
            obj.gameObject.tag = "Enemy";
            obj.GetComponent<Collider2D>().isTrigger = false;
            obj.transform.localScale = new Vector3(2, 2, 2);

            //Forward moveTowardsObject = obj.gameObject.AddComponent<Forward>();
            //moveTowardsObject.isChaising = true;
            //moveTowardsObject.path.maxSpeed = 30;
            //moveTowardsObject.acceleration = 1;

            //Attack attack = obj.gameObject.AddComponent<Attack>();
            //attack.damage = 2;
            //attack.attackVFX = vfxAttack;
            //attack.stepAttack = 0.3f;
            //attack.stepAttackMax = 0.3f;

            Animator anim = obj.gameObject.AddComponent<Animator>();
            anim.transform.root.GetComponent<Animator>();
            anim.runtimeAnimatorController = parentTransform.GetComponent<Animator>().runtimeAnimatorController;

            //EnemyState health = obj.GetComponent<EnemyState>();
            //health.anim = anim;
            //health.bodyAnim = anim.gameObject;
            //health.isBossPart = true;  
            
            DropItems drop = obj.gameObject.AddComponent<DropItems>();
            drop.itemPrefab = parentTransform.GetComponent<DropItems>().itemPrefab;
            drop.itemsLoaded = parentTransform.GetComponent<DropItems>().itemsLoaded;

            drop.CommonItems = parentTransform.GetComponent<DropItems>().CommonItems;
            drop.RareItems = parentTransform.GetComponent<DropItems>().RareItems;
            drop.MiphicalItems = parentTransform.GetComponent<DropItems>().MiphicalItems;
            drop.LegendaryItems = parentTransform.GetComponent<DropItems>().LegendaryItems;

            drop.spawnRare = parentTransform.GetComponent<DropItems>().spawnRare;
            drop.spawnMiphical = parentTransform.GetComponent<DropItems>().spawnMiphical;
            drop.spawnLegendary = parentTransform.GetComponent<DropItems>().spawnLegendary;
            drop.rarityType = parentTransform.GetComponent<DropItems>().rarityType;

            obj.parts.Clear();

            Destroy(part.gameObject);
            obj.countParts = countParts;
            parts.RemoveAt(i);
        }

        if (gameObject.name == "Body")
        {
            Destroy(parentTransform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

