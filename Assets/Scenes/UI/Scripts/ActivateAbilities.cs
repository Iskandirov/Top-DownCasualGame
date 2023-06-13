using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateAbilities : MonoBehaviour
{
    public Image[] abilities;
    public GameObject[] abilitiesObj;
    public int countActiveAbilities;
    // Start is called before the first frame update
    void Start()
    {
        countActiveAbilities = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (var abil in abilitiesObj)
        //{
        //    if (abil.activeSelf)
        //    {
        //        Debug.Log(abil.name);
        //    }
        //}
    }
}
