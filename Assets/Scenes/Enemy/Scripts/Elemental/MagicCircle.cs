using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle : MonoBehaviour
{
    public bool TikTokBool;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("isBooled", TikTokBool);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
