using UnityEngine;
using UnityEngine.SceneManagement;
[DefaultExecutionOrder(10)]
public class SpawnBobs : MonoBehaviour
{
    //public Timer timer;
    //public float timeToSpawnBobs;
    //float timeToSpawnBobsStart;
    //public GameObject bobs;
    //public int bosscount;
    //public bool isSpawned = false;
    //GameManager gameManager;
    //AudioManager audioManager;
    //PlayerManager player;
    //public GameObject enemyParentl;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    gameManager = GameManager.Instance;
    //    audioManager = AudioManager.instance;
    //    player = PlayerManager.instance;
    //    if (gameManager.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) > 0)
    //        timeToSpawnBobs += 15;
    //    timeToSpawnBobsStart = timeToSpawnBobs;
    //}

    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    if (timer.time >= timeToSpawnBobs && isSpawned == false)
    //    {
    //        foreach (SaveEnemyInfo obj in gameManager.enemyInfo)
    //        {
    //            if (obj.Name.Contains(bobs.name))
    //            {
    //                if (gameManager.CheckInfo(obj.ID))
    //                {
    //                    gameManager.FillInfo(obj.ID);
    //                    gameManager.enemyInfoLoad.Clear();
    //                    gameManager.LoadEnemyInfo();
    //                }
    //            }
    //        }

    //        //GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("Enemy");

    //        //foreach (var obj in objectsToDelete)
    //        //{
    //        //    Destroy(obj.GetComponentInParent<HealthPoint>().transform.parent.gameObject);
    //        //    gameManager.enemyCount = 0;
    //        //}
    //        Destroy(enemyParentl);
    //        GetComponent<EnemyController>().SetSpawnStatus(true);
    //        if (audioManager != null)
    //        {
    //            audioManager.PlayMusic("BossFight1");
    //        }
    //        GameObject boss = Instantiate(bobs, transform.position, Quaternion.identity);
    //        //boss.GetComponent<Forward>().player = player;
    //        isSpawned = true;
    //        timeToSpawnBobs += timeToSpawnBobsStart;
    //    }
    //}
}
