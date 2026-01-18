using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerTrail : MonoBehaviour
{
    public List<status> Elements;
    public float damage = 1f;
    public float damageInterval = 0.3f;
    public SkillBase basa;

    private Dictionary<GameObject, Coroutine> activeDamages = new Dictionary<GameObject, Coroutine>();

    private IEnumerator DamageOverTime(FSMC_Executer enemy)
    {
        while (enemy != null && enemy.gameObject.activeInHierarchy)
        {
            // Ефект
            var debuff = enemy.GetComponent<ElementActiveDebuff>();
            if (debuff != null)
            {
                debuff.ApplyEffect(status.Grass, 5);
                Debug.Log($"[Trail] Debuff applied to: {enemy.name}");
            }
            else
            {
                Debug.LogWarning($"[Trail] No ElementActiveDebuff on: {enemy.name}");
            }

            // Нанесення шкоди
            if (enemy != null)
            {
                float beforeHealth = enemy.health;
                enemy.TakeDamage(basa.damage, 1f);
                float dealt = Mathf.Min(beforeHealth, basa.damage);
                Debug.Log($"[Trail] Damage dealt to: {enemy.name}, amount: {dealt}");

                // Статистика
                GameManager.Instance.FindStatName("trailDamage", dealt);

                // Щоденний квест і лікування
                if (basa.stats.Count > 4 && basa.stats[4].isTrigger)
                {
                    if (enemy.health <= 0)
                    {
                        float heal = enemy.healthMax * 0.1f;
                        Debug.Log($"[Trail] Heal for player: {heal}");
                        if (DailyQuests.instance != null && DailyQuests.instance.quest.FirstOrDefault(s => s.id == 1 && s.isActive == true) != null)
                        {
                            DailyQuests.instance.UpdateValue(1, heal, false, true);
                            Debug.Log("[Trail] Daily quest updated for heal");
                        }
                        PlayerManager.instance.HealHealth(heal);
                    }
                }
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !activeDamages.ContainsKey(other.gameObject))
        {
            var enemy = other.GetComponent<FSMC_Executer>();
            if (enemy != null)
            {
                Debug.Log($"[Trail] Enemy entered: {enemy.name}");
                Coroutine c = StartCoroutine(DamageOverTime(enemy));
                activeDamages.Add(other.gameObject, c);
            }
            else
            {
                Debug.LogWarning($"[Trail] Enemy without FSMC_Executer: {other.name}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && activeDamages.TryGetValue(other.gameObject, out Coroutine c))
        {
            Debug.Log($"[Trail] Enemy exited: {other.name}");
            StopCoroutine(c);
            activeDamages.Remove(other.gameObject);
        }
    }

    private void OnDestroy()
    {
        // Зупиняємо всі корутини при знищенні сегмента
        foreach (var c in activeDamages.Values)
            if (c != null) StopCoroutine(c);
        activeDamages.Clear();
    }
}
