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
    public bool isBlood;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFire)
        {
            attack.damage = attack.damageMax / 2;
        }
        else
        {
            attack.damage = attack.damageMax;
        }
        if (isFire && isWind)
        {
            attack.damage = attack.damageMax / 6;
        }
        else
        {
            attack.damage = attack.damageMax;
        }
        if (isFire && isWater)
        {
            attack.damage = attack.damageMax / 6;
        }
        else
        {
            attack.damage = attack.damageMax;
        }
    }
}
