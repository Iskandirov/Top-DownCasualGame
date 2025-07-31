using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMC.Runtime;
using System;
using System.Linq;
using Pathfinding;
using System.CodeDom.Compiler;

[Serializable]
public class StateDeath : FSMC_Behaviour
{
    AIPath path;
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        path = executer.GetComponent<AIPath>();

    }
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        path.maxSpeed = 0;
    }

    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
    }

    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        if (executer.isBoss)
        {
            executer.GetComponent<LootManager>().DropLoot(false, executer.transform);
            EnemySpawner.instance.children.Clear();
            UnityEngine.Object.Destroy(executer.gameObject);
        }
        else
        {
            ExpGive(executer, executer.objTransform.position);
            executer.health = executer.healthMax;
            Respawn(executer);
            //executer.StateMachine.SetCurrentState("Chase", executer);
        }
    }
    void ExpGive(FSMC_Executer enemy, Vector3 pos)
    {
        DailyQuests.instance.UpdateValue(0, 1, false,true);
        if (enemy.name == "Infiltrator")
        {
            DailyQuests.instance.UpdateValue(2, 1, false, true);
        }
        EXP a = UnityEngine.Object.Instantiate(enemy.expiriancePoint.GetComponent<EXP>(), pos, Quaternion.identity);
        a.expBuff = enemy.expGiven * enemy.dangerLevel;
        GameManager.Instance.expiriencepoint.fillAmount += enemy.expGiven * enemy.dangerLevel / PlayerManager.instance.expNeedToNewLevel;
        GameManager.Instance.score++;

    }
    ///Spawn
    public void Respawn(FSMC_Executer executer)
    {
        executer.transform.position = GetRandomSpawnPosition(executer.transform.position, true, 100);
        ElementActiveDebuff element = executer.GetComponent<ElementActiveDebuff>();
        for (int i = 0; i < element.isActiveCurrentData.Count; i++)
        {
            if (element.isActiveCurrentData[i])
            {
                element.DeactivateDebuff(executer, (status)i);
            }
        }
        for (int i = 0; i < executer.GetComponent<ElementActiveDebuff>().elementDebuffParent.transform.childCount; i++)
        {
            Transform child = executer.GetComponent<ElementActiveDebuff>().elementDebuffParent.transform.GetChild(i);

            UnityEngine.Object.Destroy(child.gameObject);
        }
        executer.anim.SetBool("Death", false);
    }
    static public bool GetSpawnPositionNotInAIPath(float radius, Vector2 pos)
    {
        // Ñòâîðèòè ñïèñîê äîñòóïíèõ ïîçèö³é
        List<Vector3> availablePositions = new List<Vector3>();

        // Îòðèìàòè âóçëè ñ³òêè
        var nodes = AstarPath.active.data.gridGraph.nodes;

        // Ïåðåáðàòè âñ³ âóçëè â ðàä³óñ³
        foreach (var node in nodes.Where(n => Mathf.Abs(n.position.x) > radius || Mathf.Abs(n.position.y) > radius))
        {
            if (node.Walkable)
            {
                availablePositions.Add(new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid));
                Debug.Log(new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid));
            }
        }

        return availablePositions.Contains(pos);
    }
    public Vector3 SpawnObjectInMapBounds(FSMC_Executer executer)
    {
        Vector2 colliderCenter = executer.mapBounds.bounds.center;
        Vector2 randomPointInsideCollider = colliderCenter + UnityEngine.Random.insideUnitCircle * new Vector2(executer.mapBounds.bounds.size.x * 0.6f, executer.mapBounds.bounds.size.y * 0.5f);
        return randomPointInsideCollider;
    }
    private bool IsInsideCameraBounds(Vector3 position, bool needToBeOutside)
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(position);
        if (needToBeOutside)
            return viewportPosition.x >= 0f && viewportPosition.x <= 1f && viewportPosition.y >= 0f && viewportPosition.y <= 1f;
        else
            return viewportPosition.x <= 0.2f && viewportPosition.x >= 0.8f && viewportPosition.y <= 0.2f && viewportPosition.y >= 0.8f;
    }
    private bool IsInsideWallBounds(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
    public Vector3 GetRandomSpawnPosition(Vector3 pos, bool needToBeOutside, float radius)
    {
        Vector3 spawnPosition;
        do
        {
            float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * radius;
            spawnPosition = new Vector3(pos.x + spawnOffset.x, pos.y + spawnOffset.y, 0);
        } while (IsInsideCameraBounds(spawnPosition, needToBeOutside) || IsInsideWallBounds(spawnPosition));

        return spawnPosition;
    }
    ///Spawn end
}