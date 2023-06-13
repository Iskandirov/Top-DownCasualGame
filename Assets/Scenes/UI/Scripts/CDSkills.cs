using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CDSkills : MonoBehaviour
{
    public float skillCD;
    public float skillCDMax;
    public Image spriteCD;
    public int number;
    public GameObject text;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
        spriteCD.fillAmount = skillCD / skillCDMax;
    }
}
