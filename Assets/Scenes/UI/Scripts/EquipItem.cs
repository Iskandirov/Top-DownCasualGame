using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class EquipItem : MonoBehaviour
{
    SavedEquipData equipedItenms;
    public List<SavedEquipData> updatedList;
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

        updatedList = new List<SavedEquipData>();
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
                    string decrypt = hashing.Decrypt(jsonLine);
                    SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(decrypt);
                    if (data.Tag != item.GetComponent<SetParametersToitem>().Tag)
                    {
                        updatedList.Add(data);
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
                SaveUpdateEquip(path, updatedList);
            }
        }
    }
    public void SaveUpdateEquip(string path, List<SavedEquipData> list)
    {
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (SavedEquipData line in list)
            {
                string jsonData = JsonUtility.ToJson(line);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();

        }
    }
    private void SaveEquip(SavedEquipData equip)
    {
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        using (StreamWriter writer = new StreamWriter(path, true))
        {
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
    }
    private void SaveEquip()
    {
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        File.Create(path);
    }
}
