using UnityEngine;

// Простой интерфейс для эффектов. При необходимости расширим.
public interface ISkillEffect
{
    void OnApply(GameObject source, GameObject target);
    void OnTick(GameObject source);
    void OnRemove(GameObject source);
}
