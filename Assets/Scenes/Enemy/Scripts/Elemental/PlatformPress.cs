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

public class PlatformPress : MonoBehaviour
{
    [Header("Base platform Settrung")]
    public Animator anim;
    public Transform player;
    public CinemachineVirtualCamera followCamera;
    public CinemachineVirtualCamera focusCamera;
    public Vector3 focusCameraCoordinate;
    public PuzzleController puzzle;
    public int objectOnPlatform = 0; //  ≥льк≥сть об'Їкт≥в на платформ≥, €к≥ можуть бути активован≥

    [Header("Rotation Mirror Trap")]
    public bool mirrorTrap = false;
    public bool pressed;
    public GameObject rorateObject;
    public GameObject rorateGalssAngleObject;
    public float rorationSpeed;
    public float minAngle = 0f;
    public float maxAngle = 90f;
    public bool isGlass;
    private int rotationDirection = 1; // 1 = вперед, -1 = назад
    public float targetAngle = 0f; //  ут, до €кого потр≥бно повернутис€

    private bool isPaused = false;
    private float pauseTimer = 0f;
    private const float pauseDuration = 2f; // 2 секунда
    private bool wasPausedOnTarget = false;
    public Vector2 mirrorFocusCoordinate;

    [Header("Spike Trap")]
    public bool spikeTrap = false;
    public bool spikeTrapSolver = false;
    public List<Dissolve> dissolves;
    public Vector2 spikeFocusCoordinate;

    [Header("Wall Rotate Trap")]
    public bool wallTrap = false;
    public WallSignTrap wallSignTrap;
    public Vector2 rotateFocusCoordinate;

    [Header("Wall Draw Trap")]
    public bool drawTrap = false;
    public DrawComparer wallDrawTrap;
    public WallDraw drawwMesh;
    public SpriteRenderer exampleMesh;
    public WallDrawresult result;
    public Vector2 drawFocusCoordinate;

    [Header("Drag&Drop Trap")]
    public bool isDragTrap = false;
    public UnityEngine.UI.Image door;
    public BoxCollider2D doorBox;
    public UnityEngine.UI.Image doorGlow;
    public Vector2 dragFocusCoordinate;

    public float fillSpeed = 0.5f; // Ўвидк≥сть заповненн€
    public float drainSpeed = 0.5f; // Ўвидк≥сть спаданн€
    public float requiredFill = 1f; //  оли дос€гаЇ цього значенн€ Ч спрацьовуЇ д≥€

    public Transform doorClosedPosition;  // ѕочаткова позиц≥€ дверей (вгор≥)
    public Transform doorOpenedPosition;  //  уди мають опуститись двер≥ (вниз)
    public float doorMoveSpeed = 1f;
    public float doorReturnSpeed = 3f;
    public float deplificator = 3f;

    private bool actionTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        focusCameraCoordinate = GetComponentInParent<PuzzleController>().transform.position;
        puzzle = GetComponentInParent<PuzzleController>();
        player = PlayerManager.instance.transform;
        if(isDragTrap)
            doorBox = door.GetComponent<BoxCollider2D>();
        focusCamera = GameObject.FindGameObjectWithTag("AdditionalCamera").GetComponent<CinemachineVirtualCamera>();
        followCamera = GameObject.FindGameObjectWithTag("Respawn").GetComponent<CinemachineVirtualCamera>();
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

            // якщо об'Їкт у пауз≥
            if (isPaused)
            {
                pauseTimer += Time.fixedDeltaTime;
                if (pauseTimer >= pauseDuration)
                {
                    isPaused = false;
                    pauseTimer = 0f;
                    wasPausedOnTarget = true; // ѕ≥сл€ паузи дозвол€Їмо рух дал≥
                }
                return;
            }

            // якщо дос€гли targetAngle (з допуском) ≥ ще не було паузи на цьому проход≥
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

            // якщо вийшли за меж≥ targetAngle Ч дозвол€Їмо знову паузу при наступному проход≥
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
                // «м≥на напр€мку при дос€гненн≥ меж з допуском
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

