using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WriteReadFile : MonoBehaviour
{
    public static void Read<T>(string path, List<T> classReturn)
    {
        classReturn.Clear();
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string jsonLine in lines)
            {
                string decrypt = DataHashing.inst.Decrypt(jsonLine);

                T data = JsonUtility.FromJson<T>(decrypt);
                classReturn.Add(data);
            }
        }
    }
    public static void Write<T>(string path, List<T> dataList)
    {
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (var stat in dataList)
            {
                string jsonData = JsonUtility.ToJson(stat);
                string decryptedJson = DataHashing.inst.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
        Read(path, dataList);
    }
}