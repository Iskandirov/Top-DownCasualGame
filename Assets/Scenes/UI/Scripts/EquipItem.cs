using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EquipItem : MonoBehaviour
{
    SavedEquipData equipedItenms;
    public List<string> updatedList;
    DataHashing hashing;
    // Start is called before the first frame update
    void Start()
    {
        hashing = FindObjectOfType<DataHashing>();
        equipedItenms = new SavedEquipData();
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (!File.Exists(path) && equipedItenms.Name == null)
        {
            SaveEquip();
        }

        updatedList = new List<string>(); // ���������� ������ updatedList
    }

    public void onEquip()
    {
        MoveItem item = gameObject.transform.parent.GetChild(0).GetComponentInChildren<MoveItem>();
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (File.Exists(path))
        {
            string[] jsonLines = File.ReadAllLines(path);

            if (item.isEquipedNow == false)
            {
                if (jsonLines.Length < 3)
                {
                    item.SetItem(equipedItenms);
                    SaveEquip(equipedItenms);
                    MoveItem[] equips = FindObjectsOfType<MoveItem>();
                    foreach (var obj in equips)
                    {
                        if (obj.GetComponent<SetParametersToitem>().ItemName == item.GetComponent<SetParametersToitem>().ItemName)
                        {
                            obj.button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "�����";
                            obj.isEquipedNow = true;
                        }
                    }
                }
            }
            else if (item.isEquipedNow == true)
            {
                updatedList.Clear();
                foreach (string jsonLine in jsonLines)
                {
                    string decrypt = hashing.Decrypt(jsonLine);
                    SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(decrypt);
                    if (data.Tag != item.GetComponent<SetParametersToitem>().Tag)
                    {
                        updatedList.Add(jsonLine);
                    }
                    MoveItem[] equips = FindObjectsOfType<MoveItem>();
                    foreach (var obj in equips)
                    {
                        if (obj.GetComponent<SetParametersToitem>().ItemName == item.GetComponent<SetParametersToitem>().ItemName)
                        {
                            obj.button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "���������";
                            obj.isEquipedNow = false;
                        }
                    }
                }

                // �������� ��������� ����� ����� � ���� ���� ���������� ����� foreach
                string jsonContent = string.Join("\n", updatedList.ToArray());
                string encryptedJson = hashing.Encrypt(jsonContent);
                File.WriteAllText(path, encryptedJson);
            }

        }
    }
    private void SaveEquip(SavedEquipData equip)
    {
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        StreamWriter writer = new StreamWriter(path, true);

        SavedEquipData data = new SavedEquipData();
        data.Stat = equip.Stat;
        data.Level = equip.Level;
        data.Name = equip.Name;
        data.Tag = equip.Tag;

        string jsonData = JsonUtility.ToJson(data);
        string decryptedJson = hashing.Encrypt(jsonData);
        writer.WriteLine(decryptedJson);
        writer.Close();
    }
    private void SaveEquip()
    {
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        string decryptedJson = hashing.Encrypt(String.Empty);
        File.WriteAllText(path, decryptedJson);
    }
}
