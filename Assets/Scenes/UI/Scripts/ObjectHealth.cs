using FSMC.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    [SerializeField] float health = 5;
    [SerializeField] float healthMax = 5;
    [SerializeField] float explosionDamage = 5;
    [SerializeField] List<GameObject> SpawnableObjects;
    [SerializeField] Animator explodeAnim;
    [SerializeField] List<FSMC_Executer> enemiesInExplosionArea;
    [SerializeField] PlayerManager playerInExplosionArea;
    [SerializeField] GameObject explodeVFX;
    public void TakeDamage()
    {
        health -= 1;
        explodeAnim.SetTrigger("Hit");
        if (health <= 0)
        {
            if (healthMax == 1)
            {
                ExplodeAnimActivate();
            }
            else
            {
                Debug.Log("Explode!");
                //CreateLoot();
                explodeAnim.SetBool("OpenChest", true);
                health = healthMax;
            }
        }
    }
    void CreateLoot()
    {
        GameObject a = Instantiate(SpawnableObjects[Random.Range(0, SpawnableObjects.Count)], transform.position, Quaternion.identity);
        if (a.GetComponent<FSMC_Executer>() != null)
        {
            FindObjectOfType<EnemySpawner>().children.Add(a.GetComponent<FSMC_Executer>());
        }
        gameObject.SetActive(false);
        explodeAnim.SetBool("OpenChest", false);
    }
    void ExplodeAnimActivate()
    {
        explodeAnim.SetBool("isGoingExplode", true);
    }
    public void Explode()
    {
        Instantiate(explodeVFX, transform.position, Quaternion.identity);
        if (enemiesInExplosionArea != null)
        {
            foreach (var enemy in enemiesInExplosionArea)
            {
                enemy.TakeDamage(explosionDamage,1);
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
            enemiesInExplosionArea.Add(collision.GetComponent<FSMC_Executer>());
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
            enemiesInExplosionArea.Remove(collision.GetComponent<FSMC_Executer>());
        }
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            playerInExplosionArea = null;
        }
    }
}
