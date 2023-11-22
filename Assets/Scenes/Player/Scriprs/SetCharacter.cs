using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public int id;
    public int health;
    public int moveSpeed;
    public int damage;
    public float attackSpeed;
    public float spellCD;
}
public class SetCharacter : MonoBehaviour
{
    BuyCharacter charInfo;
    public List<CharacterStats> characters;
    // Start is called before the first frame update
    void Start()
    {
        charInfo = FindObjectOfType<BuyCharacter>();
        foreach (var character in characters)
        {
            foreach (var info in charInfo.charactersRead)
            {
                if (character.id == info.ID && info.isEquiped)
                {
                    GetComponent<Health>().playerHealthPointMax = character.health;
                    GetComponent<Health>().playerHealthPoint = character.health;

                    GetComponent<Move>().speedMax = character.moveSpeed;
                    GetComponent<Move>().speed = character.moveSpeed;
                    GetComponent<Move>().heroID = character.id;
                    GetComponent<Move>().dashTimeMax = character.spellCD;
                    GetComponent<Move>().shiftCDMax = character.spellCD;

                    GetComponent<Shoot>().attackSpeedMax = character.attackSpeed;
                    GetComponent<Shoot>().attackSpeed = character.attackSpeed;

                    GetComponent<Shoot>().damageToGive = character.damage;
                    break;
                }
            }
        }
    }

}
