using UnityEngine;

public class Move : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public GameObject escPanel;
    public GameObject escPanelParent;
    public Health playerHealth;
    public bool escPanelIsShowed;
    GameObject escPanelInstance;
    Animator playerAnim;

    public float shiftCD;
    public float shiftCDMax;

    public bool isInvincible = false;

    public float sprintMultiplier = 2f;
    public float dashDistance = 5f; // Ô³êñîâàíà â³äñòàíü ðèâêà
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
        playerAnim = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        rb.position = PlayerMove();
        if (Input.GetKeyUp(KeyCode.Escape) && playerHealth.playerHealthPoint > 0)
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isReloading && !isDashing)
        {
            isReloading = true;
            isDashing = true;
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            Vector2 dashDirection = GetDashDirection();
            rb.velocity = dashDirection * (speed * sprintMultiplier);
            Invoke(nameof(StopDashing), 0.1f);
            dashTime = dashTimeMax;

        }
        if (isReloading)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isReloading = false;
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
            playerAnim.SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x + speed * Time.deltaTime, rb.position.y - (speed / 2) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            playerAnim.SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x - (speed / 2) * Time.deltaTime, rb.position.y - speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        {
            playerAnim.SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x - speed * Time.deltaTime, rb.position.y + (speed / 2) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            playerAnim.SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x + (speed / 2) * Time.deltaTime, rb.position.y + speed * Time.deltaTime);
        }
        //================
        else if (Input.GetKey(KeyCode.D))
        {
            playerAnim.SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x + speed * Time.deltaTime, rb.position.y);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            playerAnim.SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x, rb.position.y - speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            playerAnim.SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x - speed * Time.deltaTime, rb.position.y);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            playerAnim.SetBool("IsMove", true);
            rb.position = new Vector2(rb.position.x, rb.position.y + speed * Time.deltaTime);
        }
        else
        {
            playerAnim.SetBool("IsMove", false);
        }
        return new Vector2(rb.position.x, rb.position.y);
    }
  
}
