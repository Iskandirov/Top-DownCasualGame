using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public bool IsSettingsPage;
    public bool IsGamePage;
    public static GameManager Instance;

    [Header("Panel manager")]
    public GameObject menuPanel;
    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject levelPanel;
    public GameObject enemyInfoPanel;
    public GameObject QuestPanel;
    [HideInInspector]
    public bool isPanelOpen;

    [Header("Language setting")]
    public List<Statistic> statRead;
    public List<GameObject> texts;
    [SerializeField]
    public List<LocalizationDataItems> localizedText;
    public List<string> language = new List<string>();
    [SerializeField]
    private List<LocalizationDataItems> locDataItems = new List<LocalizationDataItems>();
    public List<SettingsData> dataSett = new List<SettingsData>();
    public string loc = "eng";
    
    public float volumeMusic;
    public float volumeSFX;
    public float vSync;
    public string nameClip;

    [Header("Items settings")]
    [SerializeField]
    private List<SavedObjectData> items = new List<SavedObjectData>();
    public List<SavedObjectData> itemsRead = new List<SavedObjectData>();

    [Header("KillCount settings")]
    public int score;
    public int enemyCount;
    public TextMeshProUGUI fpsText;
    float deltaTime = 0.0f;
    float updateInterval = 1.0f; // Інтервал оновлення в секундах
    float lastUpdateTime = 0.0f;
    [Header("Enemy info")]
    public List<SaveEnemyInfo> enemyInfo;
    public List<SaveEnemyInfo> enemyInfoLoad;
    public GameObject InfoFiller;
    public Image InfoImg;
    public TextMeshProUGUI InfoName;
    public List<Sprite> ElementsImg;

    SaveEnemyInfo ShowedEnemy;
   
    public GameObject InfoPanel;
    public Image InfoImgPanel;
    public TextMeshProUGUI InfoNamePanel;
    public TextMeshProUGUI InfoStatHealtPanel;
    public TextMeshProUGUI InfoStatDamagePanel;
    public TextMeshProUGUI InfoStatMoveSpeedPanel;

    [Header("CharacterBuy settings")]
    [SerializeField]
    private List<SavedCharacterData> characters = new List<SavedCharacterData>();
    public List<SavedCharacterData> charactersRead = new List<SavedCharacterData>();
    public bool isShopingScene;
    public List<CharacterInfo> info;
    DataHashing hashing;
    Settings settings;
    [SerializeField] DailyQuests quest;
    [SerializeField] TextMeshProUGUI resolutionTxt;
    GameObject a;
    [Header("Player start settings")]
    public Image spriteCD;
    public Image baseSkillImg;
    public TextMeshProUGUI text;
    public PlayerManager player;
    public Image fullFillImage;
    public Image autoimage;
    public VisualEffect AutoActiveCurve;
    public EnemySpawner enemies;
    public Image expiriencepoint;
    //public Timer time;
    [SerializeField] public List<UsePotion> potionsObj;
    [SerializeField] List<GameObject> heroes;
    public Transform heroParent;
    public CinemachineVirtualCamera virtCam;

    void Awake()
    {
        Instance = this;
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("Character"))
        {
            PlayerPrefs.SetInt("Character", 1);
        }
        DeleteFile();
        hashing = DataHashing.inst;
        settings = Settings.instance;
        LoadCharacktersOnStart();

        if (!isShopingScene)
        {
            GameObject a = Instantiate(heroes[0], heroParent.position, Quaternion.identity, heroParent);
            virtCam.Follow = a.transform;
            player = PlayerManager.instance;
            baseSkillImg.sprite = ExtractSpriteListFromTexture("skills").First(s => s.name == charactersRead.Find(c => c.isEquiped).spell);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        LoadScore();
        StartLoad();

        AudioStartLoad();

        ItemsStartLoad();
        StartEnemyInfoLoad();
        foreach (var lang in dataSett)
        {
            if (lang.key == "language")
            {
                loc = lang.value;
            }
        }
        UpdateText(texts);
        score = 0;
    }
    class ResolutionRes 
    {
        public int width;
        public int height;
        public RefreshRate frameRate;
    }

    public static void SaveResolution(Resolution resolution)
    {
        ResolutionRes res = new ResolutionRes();
        res.width = resolution.width;
        res.height = resolution.height;
        res.frameRate = resolution.refreshRateRatio;

        string json = JsonUtility.ToJson(res);
        PlayerPrefs.SetString("Resolution", json);
    }
    public static Resolution LoadResolution()
    {
        string json;
        if (PlayerPrefs.HasKey("Resolution"))
        {
            json = PlayerPrefs.GetString("Resolution");
        }
        else
        {
            json = JsonUtility.ToJson(Screen.currentResolution);
        }
        ResolutionRes newRes = JsonUtility.FromJson<ResolutionRes>(json);
        Resolution newResolution = new Resolution();
        newResolution.width = newRes.width;
        newResolution.height = newRes.height;
        newResolution.refreshRateRatio = newRes.frameRate;
        return newResolution;
    }
    public static Sprite[] ExtractSpriteListFromTexture(string textureName)
    {
        Texture2D texture = Resources.Load<Texture2D>(textureName);
        return Resources.LoadAll<Sprite>(texture.name);
    }
    private void OnLevelWasLoaded(int level)
    {
        Resolution resolution = LoadResolution();
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        Time.timeScale = 1f;
    }
    private void FixedUpdate()
    {
        if (!IsSettingsPage && IsGamePage && !PlayerManager.instance.isTutor)
        {
            ShowLevel();
            Timer();
        }
        if (IsGamePage)
        {
            text.text = player.baseSkillCD.ToString("0.");
            spriteCD.fillAmount = player.baseSkillCD / player.baseSkillCDMax;
            if (player.baseSkillCD <= 0)
            {
                text.gameObject.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
            }
        }
    }
    private void Update()
    {
        if (resolutionTxt != null)
        {
            resolutionTxt.text = Screen.currentResolution.ToString();

        }
        if (!IsSettingsPage)
        {
            //can`t press pause after lose
            if (Input.GetKeyUp(KeyCode.Escape) && PlayerManager.instance.playerHealthPoint > 0)
            {
                if (!isPanelOpen)
                {
                    OpenPanel(menuPanel,true);
                    TimeScale(0);
                }
                else if(menuPanel.activeSelf)
                {
                    ClosePanel(menuPanel);
                    TimeScale(1);
                }
            }
        }
    }
    
    public void AudioStartLoad()
    {
        AudioManager music = AudioManager.instance;
        if (music.nameClip != nameClip && nameClip != null)
        {
            music.PlayMusic(nameClip);
            music.nameClip = nameClip;

        }
        if (music != null)
        {
            music.volumeMusic = volumeMusic;
        }
    }
    //Panels
    void DestroyVFX()
    {
        Destroy(a);
        OpenPanel(levelPanel, false);
        TimeScale(0);
    }
    public void ShowLevel()
    {
        PlayerManager player = PlayerManager.instance;
        if (expiriencepoint.fillAmount >= 1)
        {
            a = Instantiate(player.levelUpgradeEffect, player.objTransform.position, Quaternion.identity, player.objTransform);
            expiriencepoint.fillAmount = 0;
            player.level += 1;
            player.expNeedToNewLevel += player.expNeedToNewLevel * 0.4f;
            Invoke("DestroyVFX",1.5f);
        }
    }
    public void TimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
    public void OpenPanel(GameObject panel,bool questOpen)
    {
        panel.SetActive(true);
        isPanelOpen = true;
        if (questOpen)
        {
            QuestPanel.SetActive(true);
            quest.SetQuestData();
        }

    }
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        isPanelOpen = true;

    }
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        isPanelOpen = false;
        if (QuestPanel != null)
        {
            QuestPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }
    //Load
    public void LoadScore()
    {
        statRead.Clear();
        string path = Path.Combine(Application.persistentDataPath, "Statistic.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string jsonLine in lines)
            {
                string decrypt = hashing.Decrypt(jsonLine);

                Statistic data = JsonUtility.FromJson<Statistic>(decrypt);
                statRead.Add(data);
            }
        }
    }
    public void FindStatName(string name, float value)
    {
        foreach (Statistic stat in statRead)
        {
            if (stat.Name == name)
            {
                stat.stat += value;
            }
        }
    }
    //Language
    public TagText FindMyComponentInChildren(GameObject parentObject, string tag)
    {
        TagText component = parentObject.GetComponent<TagText>();

        if (component != null)
        {
            component.tagText = tag;
            texts.Add(component.gameObject);
            return component;
        }

        foreach (Transform child in parentObject.transform)
        {
            component = FindMyComponentInChildren(child.gameObject, tag);

            if (component != null)
            {
                component.tagText = tag;
                texts.Add(component.gameObject);
                return component;
            }
        }

        return null;
    }
    public void UpdateText(List<GameObject> text)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Localization.txt");

        if (File.Exists(filePath))
        {
            foreach (var lang in dataSett)
            {
                if (lang.key == "language")
                {
                    loc = lang.value;
                }
            }
           
            foreach (var item in localizedText)
            {
                foreach (var oneofmany in text)
                {
                    if (oneofmany.GetComponent<TagText>().tagText == item.key && loc == item.language)
                    {
                        oneofmany.GetComponent<TextMeshProUGUI>().text = item.value;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }
    public void ChangeSetting(List<string> value)
    {
        List<SettingsData> list = new List<SettingsData>();
        string path = Path.Combine(Application.persistentDataPath, "Settings.txt");
        if (File.Exists(path))
        {
            // Зчитування всіх рядків з файлу
            string[] lines = File.ReadAllLines(path);
            foreach (string item in lines)
            {
                string decryptedJson = hashing.Decrypt(item);
                SettingsData settings = JsonUtility.FromJson<SettingsData>(decryptedJson);
                if (settings.key != value[0])
                {
                    list.Add(settings);
                }
            }
        }
        // Створення нового елемента з ключем language та його значенням
        SettingsData data = new SettingsData();
        data.key = value[0];
        data.value = value[1];
        list.Add(data);
        bool hasLanguage = false;
        foreach (var item in list)
        {
            if (item.key == value[0])
            {
                if (hasLanguage)
                {
                    list.Remove(item);
                }
                else
                {
                    hasLanguage = true;
                }
            }
        }
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            // Запис усіх елементів зі списку до файлу
            foreach (var item in list)
            {
                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }


        // Перезавантаження налаштувань та оновлення тексту
        LoadSettings();
        UpdateText(texts);
    }
    public void StartLoad()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Localization.txt");
        string filePathSett = Path.Combine(Application.persistentDataPath, "Settings.txt");
        if (File.Exists(filePath))
        {
            LoadLocalizedText();
        }
        else
        {
            SaveLocalization();
        }
        if (File.Exists(filePathSett))
        {
            LoadSettings();
        }
        else
        {
            SaveWithDefaultParams();
        }
    }
    public void LoadSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Settings.txt");
        if (File.Exists(filePath))
        {
            dataSett.Clear();
            string[] encryptedText = File.ReadAllLines(filePath);

            foreach (string item in encryptedText)
            {
                // Розшифрувати JSON рядок
                string decrypt = hashing.Decrypt(item);
                
                SettingsData data = JsonUtility.FromJson<SettingsData>(decrypt);
                SettingsData settings = new SettingsData();
                settings.key = data.key;
                settings.value = data.value;
                dataSett.Add(settings);
                if (settings.key == "language" && IsSettingsPage == true)
                {
                    loc = settings.value;

                    UpdateText(texts);
                }
                if (settings.key == "volume")
                {
                    if (float.TryParse(settings.value, out float res))
                    {
                        
                        volumeMusic = res;
                        AudioManager.instance.volumeMusic = volumeMusic;
                        AudioManager.instance.musicObj.volume = volumeMusic;
                    }
                }
                if (settings.key == "sfx")
                {
                    if (float.TryParse(settings.value, out float res))
                    {
                        volumeSFX = res;
                        AudioManager.instance.volumeSFX = volumeSFX;
                        AudioManager.instance.sfxObj.volume = volumeSFX;
                    }
                }
                if (settings.key == "v-sync")
                {
                    if (float.TryParse(settings.value, out float res))
                    {
                        vSync = res;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }
    public void LoadLocalizedText()
    {
        localizedText = new List<LocalizationDataItems>();
        string filePath = Path.Combine(Application.persistentDataPath, "Localization.txt");
        if (File.Exists(filePath))
        {
            Task<string[]> task = File.ReadAllLinesAsync(filePath);

            // Wait for the task to complete
            task.Wait();

            // Get the result of the task
            string[] lines = task.Result;
            // Розділяємо розшифрований текст на окремі рядки
            foreach (string item in lines)
            {

                string decrypt = hashing.Decrypt(item);
                LocalizationDataItems data = JsonUtility.FromJson<LocalizationDataItems>(decrypt);
                LocalizationDataItems dataItem = new LocalizationDataItems();
                dataItem.language = data.language;
                dataItem.key = data.key;
                dataItem.value = data.value;

                if (!language.Contains(dataItem.language))
                {
                    language.Add(dataItem.language);
                }
                localizedText.Add(dataItem);
            }
        }
    }
    public void SaveLocalization()
    {
        string path = Path.Combine(Application.persistentDataPath, "Localization.txt");
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            foreach (LocalizationDataItems item in locDataItems)
            {
                LocalizationDataItems data = new LocalizationDataItems();
                data.language = item.language;
                data.key = item.key;
                data.value = item.value;
                string jsonData = JsonUtility.ToJson(data);
                string encryptedJson = hashing.Encrypt(jsonData);

                writer.WriteLine(encryptedJson);
            }

            writer.Close();
        }
        LoadLocalizedText();

    }
    public void SaveWithDefaultParams()
    {
        List<SettingsData> list = new List<SettingsData>();
        string path = Path.Combine(Application.persistentDataPath, "Settings.txt");
        // Додавання елементів до списку
        list.Add(new SettingsData { key = "language", value = "eng" });
        list.Add(new SettingsData { key = "volume", value = "0,25" });
        list.Add(new SettingsData { key = "sfx", value = "0,25" });
        list.Add(new SettingsData { key = "v-sync", value = "0" });
        // Відкриття файлу для запису
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            // Запис усіх елементів зі списку до файлу
            foreach (var item in list)
            {
                string jsonData = JsonUtility.ToJson(item);
                string encryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(encryptedJson);
            }
            // Закриття файлу
            writer.Close();
        }

        // Перезавантаження налаштувань та оновлення тексту
        LoadSettings();
        UpdateText(texts);
    }
    //Items
    public void ItemsStartLoad()
    {
        string path = Path.Combine(Application.persistentDataPath, "ItemInventory.txt");
        if (!File.Exists(path))
        {
            SaveInventory();
        }
        path = Path.Combine(Application.persistentDataPath, "UpgradeImage.txt");
        if (!File.Exists(path))
        {
            //SaveUpgrade();
        }
        LoadInventory(itemsRead);
    }
    public void LoadInventory(List<SavedObjectData> itemsRead)
    {
        string path = Path.Combine(Application.persistentDataPath, "ItemInventory.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                // Розшифрувати JSON рядок
                string decryptedJson = hashing.Decrypt(jsonLine);

                SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(decryptedJson);

                data.ImageSprite = ExtractSpriteListFromTexture("items").First(d => d.name == data.Name);

                //data.RareSprite = Resources.Load<Sprite>(data.RareName + " " + data.Level);
                itemsRead.Add(data);
            }
        }
        else
        {
            File.Create(path);
        }
    }
    private void SaveInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "ItemInventory.txt");
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            SavedObjectData data = new SavedObjectData();
            foreach (SavedObjectData item in items)
            {
                data.Name = item.Name;
                data.IDRare = item.IDRare;
                data.RareName = item.RareName;
                data.Stat = item.Stat;
                data.Level = item.Level;
                data.Price = item.Price;
                data.Tag = item.Tag;
                data.RareTag = item.RareTag;
                data.Description = item.Description;

                string jsonData = JsonUtility.ToJson(data);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    private void DeleteFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        // Перевірка наявності файлу
        if (File.Exists(filePath))
        {
            // Видалення файлу
            File.Delete(filePath);
        }
    }
    //KillCount
    public void Timer()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        // Перевірка, чи настав час оновлення виводу FPS
        if (Time.time - lastUpdateTime > updateInterval)
        {
            float fps = 1.0f / Time.deltaTime;
            fpsText.text = "FPS: " + fps.ToString("0.");
            lastUpdateTime = Time.time;
        }
    }
    public int LoadObjectLevelCount(int objectID)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string jsonLine in lines)
            {
                string decrypt = hashing.Decrypt(jsonLine);
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(decrypt);

                if (data.IDLevel == objectID)
                {
                    return data.countOfCount;
                }
            }
        }

        return 0;
    }
    //Enemy info
    public void StartEnemyInfoLoad()
    {
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
                    InfoImg.sprite = ExtractSpriteListFromTexture("enemy").First(e => e.name == item.Name + "_" + item.ID);
                    InfoName.text = item.Name;
                    ShowedEnemy.Name = item.Name;
                    ShowedEnemy.MoveSpeed = item.MoveSpeed;
                    ShowedEnemy.Health = item.Health;
                    ShowedEnemy.Damage = item.Damage;

                    InfoImgPanel.sprite = ExtractSpriteListFromTexture("enemy").First(e => e.name == item.Name + "_" + item.ID);
                    InfoNamePanel.text = ShowedEnemy.Name.ToString();
                    InfoStatDamagePanel.text = ShowedEnemy.Damage.ToString();
                    InfoStatHealtPanel.text = ShowedEnemy.Health.ToString();
                    InfoStatMoveSpeedPanel.text = ShowedEnemy.MoveSpeed.ToString();
                }
                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    public void SaveEnemyInfo()
    {
        string path = Path.Combine(Application.persistentDataPath, "EnemyInfo.txt");
        using (StreamWriter writer = new StreamWriter(path, true))
        {
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
        if (!isPanelOpen)
        {
            OpenPanel(InfoPanel,false);
            TimeScale(0);
            InfoImgPanel.sprite = InfoImg.sprite;
            InfoNamePanel.text = ShowedEnemy.Name.ToString();
            InfoStatHealtPanel.text = ShowedEnemy.Health.ToString();
            InfoStatDamagePanel.text = ShowedEnemy.Damage.ToString();
            InfoStatMoveSpeedPanel.text = ShowedEnemy.MoveSpeed.ToString();
        }
    }
    public void CloseEnemyInfoPanel()
    {
        ClosePanel(InfoPanel);
        InfoFiller.SetActive(false);
        InfoImg.gameObject.SetActive(false);
        InfoName.gameObject.SetActive(false);
    }
    //Buy Character
    public void LoadCharacktersOnStart()
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        if (!File.Exists(path))
        {
            SaveCharacterInventory();
        }
        SetParameters();
    }
    public void LoadInventory(List<SavedCharacterData> itemsRead)
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                // Розшифрувати JSON рядок
                string decryptedJson = hashing.Decrypt(jsonLine);

                SavedCharacterData data = JsonUtility.FromJson<SavedCharacterData>(decryptedJson);

                itemsRead.Add(data);
            }
        }
        else
        {
            File.Create(path);
        }
    }
    private void SaveCharacterInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (SavedCharacterData item in characters)
            {
                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    public void SaveCharacterUpgrade(int id)
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (SavedCharacterData item in charactersRead)
            {
                if (item.ID == id)
                {
                    item.isBuy = true;
                    item.status = "sold_out";
                }

                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    
    public void SetParameters()
    {
        charactersRead.Clear();
        LoadInventory(charactersRead);
        if (isShopingScene)
        {
            foreach (CharacterInfo character in info)
            {
                foreach (SavedCharacterData item in charactersRead)
                {
                    if (character.id == item.ID)
                    {
                        character.Name = item.Name;
                        //character.buyButton.tagText = item.status;
                        character.damage = int.Parse(item.damage);
                        character.health = int.Parse(item.health);
                        character.spell = item.spell;
                        character.attackSpeed = float.Parse(item.attackSpeed);
                        character.move = int.Parse(item.move);
                        character.description = item.description;
                        character.characterName.text = item.Name;
                        //if (item.isBuy == true)
                        {
                            character.check.SetActive(item.isBuy);
                            character.price = item.price;
                            //character.button.interactable = item.interactable;
                        }
                    }
                }
            }
            

        }
    }
    //Potion
   
}
[System.Serializable]
public class Statistic
{
    public int ID;
    public string Name;
    public float stat;
}
[System.Serializable]
public class LocalizationDataItems
{
    public string key;
    public string value;
    public string language;
}

[System.Serializable]
public class SettingsData
{
    public string key;
    public string value;
}
