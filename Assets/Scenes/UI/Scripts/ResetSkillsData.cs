using System.IO;
using UnityEngine;

public class ResetSkillsData : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        DeleteFile(filePath);
    }

    private void DeleteFile(string filePath)
    {
        // �������� �������� �����
        if (File.Exists(filePath))
        {
            // ��������� �����
            File.Delete(filePath);
        }
    }
}
