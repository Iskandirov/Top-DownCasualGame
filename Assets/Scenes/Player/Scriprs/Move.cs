using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public float speedMax;
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


    public bool isSlowingDown = false;
    public float slowdownEndTime;
    public float slowPercent;
    public float axisX;
    public float axisY;

    public bool otherPanelOpened;
    // Start is called before the first frame update
    void Start()
    {
        //float o = 100;
        //for (int i = 0; i < 25; i++)
        //{
        //    o += o * 0.2f;
        //    Debug.Log(o);
        //}
        speedMax = speed;
        dashTimeStart = dashTimeMax;
        Time.timeScale = 1f;
        playerAnim = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        //move
        if (!isDashing)
        {
            rb.position = PlayerMove();

        }
        //can`t press pause after lose
        if (Input.GetKeyUp(KeyCode.Escape) && playerHealth.playerHealthPoint > 0 && !otherPanelOpened)
        {
            if (!escPanelIsShowed)
            {
                escPanelInstance = Instantiate(escPanel, escPanelParent.transform);
                escPanelIsShowed = true;
                otherPanelOpened = true;
            }
            else
            {
                otherPanelOpened = false;
                escPanelIsShowed = false;
                Destroy(escPanelInstance);
            }
        }
        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isReloading && !isDashing)
        {
            isReloading = true;
            isDashing = true;
            Vector2 dashDirection = GetDashDirection();
            rb.velocity = dashDirection.normalized * (speed * sprintMultiplier);
            Invoke(nameof(StopDashing), 0.2f);
            dashTime = dashTimeMax;
        }
        //Dash reload
        if (isReloading)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isReloading = false;
            }
        }
        //player slow
        if (isSlowingDown)
        {
            slowdownEndTime -= Time.deltaTime;
            speed = speedMax * slowPercent;
            if (slowdownEndTime <= 0)
            {
                speed = speedMax;
                isSlowingDown = false;
            }
        }
    }
    private void StopDashing()
    {
        isDashing = false;
        rb.velocity = Vector2.zero;
    }
    private Vector2 GetDashDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerToMouse = mousePosition - transform.position;
        return playerToMouse;
    }
    //-----------
    public Vector2 PlayerMove()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0)
        {
            playerAnim.SetBool("IsMove", true);
            if (axisX != horizontalInput)
            {
                StartCoroutine(SpeedSlow(0.2f));
                axisX = horizontalInput;
            }
        }
        else if (verticalInput != 0)
        {
            if (axisY != verticalInput)
            {
                StartCoroutine(SpeedSlow(0.2f));
                axisY = verticalInput;
            }
        }
        else
        {
            playerAnim.SetBool("IsMove", false);
        }

        rb.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
        return new Vector2(rb.position.x, rb.position.y);
    }
    public IEnumerator SpeedSlow(float time)
    {
        speed = speedMax * 0.7f;
        yield return new WaitForSeconds(time);
        speed = speedMax;
    }
}
