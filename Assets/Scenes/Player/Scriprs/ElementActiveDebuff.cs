using FSMC.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Elements
{
    [SerializeField]
    public enum status
    {
        Fire,
        Electricity,
        Water,
        Dirt,
        Wind,
        Grass,
        Steam,
        Cold
    };
    [SerializeField]
    public List<float> statusCurrentData;
    public List<bool> isActiveCurrentData;
   
    public float CurrentStatusValue(status name)
    {
        return statusCurrentData[(int)name];
    }
    public void Debuff(FSMC_Executer enemy, status name)
    {
        switch (name)
        {
            case status.Fire:
                statusCurrentData[(int)status.Water] /= 2;
                //enemy.dama;
                break;
            case status.Electricity:
                statusCurrentData[(int)status.Cold] /= 2;
                break;
            case status.Water:
                statusCurrentData[(int)status.Fire] /= 2;
                break;
            case status.Dirt:
                statusCurrentData[(int)status.Steam] /= 2;
                break;
            case status.Wind:
                statusCurrentData[(int)status.Electricity] /= 2;
                break;
            case status.Grass:
                statusCurrentData[(int)status.Wind] /= 2;
                break;
            case status.Steam:
                statusCurrentData[(int)status.Fire] /= 2;
                statusCurrentData[(int)status.Water] /= 2;
                break;
            case status.Cold:
                statusCurrentData[(int)status.Dirt] /= 2;
                if (enemy != null)
                {
                    enemy.StateMachine.SetFloat("SlowTime",5f);
                    enemy.StateMachine.SetFloat("SlowPercent",.5f);
                    enemy.SetCurrentState("Slow");
                }
                break;
        }
        isActiveCurrentData[(int)name] = true;
    }
    public void DeactivateDebuff(FSMC_Executer enemy, status name)
    {
        switch (name)
        {
            case status.Fire:
                statusCurrentData[(int)status.Water] *= 2;
                //enemy.Damage(enemy.damage * 2);
                break;
            case status.Electricity:
                statusCurrentData[(int)status.Cold] *= 2;
                break;
            case status.Water:
                statusCurrentData[(int)status.Fire] *= 2;
                break;
            case status.Dirt:
                statusCurrentData[(int)status.Steam] *= 2;
                break;
            case status.Wind:
                statusCurrentData[(int)status.Electricity] *= 2;
                break;
            case status.Grass:
                statusCurrentData[(int)status.Wind] *= 2;
                break;
            case status.Steam:
                statusCurrentData[(int)status.Fire] *= 2;
                statusCurrentData[(int)status.Water] *= 2;
                break;
            case status.Cold:
                statusCurrentData[(int)status.Dirt] *= 2;
                break;
        }
        isActiveCurrentData[(int)name] = false;
    }
}
//��� ��������� ��'���� � ���� ��������� ������� ���������� ����� � ����� ����� ������, ����� ����������� �������� ����������� ������ � ���������� ���������� ������ ������
//������ ������ ���� �� ������ ��������, ���� �������
//��� ���������� ��������� ������� ����������� ���� ������, ��� ���������� �������� ��
public class ElementActiveDebuff : MonoBehaviour
{
    [Header("References")]
    public Elements elements;
    public Transform elementDebuffParent;
    public SpriteRenderer elementDebuffObject;

    private FSMC_Executer executer;
    private SpriteRenderer bodyRenderer;
    public List<Sprite> statusSprite;

    // ������ ������: ������ -> ������
    private readonly Dictionary<Elements.status, float> activeEffects = new();

    // ³������ �������: ������ -> ������
    //private readonly Dictionary<Elements.status, SpriteRenderer> debuffSprites = new();

    private static readonly List<Elements.status> toRemove = new(); // ��������� ������ ��� ��������� ��������

    private void Awake()
    {
        executer = GetComponent<FSMC_Executer>();
        bodyRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (activeEffects.Count == 0)
            return;

        float delta = Time.deltaTime;
        toRemove.Clear();

        foreach (var status in new List<Elements.status>(activeEffects.Keys))
        {
            activeEffects[status] -= delta;

            if (activeEffects[status] <= 0f)
                toRemove.Add(status);
        }

        foreach (var status in toRemove)
        {
            DeactivateEffect(status);
        }
    }

    public void ApplyEffect(Elements.status status, float duration)
    {
        int id = (int)status;

        if (activeEffects.TryGetValue(status, out float remainingTime))
        {
            // ���� ����� ��� �������� � ��������� ������, ���� ����� ������
            if (duration > remainingTime)
                activeEffects[status] = duration;

            return;
        }

        // �������� ����� �������������: ����� �� �� �������� + ��'��� �������
        if (!elements.isActiveCurrentData[id] && bodyRenderer.color.a > 0f)
        {
            elements.Debuff(executer, status);
            activeEffects[status] = duration;
            ActivateVisualEffect(status, id);
        }
    }

    private void ActivateVisualEffect(Elements.status status, int elementId)
    {
        // �������������, �� ������ ��������� �������  
        while (statusSprite.Count <= elementId)
            statusSprite.Add(null);

        if (statusSprite[elementId] == null)
        {
            var sr = Instantiate(elementDebuffObject, elementDebuffParent);
            sr.transform.localPosition = Vector3.zero; // ����� ������������  
            sr.sprite = GameManager.Instance.ElementsImg[elementId];
            statusSprite[elementId] = sr.sprite;
        }

        // ����������: Sprite �� ����� �������� enabled, ������� ����� �������� � �������� SpriteRenderer  
        var spriteRenderer = elementDebuffObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }

    private void DeactivateEffect(Elements.status status)
    {
        int id = (int)status;

        elements.DeactivateDebuff(executer, status);
        activeEffects.Remove(status);

        if (id < statusSprite.Count && statusSprite[id] != null)
        {
            var spriteRenderer = elementDebuffObject.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
        }
    }
}
