using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMC.Runtime;
using System;
using Pathfinding;

[Serializable]
public class StateAttack : FSMC_Behaviour
{
    public AIPath path;
    PlayerManager player;
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        player = GameObject.FindAnyObjectByType<PlayerManager>();
        path = executer.GetComponent<AIPath>();
    }
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        executer.anim.SetBool("Attack",true);
        path.maxSpeed = 0;
    }

    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
    }

    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        executer.attackSpeed = executer.attackSpeedMax;
        stateMachine.SetFloat("AttackSpeed", executer.attackSpeedMax);
        executer.anim.SetBool("Attack", false);
        path.maxSpeed = executer.speedMax;
    }

}