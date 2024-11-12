using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMC.Runtime;
using System;
using Pathfinding;
using System.IO;
using Unity.Mathematics;

[Serializable]
public class StateChase : FSMC_Behaviour
{
    
    AIDestinationSetter destenition;
    Transform target;
    AIPath path;
    Vector3 desiredDirection;
    float dir;
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        path = executer.GetComponent<AIPath>();
        path.maxSpeed = executer.speed;
        destenition = executer.GetComponent<AIDestinationSetter>();
        target = GameObject.FindAnyObjectByType<PlayerManager>().objTransform;
        destenition.target = target;
        executer.anim.speed = UnityEngine.Random.Range(0.8f, 1.2f);

    }
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        destenition.target = target;

    }

    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        Debug.Log("Chase Start");
        path.maxSpeed = executer.speed;
        float distance = Vector3.Distance(target.position, executer.transform.position);
        stateMachine.SetFloat("PlayerDistance", distance);
        executer.attackSpeed -= Time.deltaTime;
        stateMachine.SetFloat("AttackSpeed", executer.attackSpeed);
        desiredDirection = path.desiredVelocity.normalized;
        if (desiredDirection.x != 0)
        {
            dir = desiredDirection.x < -0.1f ? 180 : 0;
        }
        executer.transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(executer.transform.rotation.eulerAngles.y, dir, 5), 0);
        Debug.Log("Chase End");
    }

    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {

    }
}