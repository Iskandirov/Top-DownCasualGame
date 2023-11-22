using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealthTutorial : MonoBehaviour
{
    TextAppear text;
    public float health;
    public bool isBoss;
    // Start is called before the first frame update
    void Start()
    {
        text = FindObjectOfType<TextAppear>();
    }
    private void FixedUpdate()
    {
        if (health <= 0)
        {
            text.tutor.PhasePlus();
            text.isShooting = false;
            text.tutor.playerMove.GetComponent<Shoot>().enabled = false;
            text.tutor.playerMove.enabled = false;
            text.tutor.playerMove.rb.velocity = Vector3.zero;

            if (isBoss)
            {
                GetComponent<DropItems>().isTutor = true;
                GetComponent<DropItems>().OnDestroyBoss();
            }
            Destroy(gameObject);
        }
    }
}
