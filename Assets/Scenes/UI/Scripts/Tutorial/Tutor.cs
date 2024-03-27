using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
[Serializable]
public class LightData
{
    public KeyCode Key;
    public Light2D Light;
    public bool isPressed;
}
[DefaultExecutionOrder(10)]
public class Tutor : MonoBehaviour
{
    public List<LightData> light2D;
    public int phase = 0;
    public Color32 FinalColor;
    public static Light2D wLight;
    public static Light2D aLight;
    public static Light2D sLight;
    public static Light2D dLight;
    public Light2D ShiftLight;
    int lightCount = 0;
    public Animator parentPhase1;
    public Animator parentPhase2;
    public Animator parentPhase3;
    public TextAppear text;
    public Light2D playerLight;
    public PlayerManager player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        BlockMoveAndShoot();
    }
    public IEnumerator TurnOn()
    {
        while (playerLight.intensity <= 1f && playerLight.falloffIntensity >= 0.5f)
        {
            // Збільште яскравість світла на 1/10
            playerLight.intensity += 0.0004f;

            // Збільште падіння інтенсивності світла на 1/10
            playerLight.falloffIntensity -= 0.0002f;

            // Зачекайте 0,1 секунди
            yield return new WaitForSeconds(0.001f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        player.attackSpeed -= Time.deltaTime;
        if (player.attackSpeed < 0 && text.isShooting && Input.GetMouseButton(0))
        {
            Instantiate(player.bullet);
            player.attackSpeed = player.attackSpeedMax;
        }
        if (phase == 0 && text.anim.GetBool("FadeOut") == true)
        {
            StartCoroutine(TurnOn());
            player.enabled = true;
            player.rb.velocity = Vector3.zero;
            //MoveOn();
            foreach (var keyLightPair in light2D)
            {
                KeyCode key = keyLightPair.Key;
                Light2D light = keyLightPair.Light;

                if (Input.GetKeyDown(key) && !keyLightPair.isPressed)
                {
                    light.color = FinalColor;
                    lightCount++;
                    keyLightPair.isPressed = true;
                }
            }
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    if (!isPressed[0])
            //    {
            //        wLight.color = FinalColor;
            //        lightCount++;
            //        isPressed[0] = true;
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    if (!isPressed[1])
            //    {
            //        aLight.color = FinalColor;
            //        lightCount++;
            //        isPressed[1] = true;
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    if (!isPressed[2])
            //    {
            //        sLight.color = FinalColor;
            //        lightCount++;
            //        isPressed[2] = true;
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    if (!isPressed[3])
            //    {
            //        dLight.color = FinalColor;
            //        lightCount++;
            //        isPressed[3] = true;
            //    }
            //}
            if (lightCount >= 4)
            {
                parentPhase1.SetBool("IsFadeOut", true);
                PhasePlus();
                BlockMoveAndShoot();
            }
        }
        if (phase == 1)
        {
            player.enabled = false;
        }
        if (phase == 3)
        {
            if (parentPhase2 != null)
            {
                BlockMoveAndShoot();
                parentPhase2.SetBool("IsFadeOut", true);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                ShiftLight.color = FinalColor;
                parentPhase3.SetBool("IsFadeOut", true);
                PhasePlus();
                Invoke("BlockMoveAndShoot", 0.3f);
            }
        }
    }
    public void BlockMoveAndShoot()
    {
        player.enabled = false;
        text.isShooting = false;
        player.rb.velocity = Vector3.zero;
    }
    public void PhasePlus()
    {
        phase++;
        if (text.anim != null)
        {
            text.anim.SetBool("FadeOut", false);
        }
    }
    public void MoveOn()
    {
        player.enabled = true;
        text.isShooting = true;
        player.rb.velocity = Vector3.zero;
    }
    public void Destroy()
    {
        Destroy(parentPhase1.gameObject);
    }
}
