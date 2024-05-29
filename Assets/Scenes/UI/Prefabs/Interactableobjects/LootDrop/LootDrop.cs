using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LootDrop : MonoBehaviour
{
    public GameObject lootObject;
    public VisualEffect lootVFX;

    void Start()
    {
        lootVFX.Play();
    }
}
