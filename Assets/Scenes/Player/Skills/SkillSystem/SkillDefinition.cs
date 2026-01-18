using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDefinition_", menuName = "Game/SkillDefinition", order = 100)]
public class SkillDefinition : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab; // префаб, который будет инстанцироваться
    [Tooltip("Levels: index 0 -> level 1, ... max 5")]
    public List<SkillLevelData> levels = new List<SkillLevelData>(new SkillLevelData[5]);

    public int MaxLevel => 5;

    public SkillLevelData GetLevelData(int level)
    {
        int idx = Mathf.Clamp(level - 1, 0, levels.Count - 1);
        return levels[idx];
    }

    public float GetDamageForLevel(int level)
    {
        var d = GetLevelData(level);
        return d.baseDamage + d.addDamage;
    }

    [Serializable]
    public class SkillLevelData
    {
        public string shortDescription;
        public float baseDamage = 0f;
        public float addDamage = 0f;
        public float radius = 0f;
        public float lifeTime = 0f;

        // Новые поля для мультишота
        [Tooltip("Количество снарядов, выпускаемых за один выстрел (1 = стандарт)")]
        public int projectilesPerShot = 1;
        [Tooltip("Интервал (сек) между снарядами в мультишоте")]
        public float projectileInterval = 0.2f;

        // дополнительные поля можно добавить позже (cooldown, cost и т.д.)
    }
}
