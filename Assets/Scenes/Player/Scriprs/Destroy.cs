using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float lifeTime;
    //public Sprite[] lights;
    public AudioSource audioFlash;
    public AudioClip audioClip;
    public GameObject player;
    SpriteRenderer objSprite;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioFlash = player.GetComponent<AudioSource>();
        //audioFlash.PlayOneShot(audioClip);

        objSprite = GetComponent<SpriteRenderer>();

        StartCoroutine(TimerSpell());

    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);

    }
    // Update is called once per frame
    void Update()
    {
        //objSprite.sprite = lights[Random.Range(0, lights.Length)];
    }
}
