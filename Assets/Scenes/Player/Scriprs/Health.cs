using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float playerHealthPoint;
    public float playerHealthPointMax;
    public float playerHealthRegeneration;
    public Image playerHealthPointImg;
    public RestartGame loseScene;
    public GameObject loseSceneParent;
    Animator objAnim;
    // Start is called before the first frame update
    void Awake()
    {
        objAnim = gameObject.GetComponent<Animator>();
        playerHealthPointMax = playerHealthPoint;
        playerHealthRegeneration = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealthRegeneration > 0 && playerHealthPointImg.fillAmount < 1)
        {
            playerHealthPoint += playerHealthRegeneration / 100;
            playerHealthPointImg.fillAmount = playerHealthPoint / playerHealthPointMax;
        }
    }
   
    public void HitEnd()
    {
        objAnim.SetBool("IsHit", false);
        if (playerHealthPoint <= 0)
        {
            Instantiate(loseScene, loseSceneParent.transform.position, Quaternion.identity, loseSceneParent.transform);
        }
    }
}
