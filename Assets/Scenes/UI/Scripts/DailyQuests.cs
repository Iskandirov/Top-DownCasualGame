using System.Collections;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;

[Serializable]
public class Quest 
{
    public bool isActive;
    public bool isTodaysQuest;
    public bool isCompleteInOneRound;

    public string nameQuest;
    public string description;
    public int id;
    public float goal;
    public float progress;
    public float reward;
}

public class DailyQuests : MonoBehaviour
{
    public List<Quest> quest;
    static string path = "DailyQuests.txt";
    public TextMeshProUGUI TimeToNextRandom;
    const string API_URL = "http://worldtimeapi.org/api/ip";
    public List<TextMeshProUGUI> questText;
    public List<TextMeshProUGUI> questRewardText;
    public List<TextMeshProUGUI> questProgressText;
    public List<Image> questProgressFill;
    public List<Image> questImage;
    public List<Button> claimBtn;
    public TextMeshProUGUI money;
    public GameObject popUp;
    public SpriteRenderer popUpImg;
    public TextMeshProUGUI popUpTxt;
    DateTime currentDataTime = DateTime.Now;
    public GetScore score;
    public static DailyQuests instance;
    //private void OnLevelWasLoaded(int level)
    //{
    //    quest[4].progress = 0;
    //    quest[5].progress = 0;
    //    quest[6].progress = 0;
    //    SaveQuest(Path.Combine(Application.persistentDataPath, path));
    //}
    //private void OnApplicationQuit()
    //{
    //    quest[4].progress = 0;
    //    quest[5].progress = 0;
    //    quest[6].progress = 0;
    //    SaveQuest(Path.Combine(Application.persistentDataPath, path));
    //}
    private void Awake()
    {
        instance = this;
       
    }
    public void UpdateValue(int id,float value,bool isOverwrite)
    {
        Quest que;
        que = quest.FirstOrDefault(s => s.id.Equals(id));
        if (que != null && que.progress < que.goal)
        {
            if (!isOverwrite)
            {
                que.progress = que.progress + value >= que.goal ? que.goal : que.progress + value;
            }
            else
            {
                que.progress = value;
            }
            
            bool isQuestDone = que.progress / que.goal >= 1 && que.isActive ? true : false;
            if (popUp != null && !popUp.activeSelf && isQuestDone)
            {
                popUp.SetActive(isQuestDone);
                que.isActive = false;

                popUpImg.sprite = questImage.First(s => s.sprite.name == que.nameQuest).sprite;
                popUpTxt.text = que.description;
            }
            
            quest[que.id] = que;
            SaveQuest(Path.Combine(Application.persistentDataPath, path));
        }
    }
    private void Start()
    {
        string pathFile = Path.Combine(Application.persistentDataPath, path);
       
        if (!File.Exists(pathFile))
        {
            SaveQuest(pathFile);
        }
        //PlayerPrefs.DeleteAll();
        string lastTime = PlayerPrefs.GetString("LastGeneratedTime", "");
        DateTime lastRandomTime;
        if (!string.IsNullOrEmpty(lastTime))
        {
            lastRandomTime = DateTime.Parse(lastTime);
        }
        else
        {
            lastRandomTime = DateTime.MinValue;
        }
        if(DateTime.Today > lastRandomTime)
        {
            RandomGenerateQuest(pathFile);
        }
        TimeToNextRandom.text = GetTimeToNextRandom();

        LoadScore(pathFile);
        var questsToUpdate = quest.FindAll(s => s.isCompleteInOneRound);
        foreach (var q in questsToUpdate)
        {
            q.progress = 0;
        }
        SaveQuest(pathFile);
        SetQuestData();

    }
    public void SetQuestData()
    {
        int counter = 0;
        foreach (var q in quest)
        {
            if (q.isTodaysQuest)
            {
                questText[counter].text = q.description;
                questRewardText[counter].text = q.reward.ToString();

                Texture2D texture = Resources.Load<Texture2D>("Quest");
                Sprite[] sprites = Resources.LoadAll<Sprite>(texture.name);

                questImage[counter].sprite = Array.Find(sprites, sprite => sprite.name == q.nameQuest);
                questProgressText[counter].text = q.progress.ToString() + " / " + q.goal.ToString();
                questProgressFill[counter].fillAmount = q.progress / q.goal;
                bool isQuestDone = questProgressFill[counter].fillAmount == 1 && q.isActive ? true : false;
               
                //popUpAnim.SetBool("PopUp", isQuestDone);
                if (claimBtn.Count > 0)
                {
                    claimBtn[counter].interactable = isQuestDone;
                }
                counter++;
            }
        }
    }
    public void ClaimReward(int id)
    {
        if (int.TryParse(questRewardText[id].text, out int reward) && int.TryParse(money.text, out int moneyRes))
        {
            moneyRes += reward;
            money.text = moneyRes.ToString();
            score.SaveScore(moneyRes);
            score.LoadScore();
            quest[id].isActive = false;
            claimBtn[id].interactable = false;
            SaveQuest(Path.Combine(Application.persistentDataPath, path));
        }
    }
    public DateTime GetStartDateTime()
    {
        return currentDataTime;
    }
    public DateTime GetDateTimeNow()
    {
        StartCoroutine(GetRealDateTimeFromAPI());
        return currentDataTime;
    }
    IEnumerator GetRealDateTimeFromAPI()
    {
        UnityWebRequest webrequest = UnityWebRequest.Get(API_URL);
        yield return webrequest.SendWebRequest();

        if (webrequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + webrequest.error);
        }
        else
        {
            string timeDate = webrequest.downloadHandler.text;
            //Debug.Log(timeDate);
            currentDataTime = ParseDateTime(timeDate);
        }
    }
    DateTime ParseDateTime(in string dateTime)
    {
        string date = Regex.Match(dateTime, @"\d{4}-\d{2}-\d{2}").Value;
        string time = Regex.Match(dateTime, @"\d{2}:\d{2}:\d{2}\.\d{3}").Value;
        return DateTime.Parse(string.Format("{0} {1}", date, time));
    }
    private string GetTimeToNextRandom()
    {
        currentDataTime = GetDateTimeNow();
        int hours = Mathf.FloorToInt((float)(DateTime.Today.AddDays(1) - currentDataTime).TotalHours);
        int minutes = Mathf.FloorToInt((float)(DateTime.Today.AddDays(1) - currentDataTime).TotalMinutes) & 60;
        return hours + " hours and " + minutes + " minutes left to update quests";
    }
    void RandomGenerateQuest(in string path)
    {
        List<int> generatedNumbers = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            int randomNumber;
            do
            {
                randomNumber = UnityEngine.Random.Range(0, quest.Count);
            } while (generatedNumbers.Contains(randomNumber));

            generatedNumbers.Add(randomNumber);
        }

        int counter = 0;
        foreach (int index in generatedNumbers)
        {
            quest[index].isTodaysQuest = true;
            quest[index].isActive = true;
            quest[index].progress = 0;
            counter++;
        }
        SaveQuest(path);
        PlayerPrefs.SetString("LastGeneratedTime", GetDateTimeNow().ToString());
    }
    public void SaveQuest(in string path)
    {
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (var stat in quest)
            {
                string jsonData = JsonUtility.ToJson(stat);
                string decryptedJson = DataHashing.inst.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
        LoadScore(path);
    }
    public void LoadScore(in string path)
    {
        quest.Clear();
        string[] lines = File.ReadAllLines(path);

        foreach (string jsonLine in lines)
        {
            string decrypt = DataHashing.inst.Decrypt(jsonLine);

            Quest data = JsonUtility.FromJson<Quest>(decrypt);
            quest.Add(data);
        }
    }
}
