using TMPro;
using UnityEngine;

public class UsePotion : MonoBehaviour
{
    public KeyCode uningButton;
    public PotionsType type;
    public float callDown;
    public TextMeshProUGUI refreshTime;
    public float callDownMax;

    [System.Obsolete]
    private void Update()
    {
        if (Input.GetKeyDown(uningButton) && callDown <= 0)
        {
            PlayerManager.instance.UsePotion(type);
            callDown = callDownMax;
        }
        else
        {
            callDown -= Time.deltaTime;
            refreshTime.text = callDown.ToString("0");

        }
        if (callDown <= 0)
        {
            refreshTime.text = " ";
        }
    }
}
