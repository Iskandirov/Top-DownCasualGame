using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]
public class Elements 
{
    public string name;
    public Sprite elementSprite;
    public Forward move;
    public HealthPoint health;
    public Attack attack;
    public void Debuff(string name)
    {
        switch (name)
        {
            case "Fire":
                health.Water = health.WaterStart / 2;
                if (attack != null)
                {
                    attack.damage = attack.damageMax / 2;
                }
                break;
            case "Electricity":
                health.Cold = health.ColdStart / 2;
                break;
            case "Water":
                health.Fire = health.FireStart / 2;
                break;
            case "Dirt":
                health.Steam = health.SteamStart / 2;
                break;
            case "Wind":
                health.Electricity = health.ElectricityStart / 2;
                break;
            case "Grass":
                health.Wind = health.WindStart / 2;
                break;
            case "Steam":
                health.Fire = health.FireStart / 2;
                health.Water = health.WaterStart / 2;
                break;
            case "Cold":
                health.Dirt = health.DirtStart / 2;
                if (move != null)
                {
                    move.SlowEnemy(5f, 0.5f);
                }
                break;
        }
    }

}
public class ElementActiveDebuff : MonoBehaviour
{
    public List<Elements> elements;
    public List<Elements> activeEffects;
    public Forward move;
    public HealthPoint health;
    public Attack attack;
    public Transform elementDebuffParent;
    public GameObject elementDebuffObject;
    public Dictionary<string, float> activeTimers = new Dictionary<string, float>()
{
    { "isFire", 3 },
    { "isElectricity", 3 },
    { "isWater", 3 },
    { "isDirt", 3 },
    { "isWind", 3 },
    { "isGrass", 3 },
    { "isSteam", 3 },
    { "isCold", 3 },
};
    public Dictionary<string, bool> boolFlags = new Dictionary<string, bool>()
        {
    { "isFire", false },
    { "isElectricity", false },
    { "isWater", false},
    { "isDirt", false },
    { "isWind", false },
    { "isGrass", false },
    { "isSteam", false },
    { "isCold", false },
};
    public Dictionary<string, bool> boolFlagsCheck = new Dictionary<string, bool>()
        {
    { "isFire", false },
    { "isElectricity", false },
    { "isWater", false},
    { "isDirt", false },
    { "isWind", false },
    { "isGrass", false },
    { "isSteam", false },
    { "isCold", false },
};

    private Dictionary<string, GameObject> elementObjects;

