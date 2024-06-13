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
    public int Index = -1;
    public int poolSize = 60;
    public List<BossBullet> bulletPool;
    [Header("Attack settings")]
    public List<GroupCollector> attack_groups;
    public Animator anim;
    public void Start()
    {
        StartCoroutine(TimerCoroutineTypesAttack());
    }
    private IEnumerator TimerCoroutineTypesAttack()
    {
        while (true)
        {
            if (!attack_groups.Find(g => g.gameObject.activeSelf))
            {
                anim.SetBool("AttackEnd", true);
            }
            yield return new WaitForSeconds(interval);
            AttackTypes();
        }
    }
    public void StartRecover()
    {
        foreach (var group in attack_groups)
        {
            group.gameObject.SetActive(true);
        }

        StartCoroutine(EnemyController.instance.SlowEnemy(GetComponent<EnemyState>(), 5f, 0f));
    }
    public void EndRecover()
    {
        anim.SetBool("AttackEnd", false);
        GetComponent<EnemyState>().SetNotStunned();
        anim.SetBool("IsMoveToPlayer", true);
    }
    private void AttackTypes()
    {
        if (attack_groups.Find(g => g.gameObject.activeSelf) && !anim.GetBool("AttackEnd"))
        {
            foreach (var bullet in attack_groups.Find(g => g.gameObject.activeSelf).transform.GetComponentsInChildren<BossBullet>())
            {
                bullet.state = true;
            }
            StartCoroutine(DeactivateGroup(delay));
        }
    }
    public void MoveToPlayer()
    {
        transform.position = EnemyController.instance.GetRandomSpawnPosition(PlayerManager.instance.objTransform.position, false,25f);
        anim.SetBool("IsMoveToPlayer", false);
    }
    public IEnumerator DeactivateGroup(float delay)
    {
        yield return new WaitForSeconds(delay);
        GroupCollector group = attack_groups.Find(g => g.gameObject.activeSelf);
        foreach (var bullet in group.group)
        {
            bullet.gameObject.SetActive(true);
            bullet.state = false;
        }
        group.gameObject.SetActive(false);
    }
}