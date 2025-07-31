using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.ParticleSystem;

public class PlatformPress : MonoBehaviour
{
    [Header("Base platform Settrung")]
    public Animator anim;
    public Transform player;
    public CinemachineVirtualCamera followCamera;
    public CinemachineVirtualCamera focusCamera;

    [Header("Rotation Mirror Trap")]
    public bool mirrorTrap = false;
    public bool pressed;
    public GameObject rorateObject;
    public GameObject rorateGalssAngleObject;
    public float rorationSpeed;
    public float minAngle = 0f;
    public float maxAngle = 90f;
    public bool isGlass;
    private int rotationDirection = 1; // 1 = ������, -1 = �����
    public float targetAngle = 0f; // ���, �� ����� ������� �����������

    private bool isPaused = false;
    private float pauseTimer = 0f;
    private const float pauseDuration = 2f; // 2 �������
    private bool wasPausedOnTarget = false;


    [Header("Spike Trap")]
    public bool spikeTrap = false;
    public List<Dissolve> dissolves;

    [Header("Wall Rotate Trap")]
    public bool wallTrap = false;
    public WallSignTrap wallSignTrap; 
    
    [Header("Wall Draw Trap")]
    public bool drawTrap = false;
    public DrawComparer wallDrawTrap;
    public WallDraw drawwMesh;
    public SpriteRenderer exampleMesh;
    public WallDrawresult result;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.transform;
    }
    public void FalsePressStatus()
    {
        pressed = false;
    }
    public void TruePressStatus()
    {
        pressed = true;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (pressed && (mirrorTrap || wallTrap))
        {
            float currentAngle;
            if (!isGlass)
            {
                currentAngle = rorateObject.transform.eulerAngles.z;
                currentAngle = (currentAngle + 360f) % 360f;
            }
            else
            {
                currentAngle = rorateObject.transform.eulerAngles.y;
                currentAngle = (currentAngle + 360f) % 360f;
            }

            // ���� ��'��� � ����
            if (isPaused)
            {
                pauseTimer += Time.fixedDeltaTime;
                if (pauseTimer >= pauseDuration)
                {
                    isPaused = false;
                    pauseTimer = 0f;
                    wasPausedOnTarget = true; // ϳ��� ����� ���������� ��� ���
                }
                return;
            }

            // ���� ������� targetAngle (� ��������) � �� �� ���� ����� �� ����� ������
            if (!wasPausedOnTarget && Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 0.5f)
            {
                var magic = rorateObject?.GetComponent<MagicCircle>();
                if (magic != null && magic.anim != null)
                {
                    magic.anim.SetBool("isBooled", true);
                }
                isPaused = true;
                pauseTimer = 0f;
                if (wallTrap)
                {
                    wallSignTrap.GlowSing(rorateObject,true);
                }
                return;
            }

            // ���� ������ �� ��� targetAngle � ���������� ����� ����� ��� ���������� ������
            if (wasPausedOnTarget && Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 1f)
            {
                var magic = rorateObject?.GetComponent<MagicCircle>();
                if (magic != null && magic.anim != null)
                {
                    magic.anim.SetBool("isBooled", false);
                }
                wasPausedOnTarget = false;
                if (wallTrap)
                {
                    wallSignTrap.GlowSing(rorateObject, false);
                }
            }
            if (mirrorTrap)
            {
                // ���� �������� ��� ��������� ��� � ��������
                if (rotationDirection > 0 && Mathf.DeltaAngle(currentAngle, maxAngle) <= 0.5f)
                    rotationDirection = -1;
                else if (rotationDirection < 0 && Mathf.DeltaAngle(currentAngle, minAngle) >= -0.5f)
                    rotationDirection = 1;
            }

            if (!isGlass)
            {
                rorateObject.transform.Rotate(0, 0, rorationSpeed * rotationDirection * Time.fixedDeltaTime);
            }
            else
            {
                rorateObject.transform.Rotate(0, rorationSpeed * rotationDirection * Time.fixedDeltaTime, 0);

                // �������� ��� Y � ������� [-45, 45]
                float yAngle = rorateObject.transform.eulerAngles.y;
                if (yAngle > 180f) yAngle -= 360f;
                yAngle = Mathf.Clamp(yAngle, minAngle, maxAngle);

                float zAngle = Mathf.Lerp(0f, 180f, (yAngle + 45f) / 90f);

                rorateGalssAngleObject.transform.eulerAngles = new Vector3(
                    rorateGalssAngleObject.transform.eulerAngles.x,
                    rorateGalssAngleObject.transform.eulerAngles.y,
                    zAngle
                );
            }
        }
        else if(mirrorTrap || wallTrap)
        {
            // ���� ������� ����� � ���������, ������� ����� � ���������
            isPaused = false;
            pauseTimer = 0f;
            wasPausedOnTarget = false;
        }
    }

    public void SpikeTrapDissovle(int mainCamPriority, int PointCamPriority)
    {
        if (spikeTrap)
        {

            focusCamera.Priority = PointCamPriority;
            followCamera.Priority = mainCamPriority; // ����� �������
            foreach (Dissolve dis in dissolves)
            {
                if (dis != null)
                {
                    dis.isDissolving = true;
                    dis.AppearOrVanish();
                }
            }
        }
    }
    public void CompareDrawing()
    {
        if (wallDrawTrap)
        {
            float similarity = wallDrawTrap.Compare() * 100f;
            //particle.StartDisintegration();
            Debug.Log($"Similarity: {similarity:F2}%");
            if (similarity >= 60)
            {
                result.SetResult(true);
            }
            else
            {
                result.SetResult(false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("Pressed", true);
            SpikeTrapDissovle(10,20);
            CompareDrawing();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("Pressed", false);
            SpikeTrapDissovle(20, 0);
        }
    }

}
