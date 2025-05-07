using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMC.Runtime;
using System;
using Pathfinding;
using System.IO;
using Unity.Mathematics;
using System.Linq;

[Serializable]
public class StateChase : FSMC_Behaviour
{

    AIDestinationSetter destenition;
    Transform target;
    AIPath path;
    Vector3 desiredDirection;
    float dir;
    LevelUpgrade abilInfo;
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
       
        path = executer.GetComponent<AIPath>();
        path.maxSpeed = executer.speed;
        destenition = executer.GetComponent<AIDestinationSetter>();
        target = GameObject.FindAnyObjectByType<PlayerManager>().ShootPoint.transform;
        destenition.target = target;
        executer.anim.speed = UnityEngine.Random.Range(0.8f, 1.2f);

        //if (executer.isBoss)
        //{
        //    abilInfo = GameManager.Instance.levelPanel.GetComponent<LevelUpgrade>();

        //    int abilCount = abilInfo.countActiveAbilities;
        //    int abilUpgrageCount = 0;
        //    foreach (var i in abilInfo.abil.GetComponentsInChildren<CDSkills>())
        //    {
        //        abilUpgrageCount += i.skillMono.currentLevel;
        //    }
        //    Debug.Log(abilInfo.name);
        //    Debug.Log(abilCount + " Count");
        //    Debug.Log(abilUpgrageCount + " Upgrage");
        //    PlayerManager player = target.GetComponent<PlayerManager>();
        //    executer.health = CalculateBossHP(abilCount, abilUpgrageCount, player.activePerkCount, player.countActivePotions, player.itemsFromBoss.countItems);
        //}

    }
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        destenition.target = target;

    }

    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        path.maxSpeed = executer.speed;
        float distance = Vector3.Distance(target.position, executer.transform.position);
        stateMachine.SetFloat("PlayerDistance", distance);
        executer.attackSpeed -= Time.deltaTime;
        stateMachine.SetFloat("AttackSpeed", executer.attackSpeed);
        desiredDirection = path.desiredVelocity.normalized;
        if (desiredDirection.x != 0 && !executer.isBoss)
        {
            dir = desiredDirection.x < -0.1f ? 180 : 0;
        }
        executer.transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(executer.transform.rotation.eulerAngles.y, dir, 5), 0);
    }

    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
    }
    public float CalculateBossHP(int abilityCount, int abilityUpgradeLevels, int perkCount, int potionCount, int bossItemCount)
    {
        float baseHP = 800f;
        float scalingFactor = 1000f;

        // Максимальні значення
        int maxAbilities = 5;
        int maxTotalUpgrade = 25;
        int maxPerks = 16;
        int maxPotions = 2;
        int maxBossItems = 3;

        // Ваги
        float wAbilities = 0.8f;
        float wUpgrades = 1.0f;
        float wPerks = 0.6f;
        float wPotions = 0.4f;
        float wBossItems = 1.2f;

        // Нормалізовані значення
        float a = Mathf.Clamp01((float)abilityCount / maxAbilities);
        float u = Mathf.Clamp01((float)abilityUpgradeLevels / maxTotalUpgrade);
        float p = Mathf.Clamp01((float)perkCount / maxPerks);
        float z = Mathf.Clamp01((float)potionCount / maxPotions);
        float b = Mathf.Clamp01((float)bossItemCount / maxBossItems);

        float score = a * wAbilities + u * wUpgrades + p * wPerks + z * wPotions + b * wBossItems;

        return baseHP + score * scalingFactor;
    }
}