                // ќтримуЇмо кут Y у д≥апазон≥ [-45, 45]
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
            // якщо гравець з≥йшов з платформи, скидаЇмо паузу ≥ прапорець
            isPaused = false;
            pauseTimer = 0f;
            wasPausedOnTarget = false;
        }

        // Drag&Drop Trap
        if (isDragTrap)
        {
            if (pressed)
            {
                doorGlow.fillAmount += fillSpeed * Time.deltaTime;
                if (doorGlow.fillAmount >= requiredFill)
                {
                    doorGlow.fillAmount = requiredFill;
                    TriggerAction();
                }
            }
            else if (!pressed && doorGlow.fillAmount > 0f)
            {
                actionTriggered = false;
                doorGlow.fillAmount -= drainSpeed * Time.deltaTime;
                doorGlow.fillAmount = Mathf.Clamp01(doorGlow.fillAmount);
            }


            if (actionTriggered)
            {
                MoveDoor(doorOpenedPosition.position, doorMoveSpeed,false);
                door.fillAmount -= (doorMoveSpeed / deplificator) * Time.deltaTime;

                if (door.fillAmount <= 0)
                {
                    door.fillAmount = 0;
                    TriggerOpenDoorAction(false);
                }
            }

            if (!pressed)
            {
                TriggerOpenDoorAction(true);
                MoveDoor(doorClosedPosition.position, doorReturnSpeed,true);
                door.fillAmount += (doorReturnSpeed / deplificator) * Time.deltaTime;
            }
        }
    }
    public void CamerasPriority(int mainCamPriority, int PointCamPriority)
    {
        focusCamera.Priority = PointCamPriority;
        followCamera.Priority = mainCamPriority;
    }
    public void SpikeTrapDissovle(bool spikeDissovle)
    {
        if (spikeTrap)
        {
            foreach (Dissolve dis in dissolves)
            {
                if (dis != null)
                {
                    dis.isDissolving = spikeDissovle;
                    dis.AppearOrVanish();
                }
            }
            if(spikeTrapSolver)
            {
                puzzle.SolvePuzzle();
            }
        }
    }
    public void CompareDrawing()
    {
        if (drawTrap)
        {
            float similarity = wallDrawTrap.Compare() * 100f;
            //particle.StartDisintegration();
            Debug.Log($"Similarity: {similarity:F2}%");
            if (similarity >= 60)
            {
                result.SetResult(true);
                puzzle.SolvePuzzle();
            }
            else
            {
                result.SetResult(false);
            }
        }
    }
    private void TriggerAction()
    {
        actionTriggered = true;
        // “ут встав свою лог≥ку: в≥дкрити двер≥, телепорт, тощо
    }
    private void TriggerOpenDoorAction(bool active)
    {
        door.GetComponent<BoxCollider2D>().enabled = active; // ¬имикаЇмо коллайдер, щоб гравець не м≥г взаЇмод≥€ти з дверима
        doorGlow.gameObject.SetActive(active); // ¬имикаЇмо/вмикаЇмо в≥зуальний ≥ндикатор
    }
    private void MoveDoor(Vector3 targetPosition, float speed,bool closeDoor)
    {
        Vector3 previousPosition = door.transform.position;

        // –ух дверей
        door.transform.position = Vector3.MoveTowards(
            previousPosition,
            targetPosition,
            speed * Time.deltaTime
        );
        doorGlow.fillOrigin = closeDoor ? (int)UnityEngine.UI.Image.OriginVertical.Top : (int)UnityEngine.UI.Image.OriginVertical.Bottom; // ¬становлюЇмо початок заповненн€ зверху
        // –ух колайдера в протилежному напр€мку по Y
        Vector3 movementDelta = door.transform.position - previousPosition;

        Vector2 currentOffset = doorBox.offset;
        currentOffset.y -= movementDelta.y;
        doorBox.offset = currentOffset;
    }
    public void ResetState()
    {
        actionTriggered = false;
        doorGlow.fillAmount = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            objectOnPlatform++;
            if (objectOnPlatform > 0)
            {
                anim.SetBool("Pressed", true);
            }
            CamerasPriority(10, 20);
            SpikeTrapDissovle(false);
            CompareDrawing();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            objectOnPlatform--;
            if (objectOnPlatform <= 0)
            {
                anim.SetBool("Pressed", false);
            }
            CamerasPriority(20, 0);
            SpikeTrapDissovle(true);
        }
    }

}
