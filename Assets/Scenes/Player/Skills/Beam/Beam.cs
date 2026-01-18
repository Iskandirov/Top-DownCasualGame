using FSMC.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class Beam : SkillBaseMono
{
    public float tick;
    public float addToAndle;
    public bool isTwo;
    public SpriteRenderer img;
    Transform objTransform;
    public Transform bindTransform;

    // Кеш оригинального значения урона (чтобы не умножать Steam несколько раз)
    private float baseDamage;

    // Централизованный реестр активных лучей — избавляет от FindObjectsOfType в Start
    private static readonly List<Beam> activeBeams = new List<Beam>();

    void Awake()
    {
        player = PlayerManager.instance;
        objTransform = transform;
    }

    void OnEnable()
    {
        if (!activeBeams.Contains(this))
            activeBeams.Add(this);
    }

    void OnDisable()
    {
        activeBeams.Remove(this);
    }

    void Start()
    {
        // Сохраняем исходный дамаг, применим модификаторы один раз
        baseDamage = basa.damage;

        // Применяем триггерные модификаторы (как было), безопасно — без повторного умножения Steam
        if (basa.stats.Count > 1 && basa.stats[1].isTrigger)
        {
            basa.countObjects += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
            CreateBeam(-90, 0, -5);
            CreateBeam(90, 0, 5);
        }
        if (basa.stats.Count > 2 && basa.stats[2].isTrigger)
        {
            basa.damage += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats.Count > 3 && basa.stats[3].isTrigger)
        {
            basa.lifeTime += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
        if (basa.stats.Count > 4 && basa.stats[4].isTrigger)
        {
            basa.skill.skill.stepMax -= basa.stats[4].value;
            basa.stats[4].isTrigger = false;
        }

        // Применяем множитель Steam один раз для этой сущности
        basa.damage = baseDamage * Mathf.Max(0f, player?.Steam ?? 1f);

        tick = basa.damageTickMax;

        // Масштаб круга
        objTransform.localScale = new Vector3(
            objTransform.localScale.x + basa.radius,
            objTransform.localScale.y + basa.radius,
            objTransform.localScale.z + basa.radius);
        if (bindTransform != null)
            bindTransform.localScale = objTransform.localScale;

        // Безопасное уничтожение (метод в базовом классе)
        CoroutineToDestroy(gameObject, basa.lifeTime);

        // Устанавливаем углы для всех активных лучей (используем реестр)
        SetupBeamAngles();
    }

    void SetupBeamAngles()
    {
        if (activeBeams.Count <= 1) return;

        // Распределяем углы по порядку появления, шаг 90 градусов
        int idx = activeBeams.IndexOf(this);
        if (idx < 0) idx = 0;
        float startAngle = -90f;
        for (int i = 0; i < activeBeams.Count; i++)
        {
            activeBeams[i].addToAndle = startAngle + i * 90f;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Вектор направления к курсору
        Vector3 direction = player.GetMousDirection(player.objTransform.position) * basa.radius;

        // Стационарное положение вокруг точки стрельбы
        var shootPoint = player.ShootPoint != null ? player.ShootPoint.transform.position : player.objTransform.position;
        objTransform.position = new Vector3(shootPoint.x, shootPoint.y, 5f);
        if (bindTransform != null)
        {
            bindTransform.position = objTransform.position;
        }

        // Поворот к курсору с офсетом
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        objTransform.rotation = Quaternion.AngleAxis(angle + addToAndle, Vector3.forward);
        if (bindTransform != null)
            bindTransform.rotation = objTransform.rotation;

        tick -= Time.deltaTime;
    }

    void HandleBarrel(Collider2D collision)
    {
        if (collision.TryGetComponent<ObjectHealth>(out var oh))
        {
            oh.TakeDamage();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            Damage(collision);
        }
        else if (collision.CompareTag("Barrel"))
        {
            HandleBarrel(collision);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (tick <= 0f)
            {
                tick = basa.damageTickMax;
                Damage(collision);
            }
        }
        else if (collision.CompareTag("Barrel"))
        {
            HandleBarrel(collision);
        }
    }

    void Damage(Collider2D collision)
    {
        if (collision == null) return;

        var objHealth = collision.GetComponent<FSMC_Executer>();
        var debuff = collision.GetComponent<ElementActiveDebuff>();

        // Безопасное применение дебаффа
        debuff?.ApplyEffect(status.Steam, 5);

        if (objHealth == null) return;

        float steamMult = debuff != null ? debuff.CurrentStatusValue(status.Steam) : 1f;
        float coldMult = debuff != null ? debuff.CurrentStatusValue(status.Cold) : 1f;
        coldMult = Mathf.Max(coldMult, 0.0001f); // защита от деления на ноль

        float finalDamage = (basa.damage * steamMult) / coldMult;
        objHealth.TakeDamage(finalDamage, damageMultiplier);

        GameManager.Instance.FindStatName("beamDamage", finalDamage);
    }

    private void CreateBeam(float angle, float x, float y)
    {
        // Клонируем компонент Beam (удобно и сохраняет иерархию)
        Beam a = Instantiate(this, new Vector2(objTransform.position.x + x, objTransform.position.y + y), Quaternion.identity);
        // Устанавливаем корректное базовое значение урона (используем baseDamage, чтобы не умножать Steam повторно)
        a.basa.damage = baseDamage * Mathf.Max(0f, player?.Steam ?? 1f);
        a.addToAndle = angle;
        a.transform.localScale = new Vector3(objTransform.localScale.x, objTransform.localScale.y, objTransform.localScale.z);
    }
}
