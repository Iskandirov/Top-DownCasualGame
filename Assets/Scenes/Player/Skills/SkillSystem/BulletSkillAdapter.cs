using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMC.Runtime;

// Адаптер, которым можно дополнить старые префабы пули.
// Реализует ISkillInitializer, SkillInstance при создании префаба вызовет InitFromSkill.
[RequireComponent(typeof(Bullet))]
public class BulletSkillAdapter : MonoBehaviour, ISkillInitializer
{
    Bullet bullet;
    void Awake()
    {
        bullet = GetComponent<Bullet>();
    }

    // Инициализация на основе ScriptableObject
    public void InitFromSkill(SkillDefinition def, int level)
    {
        if (def == null || bullet == null) return;
        var lvl = def.GetLevelData(level);

        // Применяем значения максимально аккуратно
        if (bullet.basa != null)
        {
            bullet.basa.damage = lvl.baseDamage + lvl.addDamage;
            if (lvl.lifeTime > 0f) bullet.basa.lifeTime = lvl.lifeTime;
            if (lvl.radius > 0f) bullet.basa.radius = lvl.radius;
        }

        // Применяем параметры мультишота
        bullet.projectilesPerShot = Mathf.Max(1, lvl.projectilesPerShot);
        bullet.projectileInterval = Mathf.Max(0f, lvl.projectileInterval);

        // Можно настроить визуал/скорость/etc через дополнительные поля в SkillLevelData
        // bullet.launchForce = Mathf.Max(bullet.launchForce, someValueFromLvl);
    }
}
