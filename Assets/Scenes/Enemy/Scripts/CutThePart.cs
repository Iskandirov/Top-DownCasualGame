using System.Collections.Generic;
using UnityEngine;

public class CutThePart : MonoBehaviour
{
    public List<CutThePart> parts;
    public GameObject vfxAttack;
    public int countParts;
    public void CutTheParts()
    {
        countParts = parts.Count;
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

            HealthPoint health = obj.gameObject.GetComponent<HealthPoint>();
            health.anim = anim;
            health.bodyAnim = anim.gameObject;
            health.isBossPart = true;  
            
            DropItems drop = obj.gameObject.AddComponent<DropItems>();
            drop.itemPrefab = transform.root.GetComponent<DropItems>().itemPrefab;
            drop.items = transform.root.GetComponent<DropItems>().items;
            drop.itemsLoaded = transform.root.GetComponent<DropItems>().itemsLoaded;

            drop.CommonItems = transform.root.GetComponent<DropItems>().CommonItems;
            drop.RareItems = transform.root.GetComponent<DropItems>().RareItems;
            drop.MiphicalItems = transform.root.GetComponent<DropItems>().MiphicalItems;
            drop.LegendaryItems = transform.root.GetComponent<DropItems>().LegendaryItems;

            drop.spawnRare = transform.root.GetComponent<DropItems>().spawnRare;
            drop.spawnMiphical = transform.root.GetComponent<DropItems>().spawnMiphical;
            drop.spawnLegendary = transform.root.GetComponent<DropItems>().spawnLegendary;
            drop.rarityType = transform.root.GetComponent<DropItems>().rarityType;

            obj.GetComponent<CutThePart>().parts.Clear();

            Destroy(part.gameObject);
            obj.countParts = countParts;
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

