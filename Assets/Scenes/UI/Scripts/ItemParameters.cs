using System.IO;
using UnityEngine;
public class ItemParameters : MonoBehaviour
{
    public string itemName;
    public Sprite itemRare;
    public int idRare;
    public string itemRareName;
    public Sprite itemImage;
    public string Stat;
    public int Level;
    public int Count;
    public string Tag;
    public string RareTag;
    public string Description;
    [SerializeField]
    DataHashing hash;
    Timer dropLooted;
    public bool isTutor;
    // Start is called before the first frame update
    void Start()
    {
        hash = DataHashing.inst;
        dropLooted = FindObjectOfType<Timer>();
        GetComponent<SpriteRenderer>().sprite = itemImage;
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            ItemParameters objParam = GetComponent<ItemParameters>();
            // Створення об'єкта даних
            SavedObjectData data = new SavedObjectData();
            data.Name = objParam.itemName;
            data.IDRare = objParam.idRare;
            data.RareName = objParam.itemRareName;
            data.Stat = objParam.Stat;
            data.Level = objParam.Level;
            data.Tag = objParam.Tag;
            data.RareTag = objParam.RareTag;
            data.Description = objParam.Description;

            // Збереження даних у файл
            string fileName = Path.Combine(Application.persistentDataPath, "savedData.txt");
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                string jsonData = JsonUtility.ToJson(data);
                string decryptedJson = hash.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
                writer.Close();
            }
            if (!isTutor)
            {
                dropLooted.isDropLooted = true;
            }
            else
            {
                Tutor tut = FindObjectOfType<Tutor>();
                tut.PhasePlus();
                tut.BlockMoveAndShoot();
            }
            if (AudioManager.instance != null)
            {
                AudioManager.instance.MusicStop();
                AudioManager.instance.PlaySFX("Win");
            }
            Destroy(gameObject);
        }
    }
}
