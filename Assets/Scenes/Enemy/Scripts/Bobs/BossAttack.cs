using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public BossBullet bulletPrefab;

    public float delay = 2f;
    public float interval = 2f;
    public float damage = 20;

    public List<bool> attackBools;
    public int index = 0;
    public int poolSize = 60;
    public List<BossBullet> bulletPool;
    [Header("Attack settings")]
    public List<GroupCollector> attackGroup;
    public List<string> attackAnimBoolName;
    public Animator anim;
    public void Start()
    {
        StartCoroutine(TimerCoroutineTypesAttack());
    }
    private IEnumerator TimerCoroutineTypesAttack()
    {
        while (true)
        {
           
            yield return new WaitForSeconds(interval);
            AttackTypes();
        }
    }
    public void StartRecover()
    {
        foreach (var group in attackGroup)
        {
            group.gameObject.SetActive(true);
        }
    }
    public void EndRecover()
    {
        anim.SetBool("AttackEnd", false);
       // GetComponent<EnemyState>().SetNotStunned();
        anim.SetBool("IsMoveToPlayer", true);
    }
    private void AttackTypes()
    {
        if (index < attackAnimBoolName.Count && !anim.GetBool("AttackEnd"))
        {
            anim.SetBool(attackAnimBoolName[index], true);
            index++;
        }
        else
        {
            index = 0;
        }
    }
    public void StartAttack()
    {
        foreach (var bullet in attackGroup[index - 1].group)
        {
            bullet.gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
        }
    }
    public void DeactivateAttack(string BoolName)
    {
        foreach(var bullet in attackGroup[index - 1].group)
        {
            bullet.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        anim.SetBool(BoolName, false);
        attackGroup[index - 1].gameObject.SetActive(false);
        if (index == attackAnimBoolName.Count)
        {
            anim.SetBool("AttackEnd", true);
        }
    }
    public void MoveToPlayer()
    {
        transform.position = GetRandomPosition(PlayerManager.instance.objTransform.position,25f);
        anim.SetBool("IsMoveToPlayer", false);
    }
    public Vector3 GetRandomPosition(Vector3 pos,  float radius)
    {
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * radius;
        Vector3 spawnPosition = new Vector3(pos.x + spawnOffset.x, pos.y + spawnOffset.y, 0);

        return spawnPosition;
    }
}