using System.Collections;
using UnityEngine;

public class Tornadic_Rush : MonoBehaviour
{
    [SerializeField] Vector3 dir;
    [SerializeField] GameObject objArea;
    [SerializeField] float interval = 10;
    [SerializeField] float speedRush = 10;
    [SerializeField] bool state = false;
    [SerializeField] bool rush = false;
    [SerializeField] int playerToched = 0;
    Transform objTransofrm;
    Vector2 object1Pos;
    Vector2 object2Pos;
    [SerializeField] Animator anim;
    private void Start()
    {

        objTransofrm = transform;
        StartCoroutine(TornadorRush());
    }
    public IEnumerator TornadorRush()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            RushAnim();
        }
    }
    private void FixedUpdate()
    {
        if (rush)
        {
            Vector2 direction = object2Pos - object1Pos;
            direction.Normalize();
            direction.x *= objTransofrm.rotation.y < 0 ? -1 : 1;
            objTransofrm.Translate(direction * speedRush * Time.deltaTime);
            if (playerToched == 1)
            {
                PlayerManager.instance.TakeDamage(5f);
            }
        }
    }
    public void RushAnim()
    {
        state = !state;
        anim.SetTrigger("Rush");
    }
    public void RushMove()
    {
        rush = !rush;
    }
    public void GetDirection()
    {
        object1Pos = transform.position;
        object2Pos = PlayerManager.instance.objTransform.transform.position;

        float angle = Mathf.Atan2(object2Pos.y - object1Pos.y, object2Pos.x - object1Pos.x);

        angle *= Mathf.Rad2Deg;
        angle += 180;
        objArea.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Shield"))
        {
            collision.collider.GetComponent<Shield>().healthShield -= 5f;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", 5f);
        }
        else if (collision.collider.CompareTag("Player"))
        {
            playerToched = 1;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Shield"))
        {
            playerToched = 0;
        }
    }
}
