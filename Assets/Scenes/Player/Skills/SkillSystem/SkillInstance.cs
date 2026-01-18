using UnityEngine;

// Лёгкая runtime-обёртка вокруг SkillDefinition.
// Отвечает за инстанцирование префаба и применение данных уровня.
public class SkillInstance
{
    public SkillDefinition definition;
    public int level;

    public SkillInstance(SkillDefinition def, int level = 1)
    {
        definition = def;
        this.level = Mathf.Clamp(level, 1, def != null ? def.MaxLevel : 1);
    }

    // Спавнит префаб и применяет параметры уровня к компонентам типа SkillBaseMono или специфичным адаптерам.
    public GameObject Activate(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (definition == null || definition.prefab == null)
        {
            Debug.LogWarning($"SkillInstance.Activate: definition or prefab is null for skill '{definition?.id}'");
            return null;
        }

        GameObject go = Object.Instantiate(definition.prefab, position, rotation, parent);

        // Если на префабе есть SkillBaseMono (старый базовый класс) — применяем базовые параметры
        var sb = go.GetComponentInChildren<SkillBaseMono>();
        if (sb != null)
        {
            var lvl = definition.GetLevelData(level);
            // Безопасно применяем только если поля доступны
            try
            {
                sb.basa.damage = lvl.baseDamage + lvl.addDamage;
                if (lvl.lifeTime > 0) sb.basa.lifeTime = lvl.lifeTime;
                if (lvl.radius > 0) sb.basa.radius = lvl.radius;
            }
            catch { /* защищаемся от возможных несоответствий полей */ }
        }

        // Также пытаемся найти адаптеры (например BulletSkillAdapter) и передать definition/level
        var adapters = go.GetComponentsInChildren<ISkillInitializer>();
        foreach (var adapter in adapters)
        {
            adapter.InitFromSkill(definition, level);
        }

        return go;
    }
}

// Интерфейс для адаптеров префабов, которые умеют инициализироваться из SkillDefinition
public interface ISkillInitializer
{
    void InitFromSkill(SkillDefinition def, int level);
}
