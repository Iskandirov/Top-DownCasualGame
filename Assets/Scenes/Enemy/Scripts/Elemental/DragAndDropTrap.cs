using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropTrap : MonoBehaviour
{
    public float levitateAmplitude = 0.2f;
    public float levitateSpeed = 2f;
    public float liftHeight = 1.5f;
    public float floatHeight = 1.5f;
    public float followSpeed = 5f;
    public float dropFallSpeed = 5f;

    private bool isBeingDragged = false;
    private bool isLevitating = false;
    private Vector3 basePosition;
    private float levitateTime;
    private Vector3 originalGroundPosition;

    private Transform dragTarget;
    [SerializeField] private GameObject dropPoint;
    [SerializeField] private GameObject EWord;
    private bool dragging = false;
    private bool inArea = false;

    public bool IsBeingDragged => isBeingDragged;


    private void Start()
    {
        dragTarget = PlayerManager.instance.transform;
        dropPoint.SetActive(false);
        EWord.SetActive(false);
        originalGroundPosition = transform.position;
        basePosition = originalGroundPosition;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inArea)
        {
            if (!isBeingDragged)
                StartDrag(PlayerManager.instance.transform);
            else
                StopDrag();
        }

        if (isBeingDragged)
        {
            FollowDragTarget();
        }
        else if (isLevitating)
        {
            //LevitateMotion();
        }
    }

    public void StartDrag(Transform target)
    {
        StopAllCoroutines();

        dragTarget = target;
        isBeingDragged = true;
        isLevitating = false;
        dropPoint.SetActive(true);

        basePosition = transform.position;

        StartCoroutine(SmoothLift());
    }

    public void StopDrag()
    {
        isBeingDragged = false;
        dragTarget = null;
        dropPoint.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(SmoothDrop());
    }

    private void FollowDragTarget()
    {
        if (dragTarget == null) return;

        Vector3 baseTarget = dragTarget.position + Vector3.up * floatHeight;
        float floatingY = baseTarget.y + Mathf.Sin(Time.time * levitateSpeed) * levitateAmplitude;

        Vector3 targetPos = new Vector3(baseTarget.x, floatingY, baseTarget.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        originalGroundPosition = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z);
    }
    private void LevitateMotion()
    {
        Debug.Log("LevitateMotion called");
        Vector3 pos = transform.position;
        pos.y = basePosition.y + Mathf.Sin(Time.time * levitateSpeed) * levitateAmplitude;
        transform.position = pos;
    }

    private IEnumerator SmoothLift()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * liftHeight;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        basePosition = targetPos;
    }
    public IEnumerator SmoothDropAndWait()
    {
        yield return StartCoroutine(SmoothDrop());
    }

    public void ResetState()
    {
        isBeingDragged = false;
        isLevitating = false;
        dragTarget = null;
        dropPoint.SetActive(false);
        StopAllCoroutines();
        // Можливо, ще скинути basePosition, originalGroundPosition, якщо потрібно
    }
    private IEnumerator SmoothDrop()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, originalGroundPosition.y + 0.2f, startPos.z); // Невелике падіння

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * dropFallSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        originalGroundPosition = targetPos;
        basePosition = targetPos;
        isLevitating = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EWord.SetActive(true);
            inArea = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EWord.SetActive(false);
            inArea = false;
        }
    }
}
