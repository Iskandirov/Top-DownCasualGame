using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHealth : MonoBehaviour
{
    public int health = 3;
    private int maxHealth = 3;
    public Sprite[] blockDamagedSprite;
    public SpriteRenderer blockSprite;
    private Vector3 startLocalPos;

    public bool isRespawnable = true;

    [SerializeField] PuzzleController puzzleController;

    [Header("Debris Settings")]
    public Sprite[] debrisSprites; // спрайти уламків
    public float debrisForce = 5f;
    public float debrisLifetime = 2f;
    public float respawnDelay = 1f;


    private void Awake()
    {
        startLocalPos = transform.position;
    }

    private void Start()
    {
        maxHealth = health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            health--;

            if (health > 0)
            {
                blockSprite.sprite = blockDamagedSprite[maxHealth - health];
            }
            else
            {
                StartCoroutine(DestroyAndRespawn());
            }
        }
    }
    
    private IEnumerator DestroyAndRespawn()
    {
        if (!isRespawnable)
            blockSprite.gameObject.SetActive(false);
        SpawnDebris();

        blockSprite.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        if (!isRespawnable) yield break;

        // Дочекатися завершення SmoothDrop
        var drag = GetComponent<DragAndDropTrap>();
        if (drag != null)
            yield return StartCoroutine(drag.SmoothDropAndWait());

        // Тепер переміщаємо у потрібне місце
        transform.position = startLocalPos;
        drag.ResetState(); // <--- Додаємо цей рядок
        health = maxHealth;
        blockSprite.sprite = blockDamagedSprite[0];
        blockSprite.enabled = true;
        GetComponent<Collider2D>().enabled = true;
        
    }

    private void SpawnDebris()
    {
        puzzleController.SolvePuzzle();
        foreach (var sprite in debrisSprites)
        {
            GameObject debris = new GameObject("Debris");
            debris.transform.position = transform.position;

            // Рендер спрайта
            var sr = debris.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = blockSprite.sortingOrder;
            sr.material = blockSprite.material; // використовуємо той самий матеріал, що й у блоку

            // Фізика
            var rb = debris.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f; // можна змінити
            rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f)) * debrisForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-200f, 200f));

            // Автоматичне знищення уламка
            Destroy(debris, debrisLifetime);
        }
    }
}
