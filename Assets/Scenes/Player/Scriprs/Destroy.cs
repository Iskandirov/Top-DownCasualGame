using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float timeToDestroy;
    public Sprite[] lights;
    public AudioSource audioFlash;
    public AudioClip audioClip;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioFlash = player.GetComponent<AudioSource>();
        audioFlash.PlayOneShot(audioClip);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = lights[Random.Range(0, lights.Length)];
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }
}
