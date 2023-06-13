using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EquipItem : MonoBehaviour
{
    SavedEquipData equipedItenms;
    public List<string> updatedList;

    // Start is called before the first frame update
    void Start()
    {
        equipedItenms = new SavedEquipData();
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (!File.Exists(path) && equipedItenms.Name == null)
        {
            SaveEquip();
        }

        updatedList = new List<string>(); // Ініціалізуємо список updatedList
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
                            obj.button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Зняти";
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
                   
                    SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(jsonLine);
                    if (data.Tag != item.GetComponent<SetParametersToitem>().Tag)
                    {
                        updatedList.Add(jsonLine);
                    }
                    MoveItem[] equips = FindObjectsOfType<MoveItem>();
                    foreach (var obj in equips)
                    {
                        if (obj.GetComponent<SetParametersToitem>().ItemName == item.GetComponent<SetParametersToitem>().ItemName)
                        {
                            obj.button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Обладнати";
                            obj.isEquipedNow = false;
                        }
                    }
                }

                // Записуємо оновлений масив рядків в файл після завершення циклу foreach
                File.WriteAllLines(path, updatedList.ToArray());
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
        writer.WriteLine(jsonData);
        writer.Close();
    } 
    private void SaveEquip()
    {
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        File.WriteAllText(path, String.Empty);
    }
}
