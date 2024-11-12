using TMPro;
using UnityEngine;
[DefaultExecutionOrder(15)]
public class CompiledGameInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] info;
    // Start is called before the first frame update
    void Start()
    {
        info[0].text = GameManager.Instance.enabled.ToString();
        //info[1].text = EnemyState.instance.enabled.ToString();
        //info[3].text = EnemyState.instance.enemiesPool.Count.ToString();
        //info[4].text = EnemyState.instance.enemies.Count.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //info[2].text = EnemyState.instance.children.Count.ToString();
    }
}
