using UnityEngine;
using UnityEngine.Tilemaps;


public class RotationTile : MonoBehaviour
{
    public Tilemap tilemap; // ��������� �� Tilemap, �� ���������������� �����

    public Quaternion[] rotationOptions; // ����� � ������ ��������� ��� �����

    void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);

            // ����������, �� ���� �� � �������
            if (tile != null)
            {
                // �������� ���������� ������ ������� � ������ rotationOptions
                int randomIndex = Random.Range(0, rotationOptions.Length);
                Quaternion randomRotation = rotationOptions[randomIndex];

                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, randomRotation, Vector3.one);
                tilemap.SetTransformMatrix(position, matrix);
            }
        }
    }
}


