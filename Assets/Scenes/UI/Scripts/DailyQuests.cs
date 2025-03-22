using System.Collections;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
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
[Serializable]
public class ApiResponse
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minute;
    public int seconds;
    public int milliSeconds;
    public string dateTime;
    public string date;
    public string time;
    public string timeZone;
    public string dayOfWeek;
    public bool dstActive;
}
public class DailyQuests : MonoBehaviour
{
    public List<Quest> quest;
    static string path = "DailyQuests.txt";
    public TextMeshProUGUI TimeToNextRandom;
    const string API_URL = "https://www.timeapi.io/api/Time/current/zone?timeZone=Europe/Kiev";
    public List<TextMeshProUGUI> questText;
    public List<TextMeshProUGUI> questRewardText;
    public List<TextMeshProUGUI> questProgressText;
    public List<Image> questProgressFill;
    public List<Image> questImage;
    public List<Image> questImageDone;
    public TextMeshProUGUI money;
    public GameObject popUp;
    public SpriteRenderer popUpImg;
    public TextMeshProUGUI popUpTxt;
    public DateTime currentDataTime = DateTime.Now;
    public GetScore score;
    public static DailyQuests instance;
    public Animator anim;
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
    public void UpdateValue(int id,float value,bool isOverwrite,bool mustBeMoreThanGoal)
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
            bool isQuestDone;
            if (mustBeMoreThanGoal)
            {
                isQuestDone = que.progress / que.goal >= 1 && que.isActive ? true : false;

            }
            else
            {
                isQuestDone = que.progress / que.goal >= 1 && que.isActive ? false : true;
            }
            if (popUp != null && !popUp.activeSelf && isQuestDone)
            {
                popUp.SetActive(isQuestDone);
                que.isActive = false;
                
                popUpImg.sprite = questImage.First(s => s.sprite.name == que.nameQuest).sprite;
                popUpTxt.text = que.description;
                ClaimReward((int)que.reward);
                //quest[id].isActive = false;

            }

            quest[que.id] = que;
            SaveQuest(Path.Combine(Application.persistentDataPath, path));
            SetQuestData();
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
    public void AnimationPopUp()
    {
        anim.SetTrigger("PoP");
    }
    public void SetQuestData()
    {
        int counter = 0;
        foreach (var q in quest)
        {
            if (q.isTodaysQuest)
            {
                questText[counter].GetComponent<TagText>().tagText = q.description;
                questRewardText[counter].text = q.reward.ToString();

                Sprite[] sprites = GameManager.ExtractSpriteListFromTexture("Quest");

                questImage[counter].sprite = Array.Find(sprites, sprite => sprite.name == q.nameQuest);
                questImage[counter].SetNativeSize();
                questProgressText[counter].text = q.progress.ToString() + " / " + q.goal.ToString();
                questProgressFill[counter].fillAmount = q.progress / q.goal;
                bool isQuestDone = q.isTodaysQuest && !q.isActive ? true : false;
                questImageDone[counter].gameObject.SetActive(isQuestDone);
                counter++;
            }
        }
    }
    public void ClaimReward(int reward)
    {
        Debug.Log(reward);
        GetScore score = FindAnyObjectByType<GetScore>();
        int moneyRes = reward + score.LoadScore();
        //money.text = moneyRes.ToString();
        score.SaveScore(moneyRes);
        score.LoadScore();
        //claimBtn[id].interactable = false;
        SaveQuest(Path.Combine(Application.persistentDataPath, path));
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
        try
        {
            if (webrequest.result == UnityWebRequest.Result.Success)
            {
                if (webrequest.downloadHandler != null)
                {
                    string jsonResponse = webrequest.downloadHandler.text;
                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(jsonResponse);
                        if (apiResponse != null)
                        {
                            currentDataTime = new DateTime(apiResponse.year, apiResponse.month, apiResponse.day, apiResponse.hour, apiResponse.minute, apiResponse.seconds, apiResponse.milliSeconds);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception caught: " + ex.Message);
        }
    }
    DateTime ParseDateTime(string dateTime)
    {
        dateTime = dateTime.Replace(" ", "");
        //"datetime": "2024-10-08T00:23:02.023202+03:00",
        string date = Regex.Match(dateTime, @"\d{4}-\d{2}-\d{2}").Value;
        string time = Regex.Match(dateTime, @"\d{2}:\d{2}:\d{2}").Value;
        return DateTime.Parse($"{date} {time}");
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
