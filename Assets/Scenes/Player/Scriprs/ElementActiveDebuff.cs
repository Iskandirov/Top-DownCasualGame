using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[Serializable]
public class Elements
{
    [SerializeField]
    public enum status
    {
        Fire,
        Electricity,
        Water,
        Dirt,
        Wind,
        Grass,
        Steam,
        Cold
    };
    [SerializeField]
    public List<float> statusCurrentData;
    public List<bool> isActiveCurrentData;
   
    [field: SerializeField] public List<Sprite> elementSprite { get; private set; }
    public float CurrentStatusValue(status name)
    {
        return statusCurrentData[(int)name];
    }
    public void Debuff(EnemyState enemy, status name)
    {
        switch (name)
        {
            case status.Fire:
                statusCurrentData[(int)status.Water] /= 2;
                enemy.Damage(enemy.damage / 2);
                break;
            case status.Electricity:
                statusCurrentData[(int)status.Cold] /= 2;
                break;
            case status.Water:
                statusCurrentData[(int)status.Fire] /= 2;
                break;
            case status.Dirt:
                statusCurrentData[(int)status.Steam] /= 2;
                break;
            case status.Wind:
                statusCurrentData[(int)status.Electricity] /= 2;
                break;
            case status.Grass:
                statusCurrentData[(int)status.Wind] /= 2;
                break;
            case status.Steam:
                statusCurrentData[(int)status.Fire] /= 2;
                statusCurrentData[(int)status.Water] /= 2;

                break;
            case status.Cold:
                statusCurrentData[(int)status.Dirt] /= 2;
                if (EnemyController.instance != null)
                {
                    //move.SlowEnemy(5f, 0.5f);
                }
                break;
        }
        isActiveCurrentData[(int)name] = true;

    }
    public void DeactivateDebuff(EnemyState enemy, status name,GameObject obj)
    {
        switch (name)
        {
            case status.Fire:
                Debug.Log(statusCurrentData[(int)status.Water]);
                statusCurrentData[(int)status.Water] *= 2;
                enemy.Damage(enemy.damage * 2);
                Debug.Log(statusCurrentData[(int)status.Water]);
                break;
            case status.Electricity:
                statusCurrentData[(int)status.Cold] *= 2;
                break;
            case status.Water:
                statusCurrentData[(int)status.Fire] *= 2;
                break;
            case status.Dirt:
                statusCurrentData[(int)status.Steam] *= 2;
                break;
            case status.Wind:
                statusCurrentData[(int)status.Electricity] *= 2;
                break;
            case status.Grass:
                statusCurrentData[(int)status.Wind] *= 2;
                break;
            case status.Steam:
                statusCurrentData[(int)status.Fire] *= 2;
                statusCurrentData[(int)status.Water] *= 2;
                break;
            case status.Cold:
                statusCurrentData[(int)status.Dirt] *= 2;
                break;
        }
        UnityEngine.Object.Destroy(obj);
        isActiveCurrentData[(int)name] = false;
    }
}
//При активації об'єкта в місці зачіпання здібності активується метод з часом життя дебафа, також створюється візуальне відображення дебафа і відбувається начислення самого дебафа
//Дебафи одного типу не можуть існувати, лише унікальні
//При комбінаціїї відповідних дебафів утворюються інші дебафи, або відбувається унікальна дія
public class ElementActiveDebuff : MonoBehaviour
{
    public Elements elements;

    public Transform elementDebuffParent;
    public SpriteRenderer elementDebuffObject;

    public IEnumerator EffectTime(Elements.status name, int lifeTime)
    {
        SpriteRenderer a = null;
        int elementId = (int)name;
        if (elements.isActiveCurrentData[(int)name] != true)
        {
            elements.Debuff(GetComponent<EnemyState>(), name);
            a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.sprite = elements.elementSprite[elementId];
            UnikeEffects(a);
        }
        yield return new WaitForSeconds(lifeTime);
        if (a != null)
        {
            elements.DeactivateDebuff(GetComponent<EnemyState>(), name, a.gameObject);
        }
    }
    private void UnikeEffects(SpriteRenderer sprite)
    {
        var uniqueChildren = new List<SpriteRenderer>();
        foreach (var child in elementDebuffParent.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (child.sprite != sprite.sprite)
            {
                uniqueChildren.Add(child);
            }
        }

        // Видалення повторюваних дочірніх об'єктів
        for (int i = 0; i < elementDebuffParent.transform.childCount - 1; i++)
        {
            if (!uniqueChildren.Contains(elementDebuffParent.transform.GetChild(i).GetComponent<SpriteRenderer>()))
            {
                Destroy(elementDebuffParent.transform.GetChild(i).gameObject);
            }
        }
    }
}
