using UnityEngine;

public class UsePotion : MonoBehaviour
{
    public KeyCode uningButton;
    public PotionsType type;
    public float callDown;
    public float callDownMax;
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
        }
    }
}
