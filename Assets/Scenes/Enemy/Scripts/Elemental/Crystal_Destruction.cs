using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Destruction : MonoBehaviour
{
    public Sprite crushedSprite;
    public GameObject coreObj;
    Animator anim;
    public int health = 2;
    [SerializeField] PuzzleController puzzle;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health--;
            if (health == 1)
            {
                Debug.Log("Crystal damaged!");
                coreObj.GetComponent<SpriteRenderer>().sprite = crushedSprite;
            }
            if (health == 0)
            {
                Debug.Log("Crystal destroyed!");
                anim.SetTrigger("Explode");
                puzzle.SolvePuzzle();
            }
            Destroy(collision.gameObject); // Destroy the bullet after hitting the crystal
        }
    }
}
