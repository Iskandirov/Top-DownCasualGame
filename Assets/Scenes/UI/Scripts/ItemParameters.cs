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
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = itemImage;
    }
}
