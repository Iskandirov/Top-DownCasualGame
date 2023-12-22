using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilChasePlayer : MonoBehaviour
{
    PlayerManager player; // �������
    public Transform eyePupil; // ������
    public Transform eyeBounds; // ��� ���

    public float maxPupilSpeed = 5.0f; // ����������� �������� ���� ������
    public float maxPupilDistance = 0.5f; // ����������� ������� ������ �� ������ ���
    private void Start()
    {
        player = PlayerManager.instance;
    }
    private void FixedUpdate()
    {
        // ��������� ������ �� ������ �� ������
        Vector3 directionToPlayer = player.transform.position - eyePupil.position;

        // ���������� ���� ������� ������
        Vector3 newPosition = eyePupil.position + directionToPlayer.normalized * maxPupilSpeed * Time.fixedDeltaTime;

        // �������� ��� ������ � ����� ������ ���
        Vector3 boundedPosition = Vector3.ClampMagnitude(newPosition - eyeBounds.position, eyeBounds.localScale.x / 2 - maxPupilDistance);
        eyePupil.position = boundedPosition + eyeBounds.position;
    }
}
