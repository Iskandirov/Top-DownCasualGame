using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vortex : SkillBaseMono
{
    public List<Transform> movingObjects = new List<Transform>(); // ������ ������� ��'����
    public float bump;
    public bool isClone;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1.9f; // ������ Z-���������� ��� ��'����
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (!isClone)
        {
            transform.position = worldPosition;
            if (basa.stats[4].isTrigger)
            {
                Vortex b = Instantiate(this, new Vector3(transform.position.x + Random.Range(-30, 30), transform.position.y + Random.Range(-30, 30), 1.9f), Quaternion.identity);
                b.basa.lifeTime = basa.lifeTime;
                b.isClone = true;
            }
        }
        if (basa.stats[1].isTrigger)
        {
            basa.lifeTime += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.radius += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.stepMax -= basa.stats[3].value;
            basa.skill.skillCD -= StabilizateCurrentReload(basa.skill.skillCD, basa.stats[2].value);
            basa.stats[3].isTrigger = false;
        }

        transform.localScale = new Vector2(basa.radius * PlayerManager.instance.Steam, basa.radius * PlayerManager.instance.Steam);


        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        DamageDeal();
        Destroy(gameObject);
    }
    void DamageDeal()
    {
        foreach (Transform movingObject in movingObjects)
        {
            if (movingObject.GetComponentInChildren<HealthPoint>() != null)
            {
                HealthPoint health = movingObject.GetComponentInChildren<HealthPoint>();
                health.healthPoint -= (basa.damage * PlayerManager.instance.Wind * PlayerManager.instance.Steam) / (health.Wind * health.Steam);
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Transform movingObject in movingObjects)
        {
            Vector3 centerPosition = transform.position; // �������� ������� ������� ������������ ��'���� ���
            if (movingObject != null)
            {
                Vector3 offset = movingObject.position - centerPosition;

                float force = Mathf.Abs(offset.magnitude) * bump;

                // ���������� ���� �� �������� �� �������� �� ������ � ���� X
                float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

                // ���������� ������������� �������� ���� ����������
                float horizontalForce = force * Mathf.Cos(angle * Mathf.Deg2Rad);

                // ���������� ����������� �������� ���� ����������
                float verticalForce = force * Mathf.Sin(angle * Mathf.Deg2Rad);

                // ��������� ��������� ��������
                movingObject.position -= new Vector3(horizontalForce * Time.fixedDeltaTime, verticalForce * Time.fixedDeltaTime, 0);
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            if (collision.CompareTag("Enemy"))
            {
                Forward move = collision.GetComponentInParent<Forward>();
                if (move != null)
                {
                    if (!movingObjects.Contains(move.objTransform))
                    {
                        movingObjects.Add(move.objTransform);
                    }
                }
            }
            if (collision.CompareTag("Player"))
            {
                PlayerManager move = collision.GetComponent<PlayerManager>();
                if (move != null)
                {
                    if (!movingObjects.Contains(move.objTransform))
                    {
                        movingObjects.Add(move.objTransform);
                    }
                }
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            if (collision.CompareTag("Enemy"))
            {
                Forward move = collision.GetComponentInParent<Forward>();
                if (move != null)
                {
                    if (!movingObjects.Contains(move.objTransform))
                    {
                        int index = movingObjects.IndexOf(move.objTransform);
                        movingObjects.RemoveAt(index);
                    }
                }
            }
            if (collision.CompareTag("Player"))
            {
                PlayerManager move = collision.GetComponent<PlayerManager>();
                if (move != null)
                {
                    if (!movingObjects.Contains(move.objTransform))
                    {
                        movingObjects.Add(move.objTransform);
                    }
                }
            }
        }
    }
}