    public Sprite fire;
    public Sprite water;
    public Sprite wind;
    public Sprite cold;
    public Sprite steam;
    public Sprite dirt;
    public Sprite grass;
    public Sprite electricity;
    public void Start()
    {
        elementObjects = new Dictionary<string, GameObject>();
    }
    //При активації об'єкта в місці зачіпання здібності активується метод з часом життя дебафа, також створюється візуальне відображення дебафа і відбувається начислення самого дебафа
    //Дебафи одного типу не можуть існувати, лише унікальні
    //При комбінаціїї відповідних дебафів утворюються інші дебафи, або відбувається унікальна дія
    public IEnumerable EffectTime(Elements element, int lifeTime)
    {
        activeEffects.Add(element);
        GameObject a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
        a.GetComponent<SpriteRenderer>().sprite = element.elementSprite;
        yield return new WaitForSeconds(lifeTime);
        activeEffects.Remove(element);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateActiveTimers();
        if (IsActive("isFire", true) && IsActive("isFire", false))
        {
            GameObject a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.GetComponent<SpriteRenderer>().sprite = fire;
            health.Water = health.WaterStart / 2;
            if (attack != null)
            {
                attack.damage = attack.damageMax / 2;
            }
            SetBool("isFire", false, false);
        }
        else if(!IsActive("isFire", true))
        {
            health.Water = health.WaterStart;
            if (attack != null)
            {
                attack.damage = attack.damageMax;

            }
        }

        if (IsActive("isWater", true) && IsActive("isWind", true) && IsActive("isWater", false) && IsActive("isWind", false))
        {
            GameObject a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.GetComponent<SpriteRenderer>().sprite = water;
            GameObject b = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            b.GetComponent<SpriteRenderer>().sprite = wind;
            if (attack != null)
            {
                attack.damage = attack.damageMax / 6;
            }
                SetBool("isWind", false, false);
            SetBool("isWater", false, false);
        }
        else if(!IsActive("isWater", true) && !IsActive("isWind", true))
        {
            if (attack != null)
            {
                attack.damage = attack.damageMax;
            }
        }

        if (IsActive("isFire", true) && IsActive("isWater", true) && IsActive("isWater", false))
        {
            SetBool("isSteam", true, true);
            SetBool("isSteam", true, false);
            SetBool("isWater", false, false);
            SetBool("isFire", false, false);
        }
        else if(!IsActive("isFire", true) && !IsActive("isWater", true))
        {
            health.Water = health.WaterStart;
            health.Fire = health.FireStart;
        }

        if (IsActive("isCold", true) && IsActive("isCold", false))
        {
            GameObject a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.GetComponent<SpriteRenderer>().sprite = cold;
            if (move != null)
            {
                move.path.maxSpeed = move.speedMax / 2;
            }
            health.Steam = health.SteamStart / 2;
            SetBool("isCold", false, false);
        }
        else if (!IsActive("isCold", true))
        {
            if (move != null)
            {
                //SpeedNormalized();
            }

        }

        if (IsActive("isSteam", true) && IsActive("isSteam", false))
        {
            GameObject a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.GetComponent<SpriteRenderer>().sprite = steam;
            health.Fire = health.FireStart / 2;
            health.Water = health.WaterStart / 2;
            SetBool("isSteam", false, false);
        }
        else if(!IsActive("isSteam", true))
        {
            health.Fire = health.FireStart;
            health.Water = health.WaterStart;
        }

        if (IsActive("isWater", true) && IsActive("isElectricity", true) && IsActive("isWater", false) && IsActive("isElectricity", false))
        {
            GameObject a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.GetComponent<SpriteRenderer>().sprite = water;

            GameObject b = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            b.GetComponent<SpriteRenderer>().sprite = electricity;
            if (move != null)
            {
                move.isStunned = true;
                move.stunnTime = 2;
            }
            SetBool("isWater", false, true);
            SetBool("isElectricity", false, true);
            SetBool("isWater", false, false);
            SetBool("isElectricity", false, false);

        }
        if (IsActive("isWater", true) && IsActive("isWater", false))
        {
            GameObject a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.GetComponent<SpriteRenderer>().sprite = water;
            SetBool("isWater", false, false);
        } 
        if (IsActive("isDirt", true) && IsActive("isDirt", false))
        {
            GameObject a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.GetComponent<SpriteRenderer>().sprite = dirt;
            SetBool("isDirt", false, false);
        }
    }

    void SpeedNormalized()
    {
        move.path.maxSpeed = move.speedMax;
        health.Steam = health.SteamStart;
    }
    public void SetBool(string boolName, bool value, bool isFirst)
    {
        if (isFirst)
        {
            if (boolFlags.ContainsKey(boolName))
            {
                boolFlags[boolName] = value;

            }
        }
        else
        {
            if (boolFlagsCheck.ContainsKey(boolName))
            {
                boolFlagsCheck[boolName] = value;
            }
        }
    }

    public bool IsActive(string boolName,bool isFirst)
    {
        if (isFirst)
        {
            foreach (var pair in boolFlags)
            {
                if (pair.Key == boolName && pair.Value == false)
                {
                    return false;
                }
            }
        }
        else
        {
            foreach (var pair in boolFlagsCheck)
            {
                if (pair.Key == boolName && pair.Value == false)
                {
                    return false;
                }
            }
        }
      
        return true;
    }

    private void UpdateActiveTimers()
    {
        string[] keysToUpdate = boolFlags.Keys.ToArray();

        foreach (var key in keysToUpdate)
        {
            if (boolFlags[key] == true)
            {
                activeTimers[key] -= Time.deltaTime;

                if (activeTimers[key] <= 0f)
                {
                    SetBool(key, false,true);
                    activeTimers[key] = 3f;  // Перевстановлюємо значення на початкове
                }
            }
        }
    }
    public void DeactiveDebuff()
    {
        string[] keysToUpdate = boolFlags.Keys.ToArray();

        foreach (var key in keysToUpdate)
        {
            activeTimers[key] = 0f;
        }
    }
}
