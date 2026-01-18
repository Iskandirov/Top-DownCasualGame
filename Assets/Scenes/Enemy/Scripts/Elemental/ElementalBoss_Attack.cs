using FSMC.Runtime;
using Spine.Unity;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class BossAttacks
{
    public string attackName;
    public float cooldown;
    public MonoBehaviour attackScript; // Скрипт, який реалізує атаку

    public void Execute()
    {
        if (attackScript != null)
        {
            attackScript.Invoke("ExecuteAttack", 0f); // Скрипт має метод ExecuteAttack()
        }
    }
}
[System.Serializable]
public class BossStage
{
    public string stageName;
    public float healthThreshold; // <= цього значення -> перехід в стадію
    public BossAttacks[] attacks;
    public bool canAct = true;       // Чи може рухатись/атакувати
}

public class ElementalBoss_Attack : MonoBehaviour
{
    public FSMC_Executer boss;
    public BossStage[] stages;
    private int currentStageIndex = 0;
    private BossStage currentStage;

    private bool isInTransition = false;
    private Coroutine attackLoop;

    [Header("Puzzle Settings")]
    public GameObject puzzleParent;
    public List<GameObject> puzzlePrefab;
    public Vector3[] puzzleSpawnPoints; // список точок для спавну
    public bool waitPuzzleToStage = true;

    private PuzzleController activePuzzle;
    System.Random rand = new System.Random();
    private void Start()
    {
        SetStage(0);
    }

    private void Update()
    {
        if (!isInTransition && currentStageIndex + 1 < stages.Length)
        {
            var nextStage = stages[currentStageIndex + 1];
            if (boss.health <= (boss.healthMax / 100) * nextStage.healthThreshold)
            {
                StartPuzzlePhase(currentStageIndex + 1);
                boss.isVulnerable = false;
                Debug.Log($"Boss entering puzzle phase to reach stage {nextStage.stageName}" ); 
            }
        }
    }

    private void SetStage(int index)
    {
        puzzleParent = FindAnyObjectByType<EnemySpawner>().gameObject;
        currentStageIndex = index;
        currentStage = stages[currentStageIndex];

        if (attackLoop != null)
            StopCoroutine(attackLoop);

        if (currentStage.canAct)
            attackLoop = StartCoroutine(AttackCycle());
    }

    private void StartPuzzlePhase(int targetStageIndex)
    {
        isInTransition = true;

        // Обираємо випадкову точку для спавну
        Vector3 spawnPoint = Vector3.zero;
        if (puzzleSpawnPoints != null && puzzleSpawnPoints.Length > 0)
        {
            int rnd = Random.Range(0, puzzleSpawnPoints.Length);
            spawnPoint = puzzleSpawnPoints[rnd];
        }
        GameObject randomPuzzle = puzzlePrefab[rand.Next(puzzlePrefab.Count)];
        // Створюємо головоломку
        if (randomPuzzle != null && spawnPoint != null)
        {
            var puzzleObj = Instantiate(randomPuzzle, spawnPoint, Quaternion.identity, puzzleParent.transform);
            activePuzzle = puzzleObj.GetComponent<PuzzleController>();
            if (activePuzzle != null)
            {
                activePuzzle.Init(this, targetStageIndex);
            }
        }

        // Якщо хочемо щоб бос нічого не робив — вмикаємо "пасивний режим"
        if (waitPuzzleToStage)
        {
            if (attackLoop != null)
                StopCoroutine(attackLoop);

            // тут можна запустити анімацію "лежить" або "відновлення"
             boss.anim.SetTrigger("Stan");
        }
    }

    // Викликається PuzzleController, коли гравець розв'язав головоломку
    public void OnPuzzleCompleted(int newStageIndex)
    {
        boss.isVulnerable = true;
        currentStage.canAct = true;

        SetStage(newStageIndex);
        isInTransition = false;
    }

    private IEnumerator AttackCycle()
    {
        while (true)
        {
            if (currentStage.attacks.Length == 0)
            {
                yield return null;
                continue;
            }

            // Впорядкування за cooldown
            var sortedAttacks = currentStage.attacks.OrderBy(a => a.cooldown).ToList();

            foreach (var attack in sortedAttacks)
            {
                if (!currentStage.canAct)
                    break;
                yield return new WaitForSeconds(attack.cooldown);
                attack.Execute();
            }
        }
    }
}
