using UnityEngine;

public class SlowArea : MonoBehaviour
{
    public float dirtElement;
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyState move = collision.GetComponent<EnemyState>();
            GameManager.Instance.FindStatName("slowedTime", Time.fixedDeltaTime);
            EnemyController.instance.SlowEnemy(move, 1f, 0.5f / dirtElement);
            ElementActiveDebuff debuff = collision.GetComponentInParent<ElementActiveDebuff>();
            if (debuff != null)
            {
                debuff.StartCoroutine(debuff.EffectTime(Elements.status.Dirt, 5));
            }
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy") && collision.GetComponentInParent<Forward>() != null)
    //    {
    //        collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax;
    //    }
    //}
}
