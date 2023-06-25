using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class ElementActiveDebuff : MonoBehaviour
{
    public Forward move;
    public HealthPoint health;
    public Attack attack;
    public bool isFire;
    public bool isElectricity;
    public bool isWater;
    public bool isDirt;
    public bool isWind;
    public bool isGrass;
    public bool isSteam;
    public bool isCold;
    public Dictionary<string, float> activeTimers = new Dictionary<string, float>()
{
    { "isFire", 5 },
    { "isElectricity", 5 },
    { "isWater", 5 },
    { "isDirt", 5 },
    { "isWind", 5 },
    { "isGrass", 5 },
    { "isSteam", 5 },
    { "isCold", 5 },
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
    public void Start()
    {
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateActiveTimers();
        if (IsActive("isFire") && isFire)
        {
            health.Water /= 2;
            attack.damage = attack.damageMax / 2;
            isFire = false;
        }
        else if(!IsActive("isFire"))
        {
            health.Water = health.WaterStart;
            attack.damage = attack.damageMax;
        }

        if (IsActive("isFire") && IsActive("isWind") && isWater && isWind)
        {
            attack.damage = attack.damageMax / 6;
            isWater = false;
            isWind = false;
        }
        else
        {
            attack.damage = attack.damageMax;
        }

        if (IsActive("isFire") && IsActive("isWater") && isFire && isWater)
        {
            SetBool("isSteam", true);
            isSteam = true;
            isFire = false;
            isWater = false;
        }
        else
        {
            health.Water = health.WaterStart;
            health.Fire = health.FireStart;
        }

        if (IsActive("isCold") && isCold)
        {
            move.speed = move.speedMax / 2;
            health.Steam /= 2;
            isCold = false;
        }
        else
        {
            move.speed = move.speedMax;
            health.Steam = health.SteamStart;
        }

        if (IsActive("isSteam") && isSteam)
        {
            health.Fire /= 2;
            health.Water /= 2;
            isSteam = false;
        }
        else
        {
            health.Fire = health.FireStart;
            health.Water = health.WaterStart;
        }

        if (IsActive("isWater") && IsActive("isElectricity") && isWater && isElectricity)
        {
            move.isStunned = true;
            move.stunnTimeMax = 2;
            SetBool("isWater", false);
            SetBool("isElectricity", false);
            isWater = false;
            isElectricity = false;
        }
        if (IsActive("isWater") && isWater)
        {
            isWater = false;
        } 
        if (IsActive("isDirt") && isDirt)
        {
            isDirt = false;
        }
    }

    public void SetBool(string boolName, bool value)
    {
        if (boolFlags.ContainsKey(boolName))
        {
            boolFlags[boolName] = value;
        }
    }

    private bool IsActive(string boolName)
    {
        foreach (var pair in boolFlags)
        {
            if (pair.Key == boolName && pair.Value == false)
            {
                return false;
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
                    SetBool(key, false);
                    activeTimers[key] = 5f;  // Перевстановлюємо значення на початкове
                }
            }
        }
    }
}
