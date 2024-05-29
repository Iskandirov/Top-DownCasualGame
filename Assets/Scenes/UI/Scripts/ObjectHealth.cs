using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    [SerializeField] float health = 5;
    [SerializeField] float healthMax = 5;
    [SerializeField] float explosionDamage = 5;
    [SerializeField] List<GameObject> SpawnableObjects;
    [SerializeField] Animator explodeAnim;
    [SerializeField] List<EnemyState> enemiesInExplosionArea;
    [SerializeField] PlayerManager playerInExplosionArea;
    public void TakeDamage()
    {
        health -= 1;
        if (health <= 0)
        {
            if (healthMax == 1)
            {
                ExplodeAnimActivate();
            }
            else
            {
                CreateLoot();
                gameObject.SetActive(false);
                health = healthMax;
            }
        }
    }
    void CreateLoot()
    {
        GameObject a = Instantiate(SpawnableObjects[Random.Range(0, SpawnableObjects.Count)], transform.position, Quaternion.identity);
        if (a.GetComponent<EnemyState>() != null)
        {
            FindObjectOfType<EnemyController>().children.Add(a.GetComponent<EnemyState>());
        }
    }
    void ExplodeAnimActivate()
    {
        explodeAnim.SetBool("isGoingExplode", true);
    }
    public void Explode()
    {
        if (enemiesInExplosionArea != null)
        {
            EnemyController controller = EnemyController.instance;
            foreach (var enemy in enemiesInExplosionArea)
            {
                controller.TakeDamage(enemy, explosionDamage);
            }
        }
        if (playerInExplosionArea != null)
        {
            playerInExplosionArea.TakeDamage(explosionDamage);
        }
        enemiesInExplosionArea.Clear();
        playerInExplosionArea = null;
        explodeAnim.SetBool("isGoingExplode", false);
        gameObject.SetActive(false);
        health = healthMax;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemiesInExplosionArea.Add(collision.GetComponent<EnemyState>());
        } 
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            playerInExplosionArea = collision.GetComponent<PlayerManager>();
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemiesInExplosionArea.Remove(collision.GetComponent<EnemyState>());
        }
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            playerInExplosionArea = null;
        }
    }
}
