using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public GameObject escPanel;
    public GameObject escPanelParent;
    public bool escPanelIsShowed;
    GameObject escPanelInstance;

    public float shiftCD;
    public float shiftCDMax;

    public bool isInvincible = false;

    public float sprintMultiplier = 2f;
    public float dashDistance = 5f; // Фіксована відстань ривка
    public float dashTime;
    public float dashTimeMax;
    public float dashTimeStart;

    private bool isReloading = false;
    public bool isDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        dashTimeStart = dashTimeMax;
        Time.timeScale = 1f;
    }
    // Update is called once per frame
    void Update()
    {
        rb.position = PlayerMove();

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!escPanelIsShowed)
            {
                escPanelInstance = Instantiate(escPanel, escPanelParent.transform);
                escPanelIsShowed = true;
            }
            else
            {
                escPanelIsShowed = false;
                Destroy(escPanelInstance);
            }
        }
        // Перевірка на натискання кнопки шифт
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isReloading && !isDashing)
        {
            isReloading = true;
            isDashing = true;
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            Vector2 dashDirection = GetDashDirection();
            rb.velocity = dashDirection * (speed * sprintMultiplier);
            Invoke(nameof(StopDashing), 0.1f); // Зупиняємо ривок через 0.1 секунду
        }
        if (isReloading)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isReloading = false;
                dashTime = dashTimeMax;
            }
        }
    }
    
    private Vector2 GetDashDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerToMouse = mousePosition - transform.position;
        return playerToMouse.normalized * dashDistance;
    }

    private void StopDashing()
    {
        isDashing = false;
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        rb.velocity = Vector2.zero;
    }

    //-----------
    public Vector2 PlayerMove()
    {
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x + speed * Time.deltaTime, rb.position.y - (speed / 2) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x - (speed / 2) * Time.deltaTime, rb.position.y - speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x - speed * Time.deltaTime, rb.position.y + (speed / 2) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x + (speed / 2) * Time.deltaTime, rb.position.y + speed * Time.deltaTime);
        }
        //================
        else if (Input.GetKey(KeyCode.D))
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x + speed * Time.deltaTime, rb.position.y);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x, rb.position.y - speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x - speed * Time.deltaTime, rb.position.y);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x, rb.position.y + speed * Time.deltaTime);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("IsMove", false);
        }
        //Debug.Log("x: " + rb.position.x + "y: " +  rb.position.y);
        return new Vector2(rb.position.x, rb.position.y);
    }
  
}
