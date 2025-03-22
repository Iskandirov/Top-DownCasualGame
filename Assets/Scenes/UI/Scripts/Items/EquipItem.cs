using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class EquipItem : MonoBehaviour
{
    SavedEquipData equipedItenms;
    public List<SavedEquipData> updatedList;
    [SerializeField] GameObject checker;
    DataHashing hashing;
    // Start is called before the first frame update
    void Start()
    {
        hashing = FindObjectOfType<DataHashing>();
        equipedItenms = new SavedEquipData();
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (!File.Exists(path) && equipedItenms.Name == null)
        {
            //WriteReadFile.Write(path, updatedList);
            Debug.Log(updatedList.Count);
            SaveEquip();
        }

        updatedList = new List<SavedEquipData>();
    }
    public void EnableInput()
    {
        MoveItem item = transform.parent.GetChild(0).GetComponentInChildren<MoveItem>();
        if (item != null)
        {
            string buttonText = item.isEquipedNow ? "equiped" : "equip";
            checker.SetActive(item.isEquipedNow);
            UpdateEquipPanel(item, buttonText, item.isEquipedNow);
        }
    }
    
    public void onEquip()
    {
        MoveItem item = transform.parent.GetChild(0).GetComponentInChildren<MoveItem>();
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        //WriteReadFile.Read(path, updatedList);
        if (File.Exists(path))
        {
            string[] jsonLines = File.ReadAllLines(path);
            foreach (var jsonLine in jsonLines)
            {
                string decrypt = hashing.Decrypt(jsonLine);
                SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(decrypt);
                if (data.Name == item.GetComponent<SetParametersToitem>().ItemName && data.Level.ToString() != item.GetComponent<SetParametersToitem>().level)
                {
                    updatedList.Add(data);
                }
            }
            if (item.isEquipedNow == false && jsonLines.Length < 3 && updatedList.Count == 0)
            {
                equipedItenms = item.SetItem();
                checker.SetActive(true);
                UpdateEquipPanel(item, "equiped", true);
                SaveEquip(equipedItenms);

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
                }
                UpdateEquipPanel(item, "equip", false);
                checker.SetActive(false);
                SaveUpdateEquip(path, updatedList);

            }
        }
    }
    private void UpdateEquipPanel(MoveItem item, string buttonText, bool isEquiped)
    {
        MoveItem[] equips = FindObjectsOfType<MoveItem>();
        foreach (var obj in equips)
        {
            if (obj.GetComponent<SetParametersToitem>().ItemName == item.GetComponent<SetParametersToitem>().ItemName 
                && obj.GetComponent<SetParametersToitem>().level == item.GetComponent<SetParametersToitem>().level)
            {
                obj.equipPanel.GetComponent<ItemData>().state.GetComponent<TagText>().tagText = buttonText;
                obj.isEquipedNow = isEquiped;
            }
        }
        GameManager.Instance.UpdateText(GameManager.Instance.texts);
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
