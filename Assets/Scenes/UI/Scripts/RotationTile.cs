using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RotationTile : MonoBehaviour
{
    public Tilemap tilemap; // ��������� �� Tilemap, �� ���������������� �����
    public List<Image> floor; // ��������� �� Tilemap, �� ���������������� �����
    public Quaternion[] rotationOptions; // ����� � ������ ��������� ��� �����
    public bool isTileMap;
    void Start()
    {
        if (isTileMap)
        {
            BoundsInt bounds = tilemap.cellBounds;

            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(position);

                // ����������, �� ���� �� � �������
                if (tile != null)
                {
                    // �������� ��������� �������� ��� ������ �������
                    int randomValue = Random.Range(0, 2);

                    Quaternion rotation;

                    // �������� ������� �� ����� ����������� ��������
                    if (randomValue == 0)
                    {
                        rotation = Quaternion.Euler(0f, 0f, 0f); // 0 �������
                    }
                    else
                    {
                        rotation = Quaternion.Euler(0f, 0f, 180f); // 180 �������
                    }

                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
                    tilemap.SetTransformMatrix(position, matrix);
                }
            }
        }
        else
        {
            foreach (Image spriteRenderer in floor)
            {
                int randomValue = Random.Range(0, 2);

                Quaternion rotation;

                if (randomValue == 0)
                {
                    rotation = Quaternion.Euler(0f, 0f, 0f); // 0 �������
                }
                else
                {
                    rotation = Quaternion.Euler(0f, 0f, 180f); // 180 �������
                }

                spriteRenderer.transform.rotation = rotation;
            }
        }
    }
}


