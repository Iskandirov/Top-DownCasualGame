using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoLoader : MonoBehaviour
{
    public List<SaveEnemyInfo> enemyInfo;
    public List<SaveEnemyInfo> enemyInfoLoad;
    public DataHashing hashing;
    public GameObject InfoFiller;
    public SpriteRenderer InfoImg;
    public TextMeshProUGUI InfoName;

    SaveEnemyInfo ShowedEnemy;

    public GameObject InfoPanel;
    public Image InfoImgPanel;
    public TextMeshProUGUI InfoNamePanel;
    public TextMeshProUGUI InfoStatHealtPanel;
    public TextMeshProUGUI InfoStatDamagePanel;
    public TextMeshProUGUI InfoStatMoveSpeedPanel;

    Move PanelChecker;
    public void Start()
    {
        PanelChecker = FindObjectOfType<Move>();
        ShowedEnemy = new SaveEnemyInfo();
        string filePath = Path.Combine(Application.persistentDataPath, "EnemyInfo.txt");
        if (!File.Exists(filePath))
        {
            SaveEnemyInfo();
        }
        LoadEnemyInfo();
    }
    public bool CheckInfo(int id)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "EnemyInfo.txt");
        if (File.Exists(filePath))
        {
            string[] encryptedText = File.ReadAllLines(filePath);
            // Розділяємо розшифрований текст на окремі рядки
            foreach (string item in encryptedText)
            {
                string decrypt = hashing.Decrypt(item);
                SaveEnemyInfo data = JsonUtility.FromJson<SaveEnemyInfo>(decrypt);
                if (data.ID == id && !data.Showed)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void FillInfo(int id)
    {
        List<SaveEnemyInfo> info = new List<SaveEnemyInfo>();
        string filePath = Path.Combine(Application.persistentDataPath, "EnemyInfo.txt");

        // Відкриваємо потік для читання
        if (File.Exists(filePath))
        {
            string[] encryptedText = File.ReadAllLines(filePath);
            foreach (string item in encryptedText)
            {
                string decrypt = hashing.Decrypt(item);
                SaveEnemyInfo data = JsonUtility.FromJson<SaveEnemyInfo>(decrypt);
                info.Add(data);
            }
        }

        // Змінюємо дані і відкриваємо потік для запису
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            foreach (SaveEnemyInfo item in info)
            {
                if (item.ID == id && !item.Showed)
                {
                    item.Showed = true;
                    InfoFiller.SetActive(true);
                    InfoImg.gameObject.SetActive(true);
                    InfoName.gameObject.SetActive(true);
                    InfoImg.sprite = Resources.Load<Sprite>(item.Name + "_" + item.ID);
                    InfoName.text = item.Name;
                    ShowedEnemy.Name = item.Name;
                    ShowedEnemy.MoveSpeed = item.MoveSpeed;
                    ShowedEnemy.Health = item.Health;
                    ShowedEnemy.Damage = item.Damage;
                }
                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
        }
    }
    public void SaveEnemyInfo()
    {
        string path = Path.Combine(Application.persistentDataPath, "EnemyInfo.txt");
        StreamWriter writer = new StreamWriter(path, true);

        foreach (SaveEnemyInfo item in enemyInfo)
        {
            SaveEnemyInfo data = new SaveEnemyInfo();
            data.Name = item.Name;
            data.ID = item.ID;
            data.Attack = item.Attack;
            data.Health = item.Health;
            data.Damage = item.Damage;
            data.MoveSpeed = item.MoveSpeed;

            string jsonData = JsonUtility.ToJson(data);
            string encryptedJson = hashing.Encrypt(jsonData);

            writer.WriteLine(encryptedJson);
        }

        writer.Close();
    }
    public void LoadEnemyInfo()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "EnemyInfo.txt");
        if (File.Exists(filePath))
        {
            string[] encryptedText = File.ReadAllLines(filePath);
            // Розділяємо розшифрований текст на окремі рядки
            foreach (string item in encryptedText)
            {
                string decrypt = hashing.Decrypt(item);
                SaveEnemyInfo data = JsonUtility.FromJson<SaveEnemyInfo>(decrypt);
                data.Image = Resources.Load<Sprite>(data.Name + "_" + data.ID);
                enemyInfoLoad.Add(data);
            }
        }
    }
    public void OpenEnemyInfoCard()
    {
        if (!PanelChecker.otherPanelOpened)
        {
            InfoPanel.SetActive(true);
            InfoImgPanel.sprite = InfoImg.sprite;
            InfoNamePanel.text = ShowedEnemy.Name.ToString();
            InfoStatHealtPanel.text = ShowedEnemy.Health.ToString();
            InfoStatDamagePanel.text = ShowedEnemy.Damage.ToString();
            InfoStatMoveSpeedPanel.text = ShowedEnemy.MoveSpeed.ToString();
            Time.timeScale = 0f;
            PanelChecker.otherPanelOpened = true;
        }
    }
    public void ClosePanel()
    {
        InfoPanel.SetActive(false);
        InfoFiller.SetActive(false);
        InfoImg.gameObject.SetActive(false);
        InfoName.gameObject.SetActive(false);
        Time.timeScale = 1f;
        PanelChecker.otherPanelOpened = false;
    }
}
