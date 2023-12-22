using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VortexSpawner : MonoBehaviour
{
    public Vortex vortex;
    public float step;
    public float stepMax;
    public float radius;
    public float damage;
    public float lifeTime;
    public bool isFive;
    int buttonActivateSkill;
    PlayerManager player;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        step = gameObject.GetComponent<CDSkillObject>().CD;
        StartCoroutine(SetBumberToSkill());
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
        buttonActivateSkill = gameObject.GetComponent<CDSkillObject>().num + 1;
        keyCode = (KeyCode)((int)KeyCode.Alpha0 + buttonActivateSkill);
    }
    void FixedUpdate()
    {
        step -= Time.fixedDeltaTime;
    }
    private void Update()
    {
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 1.9f; // Задаємо Z-координату для об'єкта
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vortex a = Instantiate(vortex, worldPosition, Quaternion.identity);
            a.damage = damage * player.Wind;
            a.lifeTime = lifeTime;
            a.transform.localScale = new Vector2(radius * player.Steam, radius * player.Steam);
            step = stepMax;
            if (isFive)
            {
                Vortex b = Instantiate(vortex, new Vector3(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20), 1.9f), Quaternion.identity);
                b.damage = damage * player.Wind;
                b.lifeTime = lifeTime;
                b.transform.localScale = new Vector2(radius * player.Steam, radius * player.Steam);
            }
        }
    }
}
