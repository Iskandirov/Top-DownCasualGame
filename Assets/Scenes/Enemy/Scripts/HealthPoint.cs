using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class HealthPoint : MonoBehaviour
{
    public GameObject player;
    public float stepKick;
    public float stepKickMax;
    public bool isKick;
    public float healthPoint;
    public float healthPointMax;
    public EXP expiriancePoint;
    public bool IsBobs;
    public Animator anim;
    public GameObject bodyAnim;
    public GameObject VFX_Deadarticle;
    public bool isBurn;
    public float burnTime;
    public float burnDamage;
    public float burnTick;
    public float burnTickMax;
    SpriteRenderer[] objsSprite;
    Expirience objExp;
    DropItems objDrop;
    Forward objMove;
    KillCount objKills;

    public float expGiven;
    public float dangerLevel;

    private Camera mainCamera;
    public float spawnRadius = 10f;
    // Start is called before the first frame update
    void Start()
    {

        healthPointMax = healthPoint;
        player = GameObject.FindWithTag("Player");
        objsSprite = GetComponentsInChildren<SpriteRenderer>();
        objExp = player.GetComponent<Expirience>();
        objDrop = GetComponentInParent<DropItems>();
        objMove = GetComponentInParent<Forward>();
        objKills = FindObjectOfType<KillCount>();
    }
    private Bounds GetCameraBounds()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;

        //float minX = cameraPosition.x - cameraWidth / 2f;
        //float maxX = cameraPosition.x + cameraWidth / 2f;
        //float minY = cameraPosition.y - cameraHeight / 2f;
        //float maxY = cameraPosition.y + cameraHeight / 2f;

        return new Bounds(cameraPosition, new Vector3(cameraWidth, cameraHeight, 0f));
    }

    private bool IsInsideCameraBounds(Vector3 position)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
        return viewportPosition.x >= -0f && viewportPosition.x <= 1f && viewportPosition.y >= -0f && viewportPosition.y <= 1f;
    }

    private bool IsInsideWallBounds(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f); // Використовуємо OverlapCircleAll замість OverlapPointAll
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetRandomSpawnPosition(Bounds cameraBounds)
    {
        Vector3 spawnPosition;
        do
        {
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * spawnRadius;
            spawnPosition = new Vector3(mainCamera.transform.position.x + spawnOffset.x, mainCamera.transform.position.y + spawnOffset.y, 1.8f);
        } while (IsInsideCameraBounds(spawnPosition) || IsInsideWallBounds(spawnPosition));

        return spawnPosition;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera = Camera.main;
        if (isBurn)
        {
            burnTime -= Time.deltaTime;
            if (burnTime <= 0)
            {
                isBurn = false;
            }
            burnTick -= Time.deltaTime;
            if (burnTick <= 0)
            {
                ChangeToKick();
                healthPoint -= burnDamage;
                ChangeToNotKick();
                burnTick = burnTickMax;
            }

        }
        ChangeToNotKick();
        if (healthPoint <= 0)
        {
            EXP a = Instantiate(expiriancePoint, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1.9f), Quaternion.identity);
            a.expBuff = expGiven * dangerLevel;

            objExp.expiriencepoint.fillAmount += expGiven * dangerLevel / objExp.expNeedToNewLevel;
            objExp.ShowLevel();
            objKills.score += 1;

            if (IsBobs)
            {
                objDrop.OnDestroyBoss();
                Destroy(gameObject.transform.root.gameObject);
            }
            else
            {
                Bounds cameraBounds = GetCameraBounds();
                // Генеруємо випадкові координати в межах заданої області спавну
                Vector3 spawnPosition = GetRandomSpawnPosition(cameraBounds);
                objMove.transform.position = spawnPosition;

                healthPoint = healthPointMax;
            }
           
        }
    }

    public void ChangeToKick()
    {
        for (int i = 0; i < objsSprite.Length; i++)
        {
            objsSprite[i].color = new Color32(255, 0, 0, 255);
        }
        isKick = true;
    }
    public void ChangeToNotKick()
    {
        if (isKick == true)
        {
            stepKick -= Time.deltaTime;
            if (stepKick <= 0)
            {
                for (int i = 0; i < objsSprite.Length; i++)
                {
                    objsSprite[i].color = new Color32(255, 255, 255, 255);
                }
                isKick = false;
                stepKick = stepKickMax;
            }
        }
    }
}
