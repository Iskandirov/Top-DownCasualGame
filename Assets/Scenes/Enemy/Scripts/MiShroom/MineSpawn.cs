using UnityEngine;

public class MineSpawn : MonoBehaviour
{
    public int mineCount;
    public MiShroomMine mine;
    public float delay;
    float delayMax;
    public float radius;
    Transform objTransform;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        mineCount = 0;
        objTransform = transform;
        delayMax = delay;
        SetAlphaRecursively();
    }
    private void SetAlphaRecursively()
    {
        anim.SetTrigger("Invisible");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (mineCount < 10)
        {
            delay -= Time.fixedDeltaTime;
        }
        if (mineCount < 10 && delay <= 0)
        {
            Random.InitState((int)Time.time);
            MiShroomMine a = Instantiate(mine, new Vector3(objTransform.position.x + Random.Range(-radius, radius), objTransform.position.y + Random.Range(-radius, radius)), Quaternion.identity, objTransform.parent);
            a.parent = this;
            delay = delayMax;
            mineCount++;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            SetAlphaRecursively();
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            SetAlphaRecursively();
        }
    }
}
