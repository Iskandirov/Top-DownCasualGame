using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePrison : MonoBehaviour
{
    [Header("StonePrison Settings")]
    [SerializeField] private GameObject prisonMarkPrefab;
    [SerializeField] private GameObject stonePrisonPrefabHorizontal;
    [SerializeField] private GameObject stonePrisonPrefabVertical;
    [SerializeField] private float followDuration = 2f;
    [SerializeField] private float delayBeforeSpawn = 1f;
    public float barrierLifetime = 5f;
    public float stoneSpawnRadius = 2f;
    public int numberOfBarriers = 4;

    PlayerManager player;
    public void ExecuteAttack()
    {
        player = PlayerManager.instance;
        StartCoroutine(StonePrisonRoutine());
    }
    private IEnumerator StonePrisonRoutine()
    {
        GameObject mark = Instantiate(prisonMarkPrefab);
        Transform markTransform = mark.transform;

        float elapsed = 0f;
        while (elapsed < followDuration)
        {
            markTransform.position = player.objTransform.position;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector2 finalPosition = markTransform.position;



        yield return new WaitForSeconds(delayBeforeSpawn);
        Instantiate(stonePrisonPrefabHorizontal, finalPosition, Quaternion.identity);
    }
}