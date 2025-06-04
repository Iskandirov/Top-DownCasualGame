using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMC.Runtime;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable]
public class StateHit : FSMC_Behaviour
{
    [SerializeField] TextMeshProUGUI dpsText;
    private static float dps;
    private static float totalDamage = 0f;
    public static float CurrentDPS => dps;
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        dpsText = GameObject.Find("UI").transform.Find("DPS").GetComponent<TextMeshProUGUI>();
        executer.health = executer.healthMax;
        if (executer.isBoss && !LootManager.inst.isTutor)
        {
            Transform parent = GameObject.Find("UI").transform;
            executer.expiriancePoint = GameObject.Find("Expiriance");
            GameObject instantiatedObject = UnityEngine.Object.Instantiate(executer.bossHealthObj, parent);
            executer.healthObjImg = instantiatedObject.transform.GetChild(1).GetComponent<Image>();
            executer.avatar = instantiatedObject.transform.GetChild(2).GetComponent<Image>();
            executer.bossHealthObj.SetActive(true);
            executer.avatar.sprite = GameManager.ExtractSpriteListFromTexture("boss_avatars").First(a => a.name == ("boss_avatars_" + (SceneManager.GetActiveScene().buildIndex - 2)));
        }
        else if(executer.isBoss && LootManager.inst.isTutor)
        {
            executer.bossHealthObj.SetActive(true);
        }
    }
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        RegisterDamage(executer.GetDamage(), executer);
        if (executer.isBoss)
            executer.healthObjImg.fillAmount = 1 - (executer.healthMax - executer.health) / executer.healthMax;
        executer.anim.SetTrigger("Hit");
        AudioManager.instance.PlaySFX("Hit");
        if (executer.health <= 0)
        {
            executer.anim.SetBool("Death", true);
            stateMachine.SetTrigger("Death");
            //stateMachine.SetTrigger("Death");
        }
        else if (executer.GetFloat("Stun Time") > 0.2f)
        {
            stateMachine.SetCurrentState("Stun", executer);
        }
        else if (executer.GetFloat("SlowTime") > 0.2f)
        {
            stateMachine.SetCurrentState("Slowed", executer);
        }
        else
        {
            stateMachine.SetCurrentState("Chase", executer);
        }
    }

    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
       
    }
   
    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {

    }
   
    public void RegisterDamage(float damage, FSMC_Executer executer)
    {
        totalDamage = executer.GetDamageData();
        totalDamage += damage;
        dpsText.text = executer.GetDPSData().ToString("F2") + " DPS";
        executer.SetDamageData(totalDamage);

    }
    public static float GetTotalDamage()
    {
        return totalDamage;
    }
    
}