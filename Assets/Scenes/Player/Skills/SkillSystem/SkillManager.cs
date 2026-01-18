using System.Collections.Generic;
using UnityEngine;

// Простая регистрация и фабрика умений.
// Позволяет централизованно хранить доступные определения и создавать SkillInstance.
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Tooltip("All skill definitions used in game (populate from editor)")]
    public List<SkillDefinition> skillDefinitions = new List<SkillDefinition>();

    // runtime registry: id -> SkillDefinition
    private Dictionary<string, SkillDefinition> registry = new Dictionary<string, SkillDefinition>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        foreach (var def in skillDefinitions)
        {
            if (def == null) continue;
            if (!registry.ContainsKey(def.id))
                registry.Add(def.id, def);
            else
                Debug.LogWarning($"SkillManager: duplicate skill id '{def.id}'");
        }
    }

    public SkillDefinition GetDefinition(string id)
    {
        registry.TryGetValue(id, out var def);
        return def;
    }

    public SkillInstance CreateInstance(string id, int level = 1)
    {
        var def = GetDefinition(id);
        if (def == null) return null;
        return new SkillInstance(def, level);
    }

    // Уровень апгрейда: ограничение до MaxLevel
    public int LevelUp(SkillInstance instance)
    {
        if (instance == null || instance.definition == null) return 0;
        instance.level = Mathf.Min(instance.definition.MaxLevel, instance.level + 1);
        return instance.level;
    }
}
