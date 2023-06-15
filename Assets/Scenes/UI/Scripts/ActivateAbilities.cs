using UnityEngine;
using UnityEngine.UI;

public class ActivateAbilities : MonoBehaviour
{
    public Image[] abilities;
    public GameObject[] abilitiesObj;
    public int countActiveAbilities;
    // Start is called before the first frame update
    void Start()
    {
        countActiveAbilities = 1;
    }
}
