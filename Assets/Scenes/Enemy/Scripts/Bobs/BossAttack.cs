using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public BossBullet bulletPrefab;

    public float interval = 2f;
    public float defaultInterval = 2f;
    public float randomeInterval = 2f;
    public float roundInterval = 2f;

    private float angle = 0f;
    public float speed = 5f;
    public float aceleration = 1f;

    public List<bool> attackBools;
    public float attackTypeInterval = 5f;
    public int Index = -1;
    public int poolSize = 60;
    public List<BossBullet> bulletPool;
    public void Start()
    {
        if (GetComponent<Forward>().isTutorial)
        {
            bulletPrefab.damage = 0;
        }
        StartCoroutine(ToggleBoolsCoroutine());
        StartCoroutine(TimerCoroutineTypesAttack());
        InitializeObjectPool();
    }
    void InitializeObjectPool()
    {
        bulletPool = new List<BossBullet>();
        for (int i = 0; i < poolSize; i++)
        {
            BossBullet obj = Instantiate(bulletPrefab);
            obj.gameObject.SetActive(false);
            bulletPool.Add(obj);
        }
    }
    public BossBullet GetFromPool()
    {
        foreach (BossBullet obj in bulletPool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }
        BossBullet gameObject = Instantiate(bulletPrefab);
        bulletPool.Add(gameObject);
        return gameObject;
    }
    private IEnumerator ToggleBoolsCoroutine()
    {
        while (true)
        {
            if (Index < attackBools.Count - 1)
            {
                Index++;
            }
            else
            {
                Index = 0;
            }
            attackBools[Index] = true;

            // Виключаємо всі інші були
            for (int i = 0; i < attackBools.Count; i++)
            {
                if (i != Index)
                {
                    attackBools[i] = false;
                }
            }

            // Зачекайте декілька секунд перед наступною зміною
            yield return new WaitForSeconds(attackTypeInterval);
        }
    }
    private IEnumerator TimerCoroutineTypesAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            aceleration = 1;
            AttackTypes();
        }
    } 
    private void AttackTypes()
    {
        if (attackBools[0] == true)
        {
            float angleSinCos = 45f;
            Vector2 direction ;
            // Випустити об'єкти з відповідними напрямками
            for (int i = 0; i < 4;i++)
            {
                direction = new Vector2(Mathf.Cos((angle + angleSinCos) * Mathf.Deg2Rad), Mathf.Sin((angle + angleSinCos) * Mathf.Deg2Rad));
                BossBullet bullet = GetFromPool();
                bullet.startPosition = transform.position;
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
                bullet.GetComponent<Rigidbody2D>().velocity = speed * direction;
                bullet.lifeTime = 3f;
                bullet.Invoke("DestroyBullet", bullet.lifeTime);
                angleSinCos += 90;
            }
           
            ;
            angle += 45f;
            interval = defaultInterval;
        }
        else if (attackBools[1] == true)
        {
            for (int i = 0; i < 10; i++)
            {
                BossBullet bullet = GetFromPool();
                bullet.transform.position = transform.position;
                bullet.startPosition = transform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
                bullet.GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle) * Mathf.Deg2Rad), Mathf.Sin((angle) * Mathf.Deg2Rad));
                bullet.enemyBody = transform;
                bullet.isAround = true;
                bullet.speed = 1f / (1f + i * 0.15f);
                bullet.lifeTime = 3f;
                bullet.Invoke("DestroyBullet", bullet.lifeTime);
            }
            interval = roundInterval;
        }
        else if (attackBools[2] == true)
        {
            Vector2 direction;
            for (int i = 0; i < 30; i++)
            {
                direction = new Vector2(Mathf.Cos((Random.Range(5, 360)) * Mathf.Deg2Rad), Mathf.Sin((Random.Range(5, 360)) * Mathf.Deg2Rad));
                BossBullet bullet = GetFromPool();
                bullet.startPosition = transform.position;
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                bullet.GetComponent<Rigidbody2D>().velocity = speed * direction;
                bullet.lifeTime = 5f;
                bullet.Invoke("DestroyBullet", bullet.lifeTime);
            }
            interval = randomeInterval;
        }
        
    }
    private void Update()
    {
        aceleration += 0.1f;
    }
}