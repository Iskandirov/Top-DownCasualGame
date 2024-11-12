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
    public Elements elements;

    public Transform elementDebuffParent;
    public SpriteRenderer elementDebuffObject;

    public IEnumerator EffectTime(Elements.status name, int lifeTime)
    {
        SpriteRenderer a = null;
        int elementId = (int)name;
        if (elements.isActiveCurrentData[(int)name] != true && GetComponentInChildren<SpriteRenderer>().color.a != 0)
        {
            elements.Debuff(GetComponent<FSMC_Executer>(), name);
            a = Instantiate(elementDebuffObject, elementDebuffParent.position, Quaternion.identity, elementDebuffParent);
            a.sprite = GameManager.Instance.ElementsImg[elementId];
            UnikeEffects(a);
        }
        yield return new WaitForSeconds(lifeTime);
        if (a != null)
        {
            elements.DeactivateDebuff(GetComponent<FSMC_Executer>(), name);
            Destroy(a.gameObject);
        }
    }
    private void UnikeEffects(SpriteRenderer sprite)
    {
        var uniqueChildren = new List<SpriteRenderer>();
        foreach (var child in elementDebuffParent.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (child.sprite != sprite.sprite)
            {
                uniqueChildren.Add(child);
            }
        }

        // ��������� ������������ ������� ��'����
        for (int i = 0; i < elementDebuffParent.transform.childCount - 1; i++)
        {
            if (!uniqueChildren.Contains(elementDebuffParent.transform.GetChild(i).GetComponent<SpriteRenderer>()))
            {
                Destroy(elementDebuffParent.transform.GetChild(i).gameObject);
            }
        }
    }
}
