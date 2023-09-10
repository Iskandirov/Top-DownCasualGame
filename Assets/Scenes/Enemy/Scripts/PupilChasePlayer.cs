using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilChasePlayer : MonoBehaviour
{
    Transform player; // �������
    public Transform eyePupil; // ������
    public Transform eyeBounds; // ��� ���

    public float maxPupilSpeed = 5.0f; // ����������� �������� ���� ������
    public float maxPupilDistance = 0.5f; // ����������� ������� ������ �� ������ ���
    public void Start()
    {
        player = FindObjectOfType<Move>().transform;
    }
    private void Update()
    {
        // ��������� ������ �� ������ �� ������
        Vector3 directionToPlayer = player.position - eyePupil.position;

        // ���������� ���� ������� ������
        Vector3 newPosition = eyePupil.position + directionToPlayer.normalized * maxPupilSpeed * Time.deltaTime;

        // �������� ��� ������ � ����� ������ ���
        Vector3 boundedPosition = Vector3.ClampMagnitude(newPosition - eyeBounds.position, eyeBounds.localScale.x / 2 - maxPupilDistance);
        eyePupil.position = boundedPosition + eyeBounds.position;
    }
}
