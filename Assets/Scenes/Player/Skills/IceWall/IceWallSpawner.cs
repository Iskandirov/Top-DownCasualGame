using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWallSpawner : MonoBehaviour
{
    public IceWall vortex;
    public float step;
    public float stepMax;
    public float wide;
    public float damage;
    public float cold;
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
            IceWall a = Instantiate(vortex, worldPosition, Quaternion.identity);
            a.lifeTime = lifeTime;
            a.transform.localScale = new Vector2(wide * player.Steam, wide * player.Steam);
            a.damage = damage;
            a.cold = cold * player.Cold;
            step = stepMax;
        }
    }
}
